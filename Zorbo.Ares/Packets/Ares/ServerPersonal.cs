using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class ServerPersonal : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_PERSONAL_MESSAGE; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 20)]
        public string Username { get; set; }

        [PacketItem(1, MaxLength = 255, NullTerminated = false)]
        public string Message { get; set; }


        public ServerPersonal() { }

        public ServerPersonal(string name, string text) {
            Username = name;
            Message = text;
        }
    }
}
