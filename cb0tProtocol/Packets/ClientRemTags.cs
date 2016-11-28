﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;
using Zorbo.Packets;

namespace cb0tProtocol.Packets
{
    class ClientRemTags : AresPacket
    {
        public override byte Id {
            get { return (byte)AdvancedId.MSG_CHAT_CLIENT_CUSTOM_REM_TAGS; }
        }

        [PacketItem(0)]
        public string Tag { get; set; }
    }
}
