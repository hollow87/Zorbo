using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class Error : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_ERROR; }
            protected set { }
        }
        
        [PacketItem(0, NullTerminated = false)]
        public string Message { get; set; }


        public Error() { }

        public Error(string message) {
            Message = message;
        }
    }
}
