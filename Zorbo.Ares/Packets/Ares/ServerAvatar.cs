using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class ServerAvatar : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_AVATAR; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 20)]
        public string Username { get; set; }

        [PacketItem(1)]
        public byte[] AvatarBytes { get; set; }


        public ServerAvatar() { }

        public ServerAvatar(IClient user) {
            Username = user.Name;
            AvatarBytes = user.Avatar.SmallBytes;
        }

        public ServerAvatar(string name, IAvatar avatar) {
            Username = name;
            AvatarBytes = avatar.SmallBytes;
        }
    }
}
