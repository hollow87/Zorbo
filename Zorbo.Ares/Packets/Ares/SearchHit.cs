using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    public sealed class SearchHit : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_SEARCHHIT; }
            protected set { }
        }

        [PacketItem(0)]
        public ushort SearchId { get; set; }

        [PacketItem(1)]
        public byte Type { get; set; }

        [PacketItem(2)]
        public uint Size { get; set; }

        [PacketItem(3)]
        public byte[] Content { get; set; }

        [PacketItem(4, MaxLength = 20)]
        public string Username { get; set; }

        [PacketItem(5)]
        public IPAddress ExternalIp { get; set; }

        [PacketItem(6)]
        public ushort DCPort { get; set; }

        [PacketItem(7)]
        public IPAddress NodeIp { get; set; }

        [PacketItem(8)]
        public ushort NodePort { get; set; }

        [PacketItem(9)]
        public IPAddress LocalIp { get; set; }

        [PacketItem(10)]
        public byte Uploads { get; set; }

        [PacketItem(11)]
        public byte MaxUploads { get; set; }

        [PacketItem(12)]
        public byte Queued { get; set; }

        [PacketItem(13)]
        public byte Skipped {
            get { return 1; }
            set { }
        }


        public SearchHit() { }

        public SearchHit(ushort searchid, IClient user, ISharedFile file) {
            SearchId = searchid;
            Type = file.Type;
            Size = file.Size;
            Content = file.Content;
            Username = user.Name;
            ExternalIp = user.ExternalIp;
            DCPort = user.ListenPort;
            NodeIp = user.NodeIp;
            NodePort = user.NodePort;
            LocalIp = user.LocalIp;
        }
    }
}
