using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class Offline : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_OFFLINEUSER; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 20)]
        public string Username { get; set; }

        public Offline() { }

        public Offline(string name) { Username = name; }
    }
}
