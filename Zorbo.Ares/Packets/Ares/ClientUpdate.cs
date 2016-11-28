using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;
using Zorbo.Interface;

namespace Zorbo.Packets.Ares
{
    public sealed class ClientUpdate : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_UPDATE_STATUS; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort FileCount {
            get;
            internal set;
        }

        [PacketItem(1)]
        public byte SupportFlag {
            get;
            internal set;
        }

        [PacketItem(2)]
        public IPAddress NodeIp {
            get;
            internal set;
        }

        [PacketItem(3)]
        public ushort NodePort {
            get;
            internal set;
        }

        [PacketItem(4)]
        public IPAddress ExternalIp {
            get;
            internal set;
        }

        [PacketItem(5, Optional = true)]
        public byte Age {
            get;
            internal set;
        }

        [PacketItem(6, Optional = true)]
        public Gender Gender {
            get;
            internal set;
        }

        [PacketItem(7, Optional = true)]
        public Country Country {
            get;
            internal set;
        }

        [PacketItem(8, Optional = true, MaxLength = 30)]
        public string Region {
            get;
            internal set;
        }

        [PacketItem(9, Optional = true)]
        public byte Skipped2 {
            get;
            internal set;
        }
    }
}
