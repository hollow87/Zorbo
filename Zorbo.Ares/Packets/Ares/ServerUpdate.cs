using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class ServerUpdate : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_UPDATE_USER_STATUS; }
            protected set { }
        }

        [PacketItem(0, MaxLength = 20)]
        public string Username { get; set; }

        [PacketItem(1)]
        public ushort FileCount { get; set; }

        [PacketItem(2)]
        public bool Browsable { get; set; }

        [PacketItem(3)]
        public IPAddress NodeIp { get; set; }

        [PacketItem(4)]
        public ushort NodePort { get; set; }

        [PacketItem(5)]
        public IPAddress ExternalIp { get; set; }

        [PacketItem(6)]
        public byte Level { get; set; }

        [PacketItem(7)]
        public byte Age { get; set; }

        [PacketItem(8)]
        public Gender Gender { get; set; }

        [PacketItem(9)]
        public Country Country { get; set; }

        [PacketItem(10, MaxLength = 30)]
        public string Region { get; set; }


        public ServerUpdate() {

        }

        public ServerUpdate(IClient client) {
            Username = client.Name;
            FileCount = client.FileCount;
            Browsable = client.Browsable;
            NodeIp = client.NodeIp;
            NodePort = client.NodePort;
            ExternalIp = client.ExternalIp;
            Level = (byte)client.Admin;
            Age = client.Age;
            Gender = client.Gender;
            Country = client.Country;
            Region = client.Region;
        }
    }
}
