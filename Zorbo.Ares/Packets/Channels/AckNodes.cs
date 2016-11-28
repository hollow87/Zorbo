using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Channels
{
    sealed class AckNodes : UdpPacket
    {
        public override byte Id {
            get { return (byte)UdpId.OP_SERVERLIST_ACKNODES; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort Port { get; set; }

        [PacketItem(1)]
        public byte[] Nodes { get; set; }
    }
}
