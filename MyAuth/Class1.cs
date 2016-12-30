using log4net;
using MiNET;
using MiNET.Net;
using MiNET.Plugins;
using MiNET.Plugins.Attributes;
using MyAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAuth
{

    [Plugin(PluginName = "MyAuth", Description = "Mysql Auth Plugin for MiNET", PluginVersion = "1.0", Author = "haniokasai")]
    public class Class1 : Plugin
    {
        public static Dictionary<string, Boolean> lged = new Dictionary<string, Boolean>();
        Dictionary<string, Boolean> prerg = new Dictionary<string, Boolean>();
        Dictionary<string, int> ct = new Dictionary<string, int>();

        public static ILog _log = LogManager.GetLogger("MyAuth");

        protected override void OnEnable()
        {
            //mysql.load();
            Context.Server.PlayerFactory.PlayerCreated += PlayerFactory_PlayerCreated;
            _log.Warn("Loaded");
            
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

        private string toEn(string value)
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



        }
}
