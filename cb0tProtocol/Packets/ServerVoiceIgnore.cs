using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;
using Zorbo.Packets;

namespace cb0tProtocol.Packets
{
    class ServerVoiceIgnore : AresPacket
    {
        public override byte Id {
            get { return (byte)AdvancedId.MSG_CHAT_SERVER_VC_IGNORE; }
        }

        [PacketItem(0)]
        public string Username { get; set; }
    }
}
