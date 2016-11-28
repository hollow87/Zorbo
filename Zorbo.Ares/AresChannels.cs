using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using System.Xml.Linq;

using Zorbo.Data;
using Zorbo.Sockets;
using Zorbo.Resources;
using Zorbo.Interface;
using Zorbo.Serialization;

using Zorbo.Packets;
using Zorbo.Packets.Channels;
using Zorbo.Packets.Formatters;

namespace Zorbo
{
    public class AresChannels : NotifyObject, IChannelList
    {
        IServer server;

        Timer timer;
        TimeSpan ticklength;

        DateTime lastpurge;
        DateTime lastexpire;

        AresUdpSocket socket;

        List<IPAddress> banned;
        List<FirewallTest> firewalltests;
        List<ServerRecord> myfirewalltests;

        ObservableCollection<IServerRecord> servers;
        ReadOnlyList<IServerRecord> pub_servers;

        Dictionary<IPAddress, FloodCounter> counters;

        uint ackips = 0;
        uint ackinfo = 0;
        uint requests = 0;
        uint sendnodes = 0;
        uint checkfire = 0;

        bool firewall = false;
        bool firewallTest = false;

        volatile bool running = false;

        public IMonitor Monitor {
            get {  return (socket != null) ? socket.Monitor : null; }
        }

        public uint AckIpHits {
            get { return ackips; }
            set {
                if (ackips != value) {
                    ackips = value;
                    RaisePropertyChanged(() => AckIpHits);
                }
            }
        }

        public uint AckInfoHits {
            get { return ackinfo; }
            set {
                if (ackinfo != value) {
                    ackinfo = value;
                    RaisePropertyChanged(() => AckInfoHits);
                }
            }
        }

        public uint SendInfoHits {
            get { return requests; }
            set {
                if (requests != value) {
                    requests = value;
                    RaisePropertyChanged(() => SendInfoHits);
                }
            }
        }

        public uint SendNodeHits {
            get { return sendnodes; }
            set {
                if (sendnodes != value) {
                    sendnodes = value;
                    RaisePropertyChanged(() => SendNodeHits);
                }
            }
        }

        public uint CheckFirewallHits {
            get { return checkfire; }
            set {
                if (checkfire != value) {
                    checkfire = value;
                    RaisePropertyChanged(() => CheckFirewallHits);
                }
            }
        }

        public bool Listing {
            get { return server.Config.ShowChannel; }
            set {
                if (server.Config.ShowChannel != value) {
                    server.Config.ShowChannel = value;
                    RaisePropertyChanged(() => Listing);
                }
            }
        }

        public bool FirewallOpen {
            get { return firewall; }
            private set {
                if (firewall != value) {
                    firewall = value;
                    RaisePropertyChanged(() => FirewallOpen);
                }
            }
        }

        public bool FinishedTestingFirewall {
            get { return firewallTest; }
            private set { 
                firewallTest = value;
                RaisePropertyChanged(() => FinishedTestingFirewall);
            }
        }


        public IReadOnlyList<IServerRecord> Servers {
            get { return pub_servers; }
        }


        #region " Nested Classes "

        public class ServerRecord : NotifyObject, IServerRecord
        {
            uint ackcount = 0;
            uint trycount = 0;
            ushort port = 0;

            DateTime asked;
            DateTime lastAck;
            DateTime lastTry;

            IPAddress address;
            
            public ushort Port {
                get { return port; }
                set {
                    if (port != value) {
                        port = value;
                        RaisePropertyChanged(() => Port);
                    }
                }
            }

            public IPAddress ExternalIp {
                get { return address; }
                set {
                    if (address == null || !address.Equals(value)) {
                        address = value;
                        RaisePropertyChanged(() => ExternalIp);
                    }
                }
            }

            public uint AckCount {
                get { return ackcount; }
                set {
                    if (ackcount != value) {
                        ackcount = value;
                        RaisePropertyChanged(() => AckCount);
                    }
                }
            }

            public uint TryCount {
                get { return trycount; }
                set {
                    if (trycount != value) {
                        trycount = value;
                        RaisePropertyChanged(() => TryCount);
                    }
                }
            }

            public DateTime LastAcked {
                get { return lastAck; }
                set {
                    if (lastAck != value) {
                        lastAck = value;
                        RaisePropertyChanged(() => LastAcked);
                    }
                }
            }

            public DateTime LastSendIPs {
                get { return lastTry; }
                set {
                    if (lastTry != value) {
                        lastTry = value;
                        RaisePropertyChanged(() => LastSendIPs);
                    }
                }
            }

            public DateTime LastAskedFirewall {
                get { return asked; }
                set {
                    if (asked != value) {
                        asked = value;
                        RaisePropertyChanged(() => LastAskedFirewall);
                    }
                }
            }

            /// <summary>
            /// constructor used by xml serializer
            /// </summary>
            public ServerRecord() {
                DateTime now = TimeBank.CurrentTime;

                this.lastAck = now.Subtract(TimeSpan.FromMinutes(15));
                this.lastTry = now.Subtract(TimeSpan.FromMinutes(1));
            }

            public ServerRecord(uint ip, ushort port)
                : this(ip.ToIPAddress(), port) { }

            public ServerRecord(IPAddress ip, ushort port) 
                : this(ip, port, 0) { }

            public ServerRecord(IPAddress ip, ushort port, uint ackcount) {
                this.address = ip;
                this.port = port;
                this.ackcount = ackcount;

                DateTime now = TimeBank.CurrentTime;

                this.lastAck = now.Subtract(TimeSpan.FromMinutes(15));
                this.lastTry = now.Subtract(TimeSpan.FromMinutes(1));
            }

            internal void AckReceived() {
                ackcount++;
                trycount = 0;
                lastAck = TimeBank.CurrentTime;
            }

            internal void TryingIPs() {
                trycount++;
                lastTry = TimeBank.CurrentTime;
            }
        }

        internal class FirewallTest 
        {
            uint cookie;
            Socket socket;

            bool succeeded;


            public Boolean Succeeded {
                get { return succeeded; }
                private set { succeeded = value; }
            }

            public IPEndPoint RemoteEndPoint {
                get;
                private set;
            }

            public FirewallTest(IPEndPoint remoteEp) {
                RemoteEndPoint = remoteEp;
            }

            public FirewallTest(IPAddress address, ushort port)
                : this(new IPEndPoint(address, port)) { }


            public void Begin() {
                socket = SocketManager.CreateTcp(AddressFamily.InterNetwork);

                SocketConnectTask task = new SocketConnectTask(RemoteEndPoint);
                task.Completed += TestComplete;

                socket.QueueConnect(task);
            }

            private void TestComplete(object sender, IOTaskCompleteEventArgs<SocketConnectTask> e) {
                e.Task.Completed -= TestComplete;

                if (e.Task.Exception == null)
                    Succeeded = e.Task.Socket.Connected;

                var complete = Complete;
                if (complete != null) complete(this, EventArgs.Empty);

                socket.Destroy();
            }


            public override int GetHashCode() {
                if (cookie == 0)
                    cookie = (uint)base.GetHashCode();

                return (int)cookie;
            }

            public event EventHandler Complete;
        }

        #endregion


        public AresChannels(IServer server) {
            this.server = server;
            
            this.timer = new Timer(new TimerCallback(OnTimer), null, -1, -1);
            this.ticklength = TimeSpan.FromSeconds(1);

            this.banned = new List<IPAddress>();
            this.myfirewalltests = new List<ServerRecord>();
            this.firewalltests = new List<FirewallTest>();

            this.servers = new ObservableCollection<IServerRecord>();
            this.pub_servers = new ReadOnlyList<IServerRecord>(servers);

            this.counters = new Dictionary<IPAddress, FloodCounter>();
        }


        void StartSocket() {
            socket = new AresUdpSocket(new ChannelFormatter());

            socket.Exception += SocketException;
            socket.PacketReceived += PacketReceived;

            socket.Bind(new IPEndPoint(IPAddress.Any, server.Config.Port));

            timer.Change(TimeSpan.Zero, ticklength);

            socket.ReceiveAsync();
        }


        public void Start() {
            LoadCache();

            if (servers.Count > 0)
                ticklength = TimeSpan.FromSeconds(1.5);

            running = true;
            StartSocket();
        }

        public void Stop() {
            running = false;
            timer.Change(-1, -1);

            socket.Dispose();
            socket = null;

            FirewallOpen = false;
            FinishedTestingFirewall = false;

            SaveCache();
        }


        private void OnTimer(object state) {

            if (running) {

                if (!FinishedTestingFirewall)
                    CheckFirewall();

                else if (server.Config.ShowChannel) {
                    ServerRecord tosend = GetOldestContacted();

                    if (tosend != null) {
                        uint num = 6;

                        socket.SendAsync(
                            new AddIps(server.Config.Port, GetSendServers(tosend.ExternalIp, ref num)),
                            new IPEndPoint(tosend.ExternalIp, tosend.Port));

                    }

                    DateTime now = TimeBank.CurrentTime;

                    if (now.Subtract(lastexpire).TotalMinutes >= 1) {
                        lastexpire = now;
                        PurgeOld();
                    }

                    if (now.Subtract(lastpurge).TotalMinutes >= 10) {
                        lastpurge = now;
                        PurgeExceeding();
                    }
                }
            }
        }

        private void CheckFirewall() {
            ServerRecord server = null;
            DateTime now = TimeBank.CurrentTime;

            lock (servers)
                server = (ServerRecord)servers.Find((s) => now.Subtract(s.LastAskedFirewall).TotalMinutes > 5);

            if (server != null) {

                server.LastAskedFirewall = now;
                IPAddress ip = server.ExternalIp;

                if (!ip.IsLocal()) {
                    lock (myfirewalltests) myfirewalltests.Add(server);

                    socket.SendAsync(
                        new CheckFirewallWanted(server.Port),
                        new IPEndPoint(ip, server.Port));
                }
            }
            else FinishedTestingFirewall = true;
        }

        private bool CheckFloodCounter(IPAddress address) {

            FloodCounter counter = null;
            DateTime now = TimeBank.CurrentTime;

            TimeSpan span = TimeSpan.FromMinutes(5);

            if (!counters.TryGetValue(address, out counter)) {
                counter = new FloodCounter(0, now);
                counters.Add(address, counter);
            }

            if (now.Subtract(counter.Last) > span)
                counter.Count = 0;

            else if (++counter.Count >= 10) {

                Logging.WriteLines(new[] {
                        "----------",
                        String.Format("UDP Flood Detected From: {0} ({1})", address, counter.Count),
                        "----------"
                    });

                //BAN SERVER
                //lock (banned) banned.Add(address);
            }
            else counter.Last = now;

            return true;
        }

        private void PurgeOld() {
            DateTime now = TimeBank.CurrentTime;

            lock (servers)
                servers.RemoveAll((s) => 
                    s.TryCount > 4 && 
                    now.Subtract(s.LastSendIPs).TotalMinutes >= 1 && 
                    now.Subtract(s.LastAcked).TotalHours >= 1);
        }

        private void PurgeExceeding() {

            if (servers.Count < 1500) 
                return;

            lock (servers) {
                servers.Sort((a, b) => (int)(b.AckCount - a.AckCount));

                while (servers.Count > 1200)
                    servers.RemoveAt(servers.Count - 1);
            }
        }


        private void ParseServers(byte[] input) {

            using (var reader = new PacketReader(input)) {
                lock (servers) {
                    while (reader.Remaining >= 6) {

                        var ip = reader.ReadIPAddress();
                        ushort port = reader.ReadUInt16();

                        if (!CheckDuplicate(ip))
                            servers.Add(new ServerRecord(ip, port));
                    }
                }
            }
        }

        private bool CheckDuplicate(IPAddress address) {

            if (CheckBanned(address))
                return true;

            foreach (var server in servers) {
                if (server.ExternalIp.Equals(address))
                    return true;
            }

            return false;
        }

        private bool CheckBanned(IPAddress ip) {

            lock (banned)
                if (banned.Contains((s) => s.Equals(ip)))
                    return true;

            if (!this.server.Config.UseBansToBanServers)
                return false;

            return server.History.Bans.Contains((s) => s.ExternalIp.Equals(ip)) ||
                   server.History.RangeBans.Contains((s) => s.IsMatch(ip.ToString()));
        }


        private ServerRecord GetOldestContacted() {
            ServerRecord oldest = null;

            if (servers.Count == 0)
                return oldest;

            lock (servers) {
                DateTime now = TimeBank.CurrentTime;

                foreach (var server in servers) {
                    if (now.Subtract(server.LastSendIPs).TotalMinutes >= 14)
                        oldest = (ServerRecord)((oldest == null) ? server : (server.LastSendIPs > oldest.LastSendIPs) ? server : oldest);
                }
            }

            if (oldest != null)
                oldest.TryingIPs();

            return oldest;
        }

        private byte[] GetSendServers(IPAddress ip, ref uint count) {
            uint num = 0;
            using (var writer = new PacketWriter()) {
                lock (servers) {
                    DateTime now = TimeBank.CurrentTime;

                    foreach (var s in servers) {

                        if (s.ExternalIp.Equals(ip) &&
                            s.AckCount > 0 &&
                            now.Subtract(s.LastAcked).TotalMinutes < 15) {

                            num++;

                            writer.Write(s.ExternalIp);
                            writer.Write(s.Port);
                        }

                        if (num == count) break;
                    }

                    count = num;
                    return writer.ToArray();
                }
            }
        }


        internal bool IsCheckingMyFirewall(IPEndPoint endpoint) {
            if (!FirewallOpen) {
                lock (myfirewalltests) {

                    FinishedTestingFirewall = myfirewalltests.Contains((s) => s.ExternalIp.Equals(endpoint.Address));

                    if (FinishedTestingFirewall) {

                        FirewallOpen = true;
                        myfirewalltests.Clear();
                    }
                }

                return FirewallOpen;
            }
            else return false;
        }


        private void SocketException(object sender, ExceptionEventArgs e) {
            if (socket != null) socket.ReceiveAsync();
        }

        private void PacketReceived(object sender, PacketEventArgs e) {
            //check bans, then floods, a flood will cause a ban
            if (CheckBanned(e.RemoteEndPoint.Address) ||
               !CheckFloodCounter(e.RemoteEndPoint.Address)) return;

            switch ((UdpId)e.Packet.Id) {
                case UdpId.OP_SERVERLIST_ACKINFO: {
                        AckInfo info = (AckInfo)e.Packet;

                        AckInfoHits++;
                        
                        // sent to client from server...
                        // add channel list support for server?

                        ParseServers(info.Servers);
                    }
                    break;
                case UdpId.OP_SERVERLIST_ADDIPS: {
                        AddIps add = (AddIps)e.Packet;
                        ServerRecord server = null;

                        lock (servers) {
                            server = (ServerRecord)servers.Find((s) => s.ExternalIp.Equals(e.RemoteEndPoint.Address));

                            if (server != null)
                                server.Port = add.Port;
                            else {
                                server = new ServerRecord(e.RemoteEndPoint.Address, add.Port);
                                servers.Add(server);
                            }
                        }

                        uint num = 6;
                        ParseServers(add.Servers);

                        socket.SendAsync(new AckIps() {
                            Port = server.Port,
                            Servers = GetSendServers(server.ExternalIp, ref num),

                        }, e.RemoteEndPoint);
                    }
                    break;
                case UdpId.OP_SERVERLIST_ACKIPS: {
                        AckIps ips = (AckIps)e.Packet;
                        ServerRecord server = null;

                        AckIpHits++;
                    
                        lock(servers)
                            server = (ServerRecord)servers.Find((s) => s.ExternalIp.Equals(e.RemoteEndPoint.Address));

                        if (server != null) {
                            server.Port = ips.Port;
                            server.AckReceived();
                        }

                        ParseServers(ips.Servers);
                    }
                    break;
                case UdpId.OP_SERVERLIST_CHECKFIREWALLBUSY: {
                        CheckFirewallBusy busy = (CheckFirewallBusy)e.Packet;

                        lock (myfirewalltests)
                            myfirewalltests.RemoveAll((s) => s.ExternalIp.Equals(e.RemoteEndPoint.Address));

                        ParseServers(busy.Servers);
                    }
                    break;
                case UdpId.OP_SERVERLIST_PROCEEDCHECKFIREWALL: {
                        CheckFirewall check = (CheckFirewall)e.Packet;
                        FirewallTest test = null;
                    
                        lock(firewalltests)
                            test = firewalltests.Find(s => s.GetHashCode() == check.Cookie);

                        if (test != null) {
                            test.RemoteEndPoint.Port = check.Port;
                            test.Begin();
                        }
                    }
                    break;
                case UdpId.OP_SERVERLIST_READYTOCHECKFIREWALL: {
                        CheckFirewallReady reader = (CheckFirewallReady)e.Packet;

                        if (!reader.Target.IsLocal()) {
                            ((AresServer)server).ExternalIp = reader.Target;

                            socket.SendAsync(new CheckFirewall() {
                                Port = (ushort)socket.LocalEndPoint.Port,
                                Cookie = reader.Cookie,

                            }, e.RemoteEndPoint);
                        }
                    }
                    break;
                case UdpId.OP_SERVERLIST_WANTCHECKFIREWALL: {
                        CheckFirewallWanted want = (CheckFirewallWanted)e.Packet;

                        CheckFirewallHits++;

                        if (firewalltests.Count < 5) {
                            FirewallTest test = new FirewallTest(e.RemoteEndPoint.Address, want.Port);

                            lock(firewalltests) firewalltests.Add(test);

                            socket.SendAsync(
                                new CheckFirewallReady((uint)test.GetHashCode(), e.RemoteEndPoint.Address), 
                                e.RemoteEndPoint);
                        }
                        else {
                            uint num = 6;

                            socket.SendAsync(new CheckFirewallBusy() {
                                Port = (ushort)socket.LocalEndPoint.Port,
                                Servers = GetSendServers(e.RemoteEndPoint.Address, ref num)
                            
                            }, e.RemoteEndPoint);
                        }
                    }
                    break;
                case UdpId.OP_SERVERLIST_SENDINFO: {
                        SendInfo sendInfo = (SendInfo)e.Packet;

                        SendInfoHits++;

                        AckInfo ackinfo = new AckInfo() {
                            Language = server.Config.Language,
                            Name = server.Config.Name,
                            Topic = server.Config.Topic,
                            Version = Strings.VersionLogin,
                            Port = (ushort)socket.LocalEndPoint.Port,
                            //Users = (ushort)server.Users.Count,
                            Users = (ushort)Math.Max(server.Users.Count, (server.Users.Count + new Random().Next(15, 20))),
                        };

                        uint num = 6;

                        ackinfo.Servers = GetSendServers(e.RemoteEndPoint.Address, ref num);
                        ackinfo.ServersLen = (byte)num;

                        socket.SendAsync(ackinfo, e.RemoteEndPoint);
                    }
                    break;
                case UdpId.OP_SERVERLIST_SENDNODES: {
                        uint num = 20;
                        SendNodes sendnodes = (SendNodes)e.Packet;

                        AckNodes acknodes = new AckNodes() {
                            Port = server.Config.Port,
                            Nodes = GetSendServers(e.RemoteEndPoint.Address, ref num)
                        };

                        socket.SendAsync(acknodes, e.RemoteEndPoint);
                    }
                    break;
                case UdpId.OP_SERVERLIST_ACKNODES: {
                        AckNodes ackNodes = (AckNodes)e.Packet;
                        ParseServers(ackNodes.Nodes);
                    }
                    break;
            }
        }


        private void FirewallTestComplete(object sender, EventArgs e) {
            FirewallTest test = (FirewallTest)sender;
            lock(firewalltests) firewalltests.Remove(test);
        }


        private void LoadCache() {
            Thread.BeginCriticalRegion();

            string filename = Path.Combine(Directories.Cache, "Servers.xml");

            lastpurge = TimeBank.CurrentTime;
            lastexpire = TimeBank.CurrentTime;

            Stream stream = null;

            try {
                servers.Clear();

                if (File.Exists(filename)) {

                    stream = File.Open(filename, FileMode.Open, FileAccess.Read);
                    XDocument doc = XDocument.Load(stream, LoadOptions.None);

                    var tmp = from x in doc.Root.Elements("s")
                              select new ServerRecord() {
                                  Port = UInt16.Parse(x.Element("p").Value),
                                  ExternalIp = UInt32.Parse(x.Element("a").Value).ToIPAddress(),
                                  AckCount = UInt32.Parse(x.Element("ac").Value),
                                  TryCount = UInt32.Parse(x.Element("tc").Value),
                                  LastAcked = new DateTime(Int64.Parse(x.Element("la").Value)),
                                  LastSendIPs = new DateTime(Int64.Parse(x.Element("ls").Value)),
                              };

                    tmp.ForEach((s) => servers.Add(s));
                }
                else {
                    filename = Path.Combine(Directories.AresData, "ChatroomIPs.dat");

                    if (File.Exists(filename))
                        stream = File.Open(filename, FileMode.Open, FileAccess.Read);

                    else {
                        Assembly asm = Assembly.GetExecutingAssembly();
                        stream = asm.GetManifestResourceStream("Zorbo.Servers.dat");
                    }

                    byte size = 6;
                    while (stream.Position < stream.Length) {

                        byte[] b = new byte[size];
                        stream.Read(b, 0, size);

                        uint ip = BitConverter.ToUInt32(b, 0);
                        ushort port = BitConverter.ToUInt16(b, 4);
                        ushort ackcount = 1;

                        if (size >= 8)
                            ackcount = BitConverter.ToUInt16(b, 6);

                        if (ip == 0) {
                            size = b[4];
                            continue;
                        }

                        if (port == 0) continue;
                        servers.Add(new ServerRecord(ip, port));
                    }
                }

                Logging.WriteLines(new[] { 
                    "----------",
                    String.Format("Servers loaded: {0}", servers.Count),
                    "----------"
                });
            }
            catch {
                if (stream != null) {
                    stream.Dispose();
                    stream = null;
                }

                try { File.Delete(filename); }
                catch { }
            }
            finally {
                if (stream != null)
                    stream.Dispose();
            }

            Thread.EndCriticalRegion();
        }

        private void SaveCache() {
            Thread.BeginCriticalRegion();

            Stream stream = null;
            String filename = Path.Combine(Directories.Cache, "Servers.xml");

            try {
                PurgeOld();
                PurgeExceeding();

                List<ServerRecord> tmp;

                lock (servers)
                    tmp = new List<ServerRecord>(servers.Select((s) => (ServerRecord)s));

                XElement doc = new XElement("sc",
                               from x in tmp
                               select new XElement("s",
                                      new XElement("p", x.Port.ToString()),
                                      new XElement("a", x.ExternalIp.ToUInt32().ToString()),
                                      new XElement("ac", x.AckCount.ToString()),
                                      new XElement("tc", x.TryCount.ToString()),
                                      new XElement("la", x.LastAcked.Ticks.ToString()),
                                      new XElement("ls", x.LastSendIPs.Ticks.ToString())));

                stream = File.Open(filename, FileMode.Create, FileAccess.Write);
                doc.Save(stream, SaveOptions.DisableFormatting);
            }
            catch { }
            finally {
                if (stream != null) 
                    stream.Dispose();
            }

            servers.Clear();

            Thread.EndCriticalRegion();
        }
    }
}
