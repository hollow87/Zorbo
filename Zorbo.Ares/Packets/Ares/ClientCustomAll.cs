using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class ClientCustomAll : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_CUSTOM_DATA_ALL; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 20)]
        public string CustomId {
            get;
            internal set;
        }

        [PacketItem(1)]
        public byte[] Data { get; set; }
    }
}
