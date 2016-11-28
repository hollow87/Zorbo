using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class SharedFile : AresPacket, ISharedFile
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_ADDSHARE; }
            protected set { }
        }

        [PacketItem(0)]
        public byte Type {
            get;
            internal set;
        }

        [PacketItem(1)]
        public uint Size {
            get;
            internal set;
        }
        
        [PacketItem(2, LengthPrefix = true)]
        public string SearchWords {
            get;
            internal set;
        }

        [PacketItem(3)]
        public byte[] Content {
            get;
            internal set;
        }
    }
}
