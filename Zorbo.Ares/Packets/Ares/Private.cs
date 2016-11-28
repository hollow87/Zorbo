using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class Private : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_PVT; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 20)]
        public string Username { 
            get;
            internal set;
        }

        [PacketItem(1, MaxLength = 1024, NullTerminated = false)]
        public string Message { get; set; }


        public Private() { }

        public Private(string username, string message) {
            Username = username;
            Message = message;
        }
    }
}
