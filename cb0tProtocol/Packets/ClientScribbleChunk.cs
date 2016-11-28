using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo;
using Zorbo.Interface;
using Zorbo.Packets;
using Zorbo.Serialization;

namespace cb0tProtocol.Packets
{
    public class ClientScribbleChunk : AresPacket
    {
        public override byte Id {
            get { return (byte)AdvancedId.MSG_CHAT_CLIENT_ROOM_SCRIBBLE_CHUNK; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 4094)]
        public byte[] Data { get; set; }
    }
}
