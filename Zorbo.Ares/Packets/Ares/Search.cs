using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class Search : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_SEARCH; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort SearchId {
            get;
            internal set;
        }

        [PacketItem(1)]
        public byte Skipped {
            get;
            internal set;
        }

        [PacketItem(2)]
        public byte Type { 
            get;
            internal set; 
        }

        [PacketItem(3, LengthPrefix = true)]
        public string SearchWords { 
            get; 
            internal set; 
        }
    }
}
