using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class ServerHtml : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_HTML; }
            protected set { }
        }

        [PacketItem(0, NullTerminated = false)]
        public string Content { get; set; }

        public ServerHtml() { }

        public ServerHtml(string content) {
            Content = content;
        }
    }
}
