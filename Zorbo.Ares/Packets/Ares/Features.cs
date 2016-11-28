using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class Features : AresPacket  
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_MYFEATURES; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 30)]
        public string Version { get; set; }

        [PacketItem(1)]
        public ServerFeatures SupportFlag { get; set; }

        [PacketItem(2)]
        public byte SharedTypes { get; set; }

        [PacketItem(3)]
        public Language Language { get; set; }

        [PacketItem(4)]
        public uint Cookie { get; set; }

        [PacketItem(5)]
        public bool Avatars {
            get { return true; }
            set { }
        }
    }
}
