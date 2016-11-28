using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

using Zorbo.Packets;
using Zorbo.Packets.Ares;

using cb0tProtocol.Packets;

namespace cb0tProtocol
{
    class AdvancedFormatter : IFormatter
    {
        PacketSerializer serializer;

        public AdvancedFormatter() {
            serializer = new PacketSerializer(this);
        }

        public byte[] Format(IPacket message) {
            return serializer.Serialize(message);
        }


        public IPacket Unformat(byte id, byte[] data) {
            return Unformat(id, data, 0, data.Length);
        }

        public IPacket Unformat(byte id, byte[] data, int index, int count) {

            switch ((AdvancedId)id) {
                case AdvancedId.MSG_CHAT_CLIENT_CUSTOM_ADD_TAGS:
                    return serializer.Deserialize<ClientAddTags>(data, index, count);
                case AdvancedId.MSG_CHAT_CLIENT_CUSTOM_REM_TAGS:
                    return serializer.Deserialize<ClientRemTags>(data, index, count);
                case AdvancedId.MSG_CHAT_CLIENT_CUSTOM_FONT:
                    return serializer.Deserialize<ClientFont>(data, index, count);
                case AdvancedId.MSG_CHAT_CLIENT_VC_SUPPORTED:
                    return serializer.Deserialize<ClientVoiceSupport>(data, index, count);
                case AdvancedId.MSG_CHAT_CLIENT_VC_FIRST:
                    return serializer.Deserialize<ClientVoiceFirst>(data, index, count);
                case AdvancedId.MSG_CHAT_CLIENT_VC_FIRST_TO:
                    return serializer.Deserialize<ClientVoiceFirstTo>(data, index, count);
                case AdvancedId.MSG_CHAT_CLIENT_VC_CHUNK:
                    return serializer.Deserialize<ClientVoiceChunk>(data, index, count);
                case AdvancedId.MSG_CHAT_CLIENT_VC_CHUNK_TO:
                    return serializer.Deserialize<ClientVoiceChunkTo>(data, index, count);
                case AdvancedId.MSG_CHAT_CLIENT_VC_IGNORE:
                    return serializer.Deserialize<ClientVoiceIgnore>(data, index, count);
                case AdvancedId.MSG_CHAT_CLIENT_SUPPORTS_CUSTOM_EMOTES:
                    return serializer.Deserialize<ClientEmoteSupport>(data, index, count);
                case AdvancedId.MSG_CHAT_SERVER_CUSTOM_EMOTES_ITEM:
                    return serializer.Deserialize<ClientEmoteItem>(data, index, count);
                case AdvancedId.MSG_CHAT_CLIENT_CUSTOM_EMOTE_DELETE:
                    return serializer.Deserialize<ClientEmoteDelete>(data, index, count);
                case AdvancedId.MSG_CHAT_CLIENT_ROOM_SCRIBBLE_FIRST:
                    return serializer.Deserialize<ClientScribbleFirst>(data, index, count);
                case AdvancedId.MSG_CHAT_CLIENT_ROOM_SCRIBBLE_CHUNK:
                    return serializer.Deserialize<ClientScribbleChunk>(data, index, count);
                default: {
                        byte[] tmp = new byte[count];
                        Array.Copy(data, index, tmp, 0, count);

                        return new Unknown(id, tmp);
                    }
            }
        }
    }
}
