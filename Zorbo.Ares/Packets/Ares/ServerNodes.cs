using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class ServerNodes : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_HERE_SUPERNODES; }
            protected set { }
        }

        [PacketItem(0)]
        public byte[] Nodes { get; set; }
    }
}
