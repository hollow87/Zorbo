using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class ClientAvatar : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_AVATAR; }
            protected set { }
        }

        [PacketItem(0)]
        public byte[] AvatarBytes { get; set; }
    }
}
