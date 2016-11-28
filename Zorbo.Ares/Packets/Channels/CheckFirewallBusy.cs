using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Channels
{
    sealed class CheckFirewallBusy : UdpPacket
    {
        public override byte Id {
            get { return (byte)UdpId.OP_SERVERLIST_CHECKFIREWALLBUSY; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort Port { get; set; }

        [PacketItem(1)]
        public byte[] Servers { get; set; }
    }
}
