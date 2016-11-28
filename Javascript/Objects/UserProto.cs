using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo;
using Zorbo.Interface;
using Zorbo.Data;

namespace Javascript.Objects
{
    public class UserProto : IClient
    {
        public ushort Id {
            get { return ushort.MaxValue; }
        }

        public Guid Guid {
            get { return Guid.Empty; }
        }

        public IServer Server {
            get { return null; }
        }

        public IMonitor Monitor {
            get { return null; }
        }

        public IClientId ClientId {
            get { return null; }
        }

        public AdminLevel Admin {
            get { return AdminLevel.User; }
            set { }
        }

        public uint Cookie {
            get { return (uint)base.GetHashCode(); }
        }

        public bool LoggedIn {
            get { return false; }
        }

        public bool Connected {
            get { return false; }
        }

        public bool LocalHost {
            get { return true; }
        }

        public bool Browsable {
            get { return false; }
        }

        public bool Muzzled {
            get { return false; }
            set { }
        }

        public bool Cloaked {
            get { return false; }
            set { }
        }

        public bool IsCaptcha {
            get { return false; }
            set { }
        }

        public bool FastPing {
            get { return false; }
        }

        public bool Compression {
            get { return false; }
        }

        public bool Encryption {
            get { return false; }
        }

        public byte Age {
            get { return 0; }
        }

        public Gender Gender {
            get { return Gender.Unknown; }
        }

        public Country Country {
            get { return Country.Unknown; }
        }

        public IAvatar Avatar {
            get { return null; }
            set { }
        }

        public IAvatar OrgAvatar {
            get { return null; }
        }

        public string Name {
            get { return null; }
            set { }
        }

        public string OrgName {
            get { return null; }
        }

        public string Version {
            get { return null; }
        }

        public string Region {
            get { return null; }
        }

        public string Message {
            get { return null; }
            set { }
        }

        public ushort Vroom {
            get { return 0; }
            set { }
        }

        public ushort FileCount {
            get { return 0; }
        }

        public ushort NodePort {
            get { return 0; }
        }

        public ushort ListenPort {
            get { return 0; }
        }

        public System.Net.IPAddress NodeIp {
            get { return null; }
        }

        public System.Net.IPAddress LocalIp {
            get { return null; }
        }

        public System.Net.IPAddress ExternalIp {
            get { return null; }
        }

        public System.Net.IPHostEntry DnsEntry {
            get { return null; }
        }

        public ClientFeatures Features {
            get { return ClientFeatures.NONE; }
            set { }
        }

        public IList<string> Ignored {
            get { return null; }
        }

        public IReadOnlyList<ISharedFile> Files {
            get { return null; }
        }

        public IDictionary<string, object> Extended {
            get { return null; }
        }

        public void SendPacket(IPacket packet) { }

        public void SendPacket(Predicate<IClient> match, IPacket packet) { }

        public void Ban() { }

        public void Ban(object state) { }

        public void Disconnect() { }

        public void Disconnect(object state) { }


        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged {
            add { }
            remove { }
        }
    }
}
