using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class OpChange : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_OPCHANGE; }
            protected set { }
        }

        [PacketItem(0)]
        public bool IsAdmin { get; set; }

        [PacketItem(1)]
        public byte Ignored { get; set; }


        public OpChange() { }

        public OpChange(bool admin) {
            IsAdmin = admin;
        }
    }
}
