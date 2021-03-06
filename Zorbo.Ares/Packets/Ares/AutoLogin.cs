﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class AutoLogin : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_AUTOLOGIN; }
            protected set { }
        }

        [PacketItem(0)]
        public byte[] Sha1Password { get; set; }
    }
}
