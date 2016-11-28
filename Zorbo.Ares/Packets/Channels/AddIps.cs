using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Channels
{
    sealed class AddIps : UdpPacket
    {
        public override byte Id {
            get { return (byte)UdpId.OP_SERVERLIST_ADDIPS; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort Port { get; set; }

        [PacketItem(1)]
        public byte[] Servers { get; set; }


        public AddIps() { }

        public AddIps(ushort port, byte[] servers) {
            Port = port;
            Servers = servers;
        }
    }
}
