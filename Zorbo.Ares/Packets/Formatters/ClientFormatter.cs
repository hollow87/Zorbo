using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Packets.Ares;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Formatters
{
    public sealed class ClientFormatter : IFormatter
    {
        PacketSerializer serializer;


        public ClientFormatter() {
            serializer = new PacketSerializer(this);
        }

        public byte[] Format(IPacket message) {
            return serializer.Serialize(message);
        }

        public IPacket Unformat(byte id, byte[] data) {
            return Unformat(id, data, 0, data.Length);
        }

        public IPacket Unformat(byte id, byte[] data, int index, int count) {

            #region " Client Packets "

            switch ((AresId)id) {
                case AresId.MSG_CHAT_CLIENT_LOGIN:
                    return serializer.Deserialize<Login>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_UPDATE_STATUS:
                    return serializer.Deserialize<ClientUpdate>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_PVT:
                    return serializer.Deserialize<Private>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_PUBLIC:
                    return serializer.Deserialize<ClientPublic>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_EMOTE:
                    return serializer.Deserialize<ClientEmote>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_FASTPING:
                    return serializer.Deserialize<ClientFastPing>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_COMMAND:
                    return serializer.Deserialize<Command>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_AUTHREGISTER:
                    return serializer.Deserialize<AuthRegister>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_AUTHLOGIN:
                    return serializer.Deserialize<AuthLogin>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_AUTOLOGIN:
                    return serializer.Deserialize<AutoLogin>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_AVATAR:
                    return serializer.Deserialize<ClientAvatar>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_PERSONAL_MESSAGE:
                    return serializer.Deserialize<ClientPersonal>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_ADDSHARE:
                    return serializer.Deserialize<SharedFile>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_REMSHARE:
                    break;
                case AresId.MSG_CHAT_CLIENT_SEARCH:
                    return serializer.Deserialize<Search>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_BROWSE:
                    return serializer.Deserialize<Browse>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_DUMMY:
                    break;
                case AresId.MSG_CHAT_CLIENT_DIRCHATPUSH:
                    return serializer.Deserialize<ClientDirectPush>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_SEND_SUPERNODES:
                    return serializer.Deserialize<ClientNodes>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_IGNORELIST:
                    return serializer.Deserialize<Ignored>(data, index, count);
                case AresId.MSG_SERVER_TOHUB_LOGINREQ:
                    break;
                case AresId.MSG_CHAT_CLIENT_CUSTOM_DATA:
                    return serializer.Deserialize<ClientCustom>(data, index, count);
                case AresId.MSG_CHAT_CLIENT_CUSTOM_DATA_ALL:
                    return serializer.Deserialize<ClientCustomAll>(data, index, count);
                default: {
                        byte[] tmp = new byte[count];
                        Array.Copy(data, index, tmp, 0, count);

                        return new Unknown(id, tmp);
                    }
            }

            #endregion

            return null;
        }
    }
}
