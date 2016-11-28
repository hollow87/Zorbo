using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Channels
{
    sealed class AckInfo : UdpPacket
    {
        public override byte Id {
            get { return (byte)UdpId.OP_SERVERLIST_ACKINFO; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort Port { get; set; }

        [PacketItem(1)]
        public ushort Users { get; set; }

        [PacketItem(2, LengthPrefix = true)]
        public string Name { get; set; }

        [PacketItem(3, LengthPrefix = true)]
        public string Topic { get; set; }

        [PacketItem(4)]
        public Language Language { get; set; }

        [PacketItem(5, LengthPrefix = true)]
        public string Version { get; set; }

        [PacketItem(6)]
        public byte ServersLen { get; set; }

        [PacketItem(7)]
        public byte[] Servers { get; set; }
    }
}
