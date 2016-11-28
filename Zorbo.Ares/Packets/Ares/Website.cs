using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class Website : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_URL; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 255)]
        public string Address { get; set; }

        [PacketItem(1, MaxLength = 255)]
        public string Caption { get; set; }


        public Website() { }

        public Website(string address, string caption) {
            Address = address;
            Caption = caption;
        }
    }
}
