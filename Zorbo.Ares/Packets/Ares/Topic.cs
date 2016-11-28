using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public abstract class TopicBase : AresPacket
    {
        public override byte Id {
            get { throw new NotImplementedException(); }
        }

        [PacketItem(0, MaxLength = 1024, NullTerminated = false)]
        public string Message { get; set; }
    }

    public sealed class Topic : TopicBase
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_TOPIC; }
            protected set { }
        }

        public Topic() { }

        public Topic(string topic) {
            Message = topic;
        }
    }

    public sealed class TopicFirst : TopicBase
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_TOPIC_FIRST; }
            protected set { }
        }

        public TopicFirst() { }

        public TopicFirst(string topic) {
            Message = topic;
        }
    }
}
