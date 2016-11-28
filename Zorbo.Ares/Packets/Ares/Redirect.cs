using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class Redirect : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_REDIRECT; }
            protected set { }
        }

        [PacketItem(0)]
        public IPAddress ExternalIp { get; set; }

        [PacketItem(1)]
        public ushort Port { get; set; }

        [PacketItem(2)]
        public IPAddress LocalIp { get; set; }

        [PacketItem(3, MaxLength = 20)]
        public string Name { get; set; }

        [PacketItem(4, MaxLength = 255)]
        public string Message { get; set; }


        public Redirect() { }

        public Redirect(Zorbo.Hashlinks.Channel hash, String message) {
            Name = hash.Name;
            Port = hash.Port;
            LocalIp = hash.LocalIp;
            ExternalIp = hash.ExternalIp;
            Message = message;
        }
    }
}
