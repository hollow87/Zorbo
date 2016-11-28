using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;
using Zorbo.Packets;

namespace cb0tProtocol.Packets
{
    public class Advanced : AresPacket
    {
        public override byte Id {
            get { return (byte)AdvancedId.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL; }
        }

        [PacketItem(0)]
        public IPacket Payload {
            get;
            internal set;
        }

        public Advanced() { }

        public Advanced(IPacket packet) { Payload = packet; }
    }
}
