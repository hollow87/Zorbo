using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public class Unknown : AresPacket
    {
        public override byte Id {
            get;
            protected set;
        }


        [PacketItem(0)]
        public byte[] Payload {
            get;
            set;
        }

        public Unknown() { }

        public Unknown(byte id, byte[] payload) {
            Id = id;
            Payload = payload;
        }
    }
}
