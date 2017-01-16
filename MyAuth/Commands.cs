using MiNET;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using System;

namespace MyAuth
{
    class Commands : Plugin
    {
        protected override void OnEnable()
        {
            base.OnEnable();
        }

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
            }else
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


    }
}
