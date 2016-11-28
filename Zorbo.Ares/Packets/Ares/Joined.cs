using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets.Ares
{
    #region " JoinBase "

    public abstract class JoinBase : AresPacket
    {
        public override byte Id {
            get { throw new NotImplementedException(); }
        }

        [PacketItem(0)]
        public ushort FileCount { get; set; }

        [PacketItem(1)]
        public int Skipped1 { get; set; }
        
        [PacketItem(2)]
        public IPAddress ExternalIp { get; set; }

        [PacketItem(3)]
        public ushort DCPort { get; set; }

        [PacketItem(4)]
        public IPAddress NodeIp { get; set; }

        [PacketItem(5)]
        public ushort NodePort { get; set; }

        [PacketItem(6)]
        public byte Skipped2 { get; set; }

        [PacketItem(7, MaxLength = 20)]
        public string Username { get; set; }

        [PacketItem(8)]
        public IPAddress LocalIp { get; set; }

        [PacketItem(9)]
        public bool Browsable { get; set; }

        [PacketItem(10)]
        public AdminLevel Level { get; set; }

        [PacketItem(11)]
        public byte Age { get; set; }

        [PacketItem(12)]
        public Gender Gender { get; set; }

        [PacketItem(13)]
        public Country Country { get; set; }

        [PacketItem(14, MaxLength = 30)]
        public string Region { get; set; }

        [PacketItem(15)]
        public ClientFeatures Features { get; set; }

        public JoinBase() { }

        public JoinBase(IClient user) {
            FileCount = user.FileCount;
            ExternalIp = user.Server.Config.HideIPs ? IPAddress.Any : user.ExternalIp;
            DCPort = user.ListenPort;
            NodeIp = user.NodeIp;
            NodePort = user.NodePort;
            Username = user.Name;
            LocalIp = user.Server.Config.HideIPs ? IPAddress.Any : user.LocalIp;
            Browsable = user.Browsable;
            Level = user.Admin;
            Age = user.Age;
            Gender = user.Gender;
            Country = user.Country;
            Region = user.Region;
            Features = user.Features;
        }
    }

    #endregion

    public sealed class Join : JoinBase
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_JOIN; }
            protected set { }
        }

        public Join() { }

        public Join(IClient user) : base(user) { }
    }

    public sealed class Userlist : JoinBase
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_CHANNEL_USER_LIST; }
            protected set { }
        }

        public Userlist() { }

        public Userlist(IClient user) : base(user) { }
    }

    public sealed class UserlistEnd : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_CHANNEL_USER_LIST_END; }
            protected set { }
        }

        [PacketItem(0)]
        public byte Null {
            get { return 0; }
            set { }
        }
    }

    public sealed class UserlistClear : AresPacket
    {
        public override byte Id {
            get { return (byte)AresId.MSG_CHAT_SERVER_CHANNEL_USERLIST_CLEAR; }
            protected set { }
        }

        [PacketItem(0)]
        public byte Null {
            get { return 0; }
            set { }
        }
    }
}
