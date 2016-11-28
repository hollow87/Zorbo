using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Channels
{
    sealed class CheckFirewallWanted : UdpPacket
    {
        public override byte Id {
            get { return (byte)UdpId.OP_SERVERLIST_WANTCHECKFIREWALL; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort Port { get; set; }


        public CheckFirewallWanted() { }

        public CheckFirewallWanted(ushort port) {
            Port = port;
        }
    }
}
