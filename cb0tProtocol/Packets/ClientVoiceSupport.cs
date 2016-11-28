using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;
using Zorbo.Packets;

namespace cb0tProtocol.Packets
{
    class ClientVoiceSupport : AresPacket
    {
        public override byte Id {
            get { return (byte)AdvancedId.MSG_CHAT_CLIENT_VC_SUPPORTED; }
        }

        [PacketItem(0)]
        public bool Public { get; set; }

        [PacketItem(1)]
        public bool Private { get; set; }
    }
}
