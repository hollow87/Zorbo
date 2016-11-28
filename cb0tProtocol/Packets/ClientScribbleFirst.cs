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
    public class ClientScribbleFirst : AresPacket
    {
        public override byte Id {
            get { return (byte)AdvancedId.MSG_CHAT_CLIENT_ROOM_SCRIBBLE_FIRST; }
            protected set { }
        }

        [PacketItem(0)]
        public uint Size { get; set; }

        [PacketItem(1)]
        public ushort Chunks { get; set; }

        [PacketItem(2, MaxLength = 4090)]
        public byte[] Data { get; set; }
    }
}
