using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Packets.Ares;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Formatters
{
    public sealed class ServerFormatter : IFormatter
    {
        PacketSerializer serializer;


        public ServerFormatter() {
            serializer = new PacketSerializer(this);
        }

        public byte[] Format(IPacket message) {
            return serializer.Serialize(message);
        }

        public IPacket Unformat(byte id, byte[] data) {
            return Unformat(id, data, 0, data.Length);
        }

        public IPacket Unformat(byte id, byte[] data, int index, int count) {

            if ((id == 0 && data == null) || data.Length == 0)
                throw new ArgumentNullException("data", "must contain data");

            #region " Server Packets "

            switch ((AresId)id) {
                case AresId.MSG_CHAT_SERVER_LOGIN_ACK:
                    return serializer.Deserialize<LoginAck>(data, index, count);
                case AresId.MSG_CHAT_SERVER_MYFEATURES:
                    return serializer.Deserialize<Features>(data, index, count);
                case AresId.MSG_CHAT_SERVER_TOPIC:
                    return serializer.Deserialize<Topic>(data, index, count);
                case AresId.MSG_CHAT_SERVER_TOPIC_FIRST:
                    return serializer.Deserialize<TopicFirst>(data, index, count);
                case AresId.MSG_CHAT_SERVER_AVATAR:
                    return serializer.Deserialize<ServerAvatar>(data, index, count);
                case AresId.MSG_CHAT_SERVER_PERSONAL_MESSAGE:
                    return serializer.Deserialize<ServerPersonal>(data, index, count);
                case AresId.MSG_CHAT_SERVER_PUBLIC:
                    return serializer.Deserialize<ServerPublic>(data, index, count);
                case AresId.MSG_CHAT_SERVER_EMOTE:
                    return serializer.Deserialize<ServerEmote>(data, index, count);
                case AresId.MSG_CHAT_SERVER_PVT:
                    return serializer.Deserialize<Private>(data, index, count);
                default: {
                        byte[] tmp = new byte[count];
                        Array.Copy(data, index, tmp, 0, count);

                        return new Unknown(id, tmp);
                    }
            }

            #endregion
        }
    }
}
