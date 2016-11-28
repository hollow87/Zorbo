using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public class DirectPushError : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_DIRCHATPUSH; }
            protected set { }
        }

        [PacketItem(0)]
        public byte Error { get; set; }


        public DirectPushError() { }

        public DirectPushError(byte code) {
            Error = code;
        }
    }
}
