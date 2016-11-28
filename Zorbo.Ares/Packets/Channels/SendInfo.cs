using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Channels
{
    sealed class SendInfo : UdpPacket
    {
        public override byte Id {
            get { return (byte)UdpId.OP_SERVERLIST_SENDINFO; }
            protected set { }
        }
    }
}
