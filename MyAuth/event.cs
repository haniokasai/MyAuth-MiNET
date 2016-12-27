using MiNET;
using MiNET.Net;
using MiNET.Plugins.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAuth
{
    class @event
    {

        [PacketHandler]
        public Package onChat(McpeText packet,Player player)
        {
            if(Class1.lged.ContainsKey(player.Username.ToLower())){
                player.SendMessage("Please Login", MessageType.Popup);
                return null;
            }
            return packet;
        }


        /////まだまだ追加するよ！
    }
}
