using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class ServerDirectPush : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_DIRCHATPUSH; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 20)]
        public string Username {
            get;
            internal set;
        }

        [PacketItem(1)]
        public IPAddress ExternalIp {
            get;
            internal set;
        }

        [PacketItem(2)]
        public ushort ListenPort {
            get;
            internal set;
        }

        [PacketItem(3)]
        public IPAddress LocalIp {
            get;
            internal set;
        }

        [PacketItem(4)]
        public byte[] TextSync {
            get;
            internal set;
        }


        public ServerDirectPush() { }

        public ServerDirectPush(IClient client, ClientDirectPush push) {
            Username = client.Name;
            ExternalIp = client.ExternalIp;
            ListenPort = client.ListenPort;
            LocalIp = client.LocalIp;
            TextSync = push.TextSync;
        }
    }
}
