using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class ClientPersonal : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_PERSONAL_MESSAGE; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 1024)]
        public string Message { get; set; }
    }
}
