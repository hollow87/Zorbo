﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class ClientPublic : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_PUBLIC; }
            protected set { }
        }
        
        [PacketItem(0, MaxLength = 1024)]
        public string Message { get; set; }
    }
}
