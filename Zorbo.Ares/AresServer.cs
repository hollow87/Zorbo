using System;
using System.Net;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.ComponentModel;

using Zorbo;
using Zorbo.Resources;

using Zorbo.Sockets;

using Zorbo.Packets;
using Zorbo.Packets.Ares;
using Zorbo.Packets.Formatters;

using Zorbo.Interface;
using Zorbo.Plugins;
using Zorbo.Users;
using System.Diagnostics;

namespace Zorbo
{
    public class AresServer : NotifyObject, IServer
    {
        Timer timer;
        TimeSpan ticklength;

        IServerConfig config;

        History history;
        AresServerStats stats;
        AresClientList users;
        AresChannels channels;
        AresTcpSocket listener;

        IPAddress localip;
        IPAddress externalip;

        PluginHost plugins;

        SortedStack<UInt16> idpool;
        Comparison<IClient> sorter = (a, b) => (a.Id - b.Id);

        List<PendingConnection> pending;
        List<IFloodRule> flood_rules;

        class PendingConnection : 
            IDisposable,
            IEquatable<ISocket>
        {
            public ISocket Socket { get; set; }
            public DateTime Time { get; set; }

            public PendingConnection() { }

            public PendingConnection(ISocket socket, DateTime time) {
                this.Socket = socket;
                this.Time = time;
            }

            public bool Equals(ISocket socket) {
                return (Socket == socket);
            }

            public void Dispose() {
                if (Socket != null)
                    Socket.Dispose();
            }
        }

        public Boolean Running {
            get { return listener != null; }
        }

        public IPAddress LocalIp {
            get { return localip; }
            private set {
                if (localip == null || !localip.Equals(value)) {
                    localip = value;
                    RaisePropertyChanged(() => LocalIp);

                    Logging.WriteLines(new[] { 
                        "----------",
                        String.Format("Reported local ip: {0}", value),
                        "----------"
                    });
                }
            }
        }

        public IPAddress ExternalIp {
            get { return externalip; }
            internal set {
                if (externalip == null || !externalip.Equals(value)) {

                    externalip = value;
                    RaisePropertyChanged(() => ExternalIp);

                    Logging.WriteLines(new[] { 
                        "----------",
                        String.Format("Reported external ip: {0}", value),
                        "----------"
                    });
                }
            }
        }

        public AresServerStats Stats {
            get { return stats; }
        }

        IServerStats IServer.Stats {
            get { return stats; }
        }

        public IServerConfig Config {
            get { return config; }
        }

        public IChannelList Channels {
            get { return channels; }
        }

        public IHistory History {
            get { return history; }
        }

        public IPluginHost PluginHost {
            get { return plugins; }
        }

        public IReadOnlyList<IClient> Users {
            get { return users; }
        }

        public IList<IFloodRule> FloodRules {
            get { return flood_rules; }
        }

        internal PluginHost PluginManager {
            get { return plugins; }
        }


        public AresServer(IServerConfig config) {

            if (config == null)
                throw new ArgumentNullException("config", "Server configuration cannot be null.");

            this.config = config;

            ticklength = TimeSpan.FromSeconds(1);

            stats = new AresServerStats();
            users = new AresClientList();
            channels = new AresChannels(this);
            plugins = new PluginHost(this);

            idpool = new SortedStack<UInt16>();
            idpool.SetSort((a, b) => (a - b));

            pending = new List<PendingConnection>();
            flood_rules = new List<IFloodRule>();

            history = new History(this);
            history.Load();
        }

        public void Start() {
            stats.Start();
            channels.Start();

            for (ushort i = 0; i < config.MaxClients; i++)
                idpool.Push(i);

            config.PropertyChanged += Config_PropertyChanged;

            listener = new AresTcpSocket(new ClientFormatter());
            listener.Accepted += ClientAccepted;

            Logging.WriteLines(new[] { 
                "Server started",
                "----------",
                String.Format("Listening on port: {0}", config.Port),
                "----------"
            });

            IPHostEntry entry = Dns.GetHostEntry(Environment.MachineName);

            foreach (var ip in entry.AddressList) {
                if (ip.AddressFamily == listener.Socket.AddressFamily) {
                    LocalIp = ip;
                    break;
                }
            }

            listener.Bind(new IPEndPoint(IPAddress.Any, config.Port));
            listener.Listen(25);

            RaisePropertyChanged(() => Running);

            timer = new Timer(new TimerCallback(OnTimer), null, ticklength, ticklength);
        }

        public void Stop() {

            timer.Change(-1, -1);
            timer.Dispose();

            config.PropertyChanged -= Config_PropertyChanged;

            listener.Dispose();
            listener = null;

            idpool.Clear();

            stats.Reset();
            channels.Stop();

            RaisePropertyChanged(() => Running);

            lock (pending) {
                foreach (var sock in pending)
                    sock.Dispose();//suppress disconnect event

                pending.Clear();
            }

            lock (users.SyncRoot) {
                foreach (var user in users) {
                    history.Add(user);
                    user.Dispose();//suppress disconnect event
                }
                users.Clear();
            }

            history.Save();
        }


        private void OnTimer(object state) {
            DateTime now = TimeBank.CurrentTime;

            lock (pending) {
                foreach (var sock in pending) {
                    if (now.Subtract(sock.Time).TotalSeconds >= 30)
                        sock.Socket.Disconnect();
                }
            }

            lock (users.SyncRoot) {
                
                foreach (var user in users) {
                    if (user.IsCaptcha) {
                        if (now.Subtract(user.CaptchaTime).TotalHours >= 2) {
                            stats.CaptchaBanned++;
                            SendAnnounce(user, Strings.BannedCaptchaTimeout);
                            user.Ban();
                            continue;
                        }
                    }

                    if (now.Subtract(user.LastUpdate).TotalMinutes >= 5)
                        user.Disconnect();
                }
            }
        }

        public void SendUserlist(IClient user) {

            lock (users.SyncRoot) {
                Join join = new Join(user);

                if (!user.IsCaptcha) {

                    user.SendPacket(new Userlist() { 
                        Username = Config.BotName,
                        FileCount = 420,
                        Level = AdminLevel.Host,
                    });

                    foreach (var s in users) {
                        if (s != user &&
                            s.Connected &&
                            s.LoggedIn &&
                           !s.IsCaptcha &&
                            s.Vroom == user.Vroom) {

                            s.SendPacket(join);
                            user.SendPacket(new Userlist(s));
                        }
                    }
                }

                user.SendPacket(new Userlist(user));
                user.SendPacket(new UserlistEnd());
            }
        }

        public void SendAvatars(IClient user) {

            lock (users.SyncRoot) {

                if (!Config.Avatar.Equals(AresAvatar.Null))
                    user.SendPacket(new ServerAvatar(Config.BotName, Config.Avatar));

                user.SendPacket(new ServerPersonal(Config.BotName, Strings.VersionLogin));

                foreach (var s in users) {

                    if (!s.IsCaptcha && s.Vroom == user.Vroom) {

                        s.SendPacket(new ServerAvatar(user));

                        if (!String.IsNullOrEmpty(user.Message))
                            s.SendPacket(new ServerPersonal(user.Name, user.Message));

                        user.SendPacket(new ServerAvatar(s));

                        if (!String.IsNullOrEmpty(s.Message))
                            user.SendPacket(new ServerPersonal(s.Name, s.Message));
                    }
                }
            }
        }


        public void SendPacket(IPacket packet) {
            lock (users.SyncRoot) {
                foreach (var user in users)
                    user.SendPacket((s) => !s.IsCaptcha, packet);
            }
        }

        public void SendPacket(IClient user, IPacket packet) {
            if (user != null) user.SendPacket(packet);
        }

        public void SendPacket(Predicate<IClient> match, IPacket packet) {
            lock (users.SyncRoot) {
                foreach (var user in users)
                    user.SendPacket(match, packet);
            }
        }


        public IClient FindUser(Predicate<IClient> match) {
            lock (users.SyncRoot) {
                foreach (var user in users)
                    if (user.Connected && 
                        user.LoggedIn && 
                        match(user)) return user;
            }

            return null;
        }


        protected virtual void ClientAccepted(object sender, AcceptEventArgs e) {

            var socket = (AresTcpSocket)e.Socket;

            if (!channels.FinishedTestingFirewall && 
                 channels.IsCheckingMyFirewall(socket.RemoteEndPoint)) {

                socket.Dispose();
            }
            else {
                socket.Exception += ClientException;
                socket.Disconnected += ClientDisconnected;
                socket.PacketSent += ClientPacketSent;
                socket.PacketReceived += ClientPacketReceived;

                lock(pending)
                    pending.Add(new PendingConnection(socket, TimeBank.CurrentTime));

                socket.ReceiveAsync();
            }
        }

        protected virtual void ClientException(object sender, ExceptionEventArgs e) {

            AresClient user = null;

            lock (users.SyncRoot)
                user = users.Find(s => s.Socket == sender);
            
            if (user == null) return;

            Logging.WriteLines(new[] {
                "----------",
                string.Format("Exception occured in client: {0}", user.Name),
                string.Format("Message: {0}", e.Exception.Message),
                e.Exception.StackTrace,
                "----------"
            });

            user.Disconnect();
        }

        protected virtual void ClientPacketSent(object sender, PacketEventArgs e) {
            stats.PacketsSent++;
            stats.AddOutput(e.Transferred);
        }

        protected virtual void ClientPacketReceived(object sender, PacketEventArgs e) {
            stats.PacketsReceived++;
            stats.AddInput(e.Transferred);

            var socket = sender as ISocket;
            if (socket == null) return;

            AresClient user = null;

            lock (users.SyncRoot)
                user = users.Find(s => s.Socket == socket);

            if (user == null) {
                if ((AresId)e.Packet.Id == AresId.MSG_CHAT_CLIENT_LOGIN) {

                    if (HandlePending(socket)) {

                        int id = idpool.Pop();
                        user = new AresClient(this, socket, (ushort)id);

                        if (user.ExternalIp.IsLocal() || user.ExternalIp.Equals(ExternalIp))
                            user.LocalHost = true;

                        lock (users.SyncRoot) {
                            users.Add(user);
                            users.Sort(sorter);
                            stats.PeakUsers = Math.Max(users.Count, stats.PeakUsers);
                        }

                        user.HandleJoin(e);
                    }
                    else {
                        socket.Disconnect();
                    }
                }
            }
            else if ((AresId)e.Packet.Id == AresId.MSG_CHAT_CLIENT_LOGIN) {
                //sending too many login packets
                SendAnnounce(user, Errors.LoginFlood);
                user.Disconnect();
            }
            else if (user.LoggedIn && CheckCounters(user, e.Packet)) {

                user.LastUpdate = TimeBank.CurrentTime;

                if (!user.HandlePacket(e) && plugins.OnBeforePacket(user, e.Packet)) {

                    user.HandleOverridePacket(e);
                    plugins.OnAfterPacket(user, e.Packet);
                }
            }
        }

        protected virtual void ClientDisconnected(object sender, DisconnectEventArgs e) {
            AresClient user = null;

            var socket = sender as ISocket;
            if (socket == null) return;

            lock (pending)
                pending.RemoveAll((s) => s.Equals(socket));

            lock (users.SyncRoot) {

                int uindex = users.FindIndex(s => s.Socket == sender);
                if (uindex == -1) return;

                user = (AresClient)users[uindex];
                users.RemoveAt(uindex);
            }

            if (user.LoggedIn) {
                history.Add(user);

                SendPacket((s) =>
                    !s.Guid.Equals(user.Guid) && //ghost?
                     s.Vroom == user.Vroom,
                     new Parted(user.Name));
            }

            plugins.OnPart(user, e.UserToken);

            idpool.Push(user.Id);
            user.Dispose();
        }


        protected virtual bool HandlePending(ISocket socket) {
            lock (pending) {
                var conn = pending.Find((s) => s.Equals(socket));
                if (conn == null) return false;

                pending.Remove(conn);
            }
            return true;
        }

        protected virtual bool CheckCounters(AresClient user, IPacket packet) {

            var rules = flood_rules.FindAll((s) => s.Id == packet.Id);

            foreach(var rule in rules) {

                FloodCounter counter = null;
                DateTime now = TimeBank.CurrentTime;

                if (!user.Counters.ContainsKey(packet.Id)) {

                    counter = new FloodCounter(0, now);
                    user.Counters.Add(packet.Id, counter);
                }
                else {
                    counter = user.Counters[packet.Id];

                    if (now.Subtract(counter.Last).TotalMilliseconds > rule.ResetTimeout)
                        counter.Count = 0;
                }

                if (++counter.Count >= rule.Count) {
                    stats.FloodsTriggered++;

                    if (!plugins.OnFlood(user, packet))
                        return false;
                }
                else counter.Last = now;
            }
            
            return true;
        }


        private void Config_PropertyChanged(object sender, PropertyChangedEventArgs e) {

            switch (e.PropertyName) {
                case "Topic":
                    SendPacket(new Topic(Config.Topic));
                    break;
                case "Avatar":
                    var allPacket = new ServerAvatar(Config.BotName, Config.Avatar);
                    lock (users.SyncRoot) {
                        foreach (var user in users) {
                            if (user.Avatar.Equals(Config.Avatar)) {
                                SendPacket(
                                    (s) => s.Vroom == user.Vroom,
                                    new ServerAvatar(user.Name, Config.Avatar));
                            }
                            SendPacket(user, allPacket);
                        }
                    }
                    break;
            }
        }


        #region " IServer Methods "

        public void SendText(string target, string sender, string text) {
            SendPacket((s) => s.Name == target, new ServerPublic(sender, text));
        }

        public void SendText(IClient target, string sender, string text) {
            SendPacket((s) => s == target, new ServerPublic(sender, text));
        }

        public void SendText(IClient target, IClient sender, string text) {
            SendPacket((s) => s == target, new ServerPublic(sender.Name, text));
        }

        public void SendText(string sender, string text) {
            SendPacket(new ServerPublic(sender, text));
        }

        public void SendText(IClient sender, string text) {
            SendPacket(new ServerPublic(sender.Name, text));
        }

        public void SendText(Predicate<IClient> match, string sender, string text) {
            SendPacket(match, new ServerPublic(sender, text));
        }

        public void SendText(Predicate<IClient> match, IClient sender, string text) {
            SendPacket(match, new ServerPublic(sender.Name, text));
        }


        public void SendEmote(string target, string sender, string text) {
            SendPacket((s) => s.Name == target, new ServerEmote(sender, text));
        }

        public void SendEmote(IClient target, string sender, string text) {
            SendPacket((s) => s == target, new ServerEmote(sender, text));
        }

        public void SendEmote(IClient target, IClient sender, string text) {
            SendPacket((s) => s == target, new ServerEmote(sender.Name, text));
        }

        public void SendEmote(string sender, string text) {
            SendPacket(new ServerEmote(sender, text));
        }

        public void SendEmote(IClient sender, string text) {
            SendPacket(new ServerEmote(sender.Name, text));
        }

        public void SendEmote(Predicate<IClient> match, string sender, string text) {
            SendPacket(match, new ServerEmote(sender, text));
        }

        public void SendEmote(Predicate<IClient> match, IClient sender, string text) {
            SendPacket(match, new ServerEmote(sender.Name, text));
        }


        public void SendPrivate(string target, string sender, string text) {
            SendPacket((s) => s.Name == target, new Private(sender, text));
        }

        public void SendPrivate(IClient target, string sender, string text) {
            SendPacket((s) => s == target, new Private(sender, text));
        }

        public void SendPrivate(IClient target, IClient sender, string text) {
            SendPacket((s) => s == target, new Private(sender.Name, text));
        }

        public void SendPrivate(Predicate<IClient> match, string sender, string text) {
            SendPacket(match, new Private(sender, text));
        }

        public void SendPrivate(Predicate<IClient> match, IClient sender, string text) {
            SendPacket(match, new Private(sender.Name, text));
        }


        public void SendAnnounce(string text) {
            SendPacket(new Announce(text));
        }

        public void SendAnnounce(string target, string text) {
            SendPacket((s) => s.Name == target, new Announce(text));
        }

        public void SendAnnounce(IClient target, string text) {
            SendPacket((s) => s == target, new Announce(text));
        }

        public void SendAnnounce(Predicate<IClient> match, string text) {
            SendPacket(match, new Announce(text));
        }


        public void SendWebsite(string address, string caption) {
            SendPacket(new Website(address, caption));
        }

        public void SendWebsite(string target, string address, string caption) {
            SendPacket((s) => s.Name == target, new Website(address, caption));
        }

        public void SendWebsite(IClient target, string address, string caption) {
            SendPacket((s) => s == target, new Website(address, caption));
        }

        public void SendWebsite(Predicate<IClient> match, string address, string caption) {
            SendPacket(match, new Website(address, caption));
        }


        public void SendHtml(string html) {
            SendPacket(new ServerHtml(html));
        }

        public void SendHtml(string target, string html) {
            SendPacket((s) => s.Name == target, new ServerHtml(html));
        }

        public void SendHtml(IClient target, string html) {
            SendPacket((s) => s == target, new ServerHtml(html));
        }

        public void SendHtml(Predicate<IClient> match, string html) {
            SendPacket(match, new ServerHtml(html));
        }

        #endregion
    }
}
