using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;
using Zorbo.Packets;

namespace cb0tProtocol.Packets
{
    public class ClientEmoteItem : AresPacket
    {
        public override byte Id {
            get { return (byte)AdvancedId.MSG_CHAT_CLIENT_CUSTOM_EMOTES_UPLOAD_ITEM; }
        }

        [PacketItem(0)]
        public string Shortcut { get; set; }

        [PacketItem(1)]
        public byte Size { get; set; }

        [PacketItem(2)]
        public byte[] Image { get; set; }
    }
}
