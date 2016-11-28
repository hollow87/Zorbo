using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class ClientCustom : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_CLIENT_CUSTOM_DATA; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 20)]
        public string CustomId { 
            get;
            set;
        }

        [PacketItem(1, MaxLength = 20)]
        public string Username { 
            get; 
            set;
        }

        [PacketItem(2)]
        public byte[] Data { get; set; }


        public ClientCustom() { }

        public ClientCustom(string name, string ident, byte[] data) {
            CustomId = ident;
            Username = name;
            Data = data;
        }

        public ClientCustom(string name, ClientCustomAll custom) {
            CustomId = custom.CustomId;
            Username = name;
            Data = custom.Data;
        }
    }
}
