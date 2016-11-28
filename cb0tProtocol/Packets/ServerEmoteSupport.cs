using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;
using Zorbo.Packets;

namespace cb0tProtocol.Packets
{
    class ServerEmoteSupport : AresPacket
    {
        public override byte Id {
            get { return (byte)AdvancedId.MSG_CHAT_SERVER_SUPPORTS_CUSTOM_EMOTES; }
        }

        [PacketItem(0)]
        public byte Flag {
            get;
            set;
        }

        public ServerEmoteSupport() { }

        public ServerEmoteSupport(byte flag) {
            this.Flag = flag;
        }
    }
}
