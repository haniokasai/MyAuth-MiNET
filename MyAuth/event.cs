using MiNET;
using MiNET.Net;
using MiNET.Plugins.Attributes;
using MiNET.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAuth
{
    public class events :Class1
    {

        protected override void OnEnable()
        {
             _log.Info("Event Handler");
        }

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
