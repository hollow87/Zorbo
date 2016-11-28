using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Channels
{
    sealed class CheckFirewallReady : UdpPacket
    {
        public override byte Id {
            get { return (byte)UdpId.OP_SERVERLIST_READYTOCHECKFIREWALL; }
            protected set { }
        }

        [PacketItem(0)]
        public uint Cookie { get; set; }

        [PacketItem(1)]
        public IPAddress Target { get; set; }


        public CheckFirewallReady() { }

        public CheckFirewallReady(uint cookie, IPAddress ip) {
            Cookie = cookie;
            Target = ip;
        }
    }
}
