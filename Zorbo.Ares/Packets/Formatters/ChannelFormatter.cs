using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Packets.Channels;
using Zorbo.Packets.Ares;

namespace Zorbo.Packets.Formatters
{
    public class ChannelFormatter : IFormatter
    {
        UdpSerializer serializer;


        public ChannelFormatter() {
            serializer = new UdpSerializer(this);
        }


        public byte[] Format(IPacket message) {
            return serializer.Serialize(message);
        }


        public IPacket Unformat(byte id, byte[] data) {
            return Unformat(id, data, 0, data.Length);
        }

        public IPacket Unformat(byte id, byte[] data, int index, int count) {

            switch ((UdpId)id) {
                case UdpId.OP_SERVERLIST_ACKINFO:
                    return serializer.Deserialize<AckInfo>(data, index, count);
                case UdpId.OP_SERVERLIST_ACKIPS:
                    return serializer.Deserialize<AckIps>(data, index, count);
                case UdpId.OP_SERVERLIST_ACKNODES:
                    return serializer.Deserialize<AckNodes>(data, index, count);
                case UdpId.OP_SERVERLIST_ADDIPS:
                    return serializer.Deserialize<AddIps>(data, index, count);
                case UdpId.OP_SERVERLIST_CHECKFIREWALLBUSY:
                    return serializer.Deserialize<CheckFirewallBusy>(data, index, count);
                case UdpId.OP_SERVERLIST_PROCEEDCHECKFIREWALL:
                    return serializer.Deserialize<CheckFirewall>(data, index, count);
                case UdpId.OP_SERVERLIST_READYTOCHECKFIREWALL:
                    return serializer.Deserialize<CheckFirewallReady>(data, index, count);
                case UdpId.OP_SERVERLIST_SENDINFO:
                    return serializer.Deserialize<SendInfo>(data, index, count);
                case UdpId.OP_SERVERLIST_SENDNODES:
                    return serializer.Deserialize<SendNodes>(data, index, count);
                case UdpId.OP_SERVERLIST_WANTCHECKFIREWALL:
                    return serializer.Deserialize<CheckFirewallWanted>(data, index, count);
                default:
                    return new Unknown(id, data.Skip(index).Take(count).ToArray());
            }
        }
    }
}
