using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class SearchEnd : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_ENDOFSEARCH; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort SearchId { get; set; }


        public SearchEnd() { }

        public SearchEnd(ushort searchid) {
            SearchId = searchid;
        }
    }
}
