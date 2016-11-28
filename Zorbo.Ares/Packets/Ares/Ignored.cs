using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class Ignored : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_IGNORELIST; }
            protected set { }
        }

        [PacketItem(0)]
        public bool Ignore { get; set; }

        [PacketItem(1)]
        public string Username { get; set; }
    }
}
