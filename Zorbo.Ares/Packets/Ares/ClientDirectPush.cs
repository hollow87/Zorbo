using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public class ClientDirectPush : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_DIRCHATPUSH; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 20)]
        public string Username {
            get;
            internal set;
        }

        [PacketItem(1)]
        public byte[] TextSync {
            get;
            internal set;
        }
    }
}
