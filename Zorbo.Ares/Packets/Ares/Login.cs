using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class Login : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_LOGIN; }
            protected set { }
        }

        [PacketItem(0)]
        public Guid Guid {
            get; 
            internal set;
        }

        [PacketItem(1)]
        public ushort FileCount {
            get;
            internal set;
        }

        [PacketItem(2)]
        public byte Encryption {
            get;
            internal set;
        }

        [PacketItem(3)]
        public ushort ListenPort { 
            get; 
            internal set; 
        }

        [PacketItem(4)]
        public IPAddress NodeIp {
            get;
            internal set;
        }

        [PacketItem(5)]
        public ushort NodePort {
            get;
            internal set;
        }

        [PacketItem(6)]
        public int Skipped2 { 
            get;
            internal set;
        }

        [PacketItem(7, MaxLength = 20)]
        public string Username {
            get;
            internal set;
        }

        [PacketItem(8, MaxLength = 20)]
        public string Version {
            get;
            internal set;
        }

        [PacketItem(9)]
        public IPAddress LocalIp {
            get;
            internal set;
        }

        [PacketItem(10)]
        public IPAddress ExternalIp {
            get;
            internal set;
        }

        [PacketItem(11)]
        public byte SupportFlag {
            get;
            internal set;
        }

        [PacketItem(12, Optional = true)]
        public byte Uploads {
            get;
            internal set;
        }

        [PacketItem(13, Optional = true)]
        public byte MaxUploads {
            get;
            internal set;
        }

        [PacketItem(14, Optional = true)]
        public byte Queued {
            get;
            internal set;
        }

        [PacketItem(15, Optional = true)]
        public byte Age {
            get;
            internal set;
        }

        [PacketItem(16, Optional = true)]
        public Gender Gender {
            get;
            internal set;
        }

        [PacketItem(17, Optional = true)]
        public Country Country {
            get;
            internal set;
        }

        [PacketItem(18, Optional = true, MaxLength = 30)]
        public string Region {
            get;
            internal set;
        }
        
        [PacketItem(19, Optional = true)]
        public ClientFeatures Features {
            get;
            internal set;
        }
        
    }
}
