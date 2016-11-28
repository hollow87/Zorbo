using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class LoginAck : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_LOGIN_ACK; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 20)]
        public string Username {
            get;
            set;
        }

        [PacketItem(1, MaxLength = 20)]
        public string ServerName {
            get;
            set;
        }
    }
}
