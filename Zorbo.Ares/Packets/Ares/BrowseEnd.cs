using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class BrowseEnd : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_ENDOFBROWSE; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort BrowseId { get; set; }


        public BrowseEnd() { }

        public BrowseEnd(ushort browseid) {
            BrowseId = browseid;
        }
    }
}
