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
            if (player == null) throw new ArgumentNullException(nameof(e.Player));
            player.Level.BroadcastMessage("Bye");

        }


        private void Player_PlayerJoin(object sender, PlayerEventArgs e)
        {
            Player player = e.Player;
            player.Level.BroadcastMessage("Welcome");
            player.SetGameMode(MiNET.Worlds.GameMode.Creative);
            /*var setCmdEnabled = McpeSetCommandsEnabled.CreateObject();
            setCmdEnabled.enabled = true;
            player.SendPackage(setCmdEnabled);
            PluginManager pm = new PluginManager();
            pm.HandleCommand(null, "help", "default",null);*/
        }



    }
}
