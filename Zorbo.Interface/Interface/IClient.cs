using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Zorbo.Data;

namespace Zorbo.Interface
{
    public interface IClient : INotifyPropertyChanged
    {

        // removed to prevent object disposed exceptions on ISocket.SendAsync
        // Suspected race condition on disposed clients
        // use IClient.SendPacket or IServer.SendPacket instead
        //ISocket Socket { get; }

        IServer Server { get; }
        IMonitor Monitor { get; }

        ushort Id { get; }
        Guid Guid { get; }

        uint Cookie { get; }

        IClientId ClientId { get; }
        AdminLevel Admin { get; set; }

        bool LoggedIn { get; }
        bool Connected { get; }
        bool LocalHost { get; }
        bool Browsable { get; }
        bool Muzzled { get; set; }
        bool Cloaked { get; set; }
        bool IsCaptcha { get; set; }
        bool FastPing { get; }
        bool Encryption { get; }
        bool Compression { get; }

        byte Age { get; }
        Gender Gender { get; }
        Country Country { get; }

        IAvatar Avatar { get; set; }
        IAvatar OrgAvatar { get; }

        string Name { get; set; }
        string OrgName { get; }
        string Version { get; }
        string Region { get; }
        string Message { get; set; }

        ushort Vroom { get; set; }
        ushort FileCount { get; }
        ushort NodePort { get; }
        ushort ListenPort { get; }

        IPAddress NodeIp { get; }
        IPAddress LocalIp { get; }
        IPAddress ExternalIp { get; }

        IPHostEntry DnsEntry { get; }
        ClientFeatures Features { get; set; }

        IList<String> Ignored { get; }
        IReadOnlyList<ISharedFile> Files { get; }

        IDictionary<String, Object> Extended { get; }

        void SendPacket(IPacket packet);
        void SendPacket(Predicate<IClient> match, IPacket packet);

        void Ban();
        void Ban(object state);

        void Disconnect();
        void Disconnect(object state);
    }
}
