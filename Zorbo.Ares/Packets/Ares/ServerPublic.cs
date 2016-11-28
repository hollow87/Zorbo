using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class ServerPublic : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_PUBLIC; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 20)]
        public string Username { get; set; }

        [PacketItem(1, MaxLength = 1024, NullTerminated = false)]
        public string Message { get; set; }


        public ServerPublic() { }

        public ServerPublic(string username, string text) {
            Username = username;
            Message = text;
        }
    }
}
