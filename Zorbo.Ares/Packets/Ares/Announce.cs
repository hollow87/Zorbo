using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class Announce : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_NOSUCH; }
            protected set { }
        }

        [PacketItem(0, NullTerminated = false, MaxLength = 1024)]
        public string Message { get; set; }


        public Announce() { }

        public Announce(string text) { 
            Message = text;
        }
    }
}
