using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class Browse : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_BROWSE; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort BrowseId {
            get;
            internal set;
        }

        [PacketItem(1)]
        public byte Type {
            get;
            internal set;
        }

        [PacketItem(2, MaxLength = 20)]
        public string Username {
            get;
            internal set;
        }
    }
}
