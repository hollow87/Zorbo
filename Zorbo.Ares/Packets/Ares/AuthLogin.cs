using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class AuthLogin : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_AUTHLOGIN; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 255)]
        public string Password { 
            get; 
            internal set; 
        }

        public AuthLogin() { }
    }
}
