using log4net;
using MiNET;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MiNET.Worlds;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MyAuth
{

    [Plugin(PluginName = "MyAuth", Description = "Mysql Auth Plugin for MiNET", PluginVersion = "1.0", Author = "haniokasai")]
    public class Class1 : Plugin
    {
        public static Dictionary<string, Boolean> lged = new Dictionary<string, Boolean>();
        public static Dictionary<string, Boolean> prerg = new Dictionary<string, Boolean>();
        public static Dictionary<string, int> ct = new Dictionary<string, int>();

        public static ILog _log = LogManager.GetLogger("MyAuth");

        protected override void OnEnable()
        {
            //mysql.load();
            mysql.load();
            Context.Server.PlayerFactory.PlayerCreated += PlayerFactory_PlayerCreated;
            _log.Warn("Loaded");
            
            
            Context.PluginManager.LoadCommands(new Commands());// /helpを使えるようにする
        }

        public override void OnDisable()
        {
            mysql.shutdown();
        }

        private void PlayerFactory_PlayerCreated(object sender, PlayerEventArgs e)
        {
            var player = e.Player;
            player.PlayerJoin += Player_PlayerJoin;
            player.PlayerLeave += Player_PlayerLeave;
        }


        private void Player_PlayerLeave(object sender, PlayerEventArgs e)
        {
            Player player = e.Player;
            string name = player.Username.ToLower();
            ct.Remove(name);
            lged.Remove(name);
            prerg.Remove(name);
        }


        private void Player_PlayerJoin(object sender, PlayerEventArgs e)
        {
            Player player = e.Player;
            string name = player.Username.ToLower();
            String ip = player.EndPoint.Address.MapToIPv4().ToString();
            String cid = player.ClientUuid.ToString();
            Dictionary<string, string> map = mysql.get(name);
            if (!map.ContainsValue(name)) {
                player.SendMessage("[MyAuth]Please Register your account /register <passwd>");
                prerg.Add(name, true);
            }
            else
            {
                var date = DateTimeOffset.FromUnixTimeSeconds(long.Parse(map["llogin"])).ToLocalTime();
                player.SendMessage("[MyAuth] last login:" + date);
                var ctime = (int)mysql.ToUnixTime(DateTime.Now.ToUniversalTime());
                mysql.settime(name, ctime);
                if (map["ip"].Equals(ip) & map["uuid"].Equals(cid))
                {
                    player.SendMessage("[MyAuth]Logined!");
                    lged.Add(name, true);
                }
                else
                {
                    player.SendMessage("[MyAuth]Please Login to server /login <passwd>");
                }
            }
        }

       public static string toEn(string value)
        {
            //http://dobon.net/vb/dotnet/string/md5.html#section3
            //文字列をbyte型配列に変換する
            byte[] data = System.Text.Encoding.UTF8.GetBytes(value);

            //MD5CryptoServiceProviderオブジェクトを作成
            System.Security.Cryptography.SHA512CryptoServiceProvider sha512 =
                new System.Security.Cryptography.SHA512CryptoServiceProvider();

            //ハッシュ値を計算する
            byte[] bs = sha512.ComputeHash(data);

            //リソースを解放する
            sha512.Clear();
            string result = BitConverter.ToString(bs).ToLower().Replace("-", "");
            return result;
        }


        /////commands

        [Command(Name = "register", Description = "Register your account", Permission = "com.haniokasai.myauth.register")]
        public void register(Player player, string passwd)
        {
            string name = player.Username;
            string ip = player.EndPoint.Address.MapToIPv4().ToString();
            if (Class1.lged.ContainsKey(name))
            {
                player.SendMessage("[MyAuth]You dont have to do it.");
            }
            else
            {
                if (Class1.prerg.ContainsKey(name))
                {
                    String hashed = Class1.toEn(passwd);
                    mysql.regi(player, hashed);
                    Class1.lged.Add(name, true);
                    Class1.prerg.Remove(name);
                    player.SendMessage("[MyAuth]Registered! Passwd: " + passwd);
                }
            }


        }

        [Command(Name = "login", Description = "Login your account", Permission = "com.haniokasai.myauth.login")]
        public void login(Player player, string passwd)
        {
            string name = player.Username;
            string ip = player.EndPoint.Address.MapToIPv4().ToString();
            String uuid = player.ClientUuid.ToString();

            if (Class1.lged.ContainsKey(name))
            {
                player.SendMessage("[MyAuth]You dont have to do it.");

            }
            else
            {
                String hashed = Class1.toEn(passwd);
                if (mysql.login(name, hashed))
                {
                    Class1.lged.Add(name, true);
                    Class1.prerg.Remove(name);
                    mysql.setuuid(name, uuid, ip);
                    player.SendMessage("[MyAuth]Logined!");

                }
                else
                {
                    if (Class1.ct.ContainsKey(name))
                    {
                        Class1.ct.Add(name, Class1.ct[name] - 1);
                    }
                    else
                    {
                        Class1.ct.Add(name, 10);
                    }

                    player.SendMessage("[MyAuth]Please enter correct passwd. Remaining : " + Class1.ct[name]);
                    if (Class1.ct[name] <= 0)
                    {
                        //Server.getInstance().getIPBans().addBan(ip, "[MyAuth] 10 times passwd missing.", null, "[MyAuth]");

                        player.Level.BroadcastMessage("[MyAuth] " + name + " :missed passwd 10 times.");
                        Class1.ct.Remove(name);
                    }

                }
            }
        }

        [Command(Name = "changepasswd", Description = "changepasswd your account", Permission = "com.haniokasai.myauth.changepasswd")]
        public void changepasswd(Player player, string oldpasswd, string newpasswd, string retypenewpasswd)
        {
            if (!newpasswd.Equals(retypenewpasswd))
            {
                player.SendMessage("[MyAuth]Please enter correct new-passwd.");
            }
            else
            {
                String name = player.Username;

                if (mysql.login(name, Class1.toEn(oldpasswd)))
                {
                    mysql.remove(name);
                    mysql.regi(player, Class1.toEn(retypenewpasswd));
                    player.SendMessage("[MyAuth]Everything Ok, Your Now Passwd is : " + retypenewpasswd);
                }
                else
                {
                    player.SendMessage("[MyAuth]Please enter correct old-passwd.");
                }
            }
        }

        [Command(Name = "unregister", Description = "UnRegister your account", Permission = "com.haniokasai.myauth.unregister")]
        public void unregister(Player player, string playername, string unregisterpasswd)
        {
            if (mysql.remove(playername))
            {
                player.SendMessage("[MyAuth]Deleted : " + playername);
            }
            else
            {
                player.SendMessage("[MyAuth]there isnt such a player : " + playername);
            }

        }
        ///////////////////



        ////event////////////
        [PacketHandler]
        public Package onChat(McpeText packet, Player player)
        {
            if (!Class1.lged.ContainsKey(player.Username.ToLower()))
            {
                notlogin(player);
                return null;
            }
            return packet;
        }

        [PacketHandler]
        public Package onUseItem(McpeUseItem packet, Player player)
        {
            if (!Class1.lged.ContainsKey(player.Username.ToLower()))
            {
                notlogin(player);
                return null;
            }
            return packet;
        }

        [PacketHandler]
        public Package onInteract(McpeInteract packet, Player player)
        {
            if (!Class1.lged.ContainsKey(player.Username.ToLower()))
            {
                notlogin(player);
                return null;
            }
            return packet;
        }

        [PacketHandler]
        public Package onDropItem(McpeDropItem packet, Player player)
        {
            if (!Class1.lged.ContainsKey(player.Username.ToLower()))
            {
                notlogin(player);
                return null;
            }
            return packet;
        }

        [PacketHandler]
        public Package onCraftingEvent(McpeCraftingEvent packet, Player player)
        {
            if (!Class1.lged.ContainsKey(player.Username.ToLower()))
            {
                notlogin(player);
                return null;
            }
            return packet;
        }


        [PacketHandler]
        public Package onMovePlayer(McpeMovePlayer packet, Player player)
        {
            if (!Class1.lged.ContainsKey(player.Username.ToLower()))
            {
                notlogin(player);
                return null;
            }
            return packet;
        }


        [PacketHandler]
        public Package onMobArmorEquipment(McpeMobArmorEquipment packet, Player player)
        {
            if (!Class1.lged.ContainsKey(player.Username.ToLower()))
            {
                notlogin(player);
                return null;
            }
            return packet;
        }

        [PacketHandler]
        public Package onCommandStep(McpeCommandStep packet, Player player)
        {
            if (!Class1.lged.ContainsKey(player.Username.ToLower()))
            {
                if (!packet.commandName.Equals("register") || packet.commandName.Equals("login"))
                {
                    notlogin(player);
                }
                return null;
            }
            return packet;
        }


        public void OnBreak(object o, BlockBreakEventArgs e)
        {
            if (!Class1.lged.ContainsKey(e.Player.Username.ToLower()))
            {
                notlogin(e.Player);
                e.Cancel = true;
            }
        }


        public void OnPlace(object o, BlockPlaceEventArgs e)
        {
            if (!Class1.lged.ContainsKey(e.Player.Username.ToLower()))
            {
                notlogin(e.Player);
                e.Cancel = true;
            }
        }

        public void notlogin(Player player)
        {
            player.SendMessage("Please Login", MessageType.Popup);
        }



        /////まだまだ追加するよ！

    }



}
