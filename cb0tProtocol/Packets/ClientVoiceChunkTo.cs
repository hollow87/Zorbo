﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;
using Zorbo.Packets;

namespace cb0tProtocol.Packets
{
    public class ClientVoiceChunkTo : AresPacket
    {
        public override byte Id {
            get { return (byte)AdvancedId.MSG_CHAT_CLIENT_VC_CHUNK_TO; }
        }

        [PacketItem(0)]
        public string Username { get; set; }

        [PacketItem(1)]
        public byte[] Chunk { get; set; }
    }
}
