using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class BrowseItem : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_BROWSEITEM; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort BrowseId { get; set; }

        [PacketItem(1)]
        public byte Type { get; set; }

        [PacketItem(2)]
        public uint Size { get; set; }

        [PacketItem(3)]
        public byte[] Content { get; set; }


        public BrowseItem() { }

        public BrowseItem(ushort browseid, ISharedFile file) {
            BrowseId = browseid;
            Type = file.Type;
            Size = file.Size;
            Content = file.Content;
        }
    }
}
