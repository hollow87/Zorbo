using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class BrowseStart : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_STARTOFBROWSE; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort BrowseId { get; set; }

        [PacketItem(1)]
        public ushort FileCount { get; set; }


        public BrowseStart() { }

        public BrowseStart(ushort browseid, ushort filecount) {
            BrowseId = browseid;
            FileCount = filecount;
        }
    }
}
