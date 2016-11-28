using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Packets.Ares
{
    public sealed class ServerFastPing : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_FASTPING; }
            protected set { }
        }
    }
}
