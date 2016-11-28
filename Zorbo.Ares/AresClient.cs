using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.ComponentModel;

using Zorbo.Resources;
using Zorbo.Sockets;

using Zorbo.Packets;
using Zorbo.Packets.Ares;

using Zorbo.Interface;
using Zorbo.Users;
using Zorbo.Data;

namespace Zorbo
{
    public class AresClient : NotifyObject, IClient, IDisposable
    {
        #region " Variables "

        ISocket socket;

        AresServer server;
        AdminLevel admin;

        ushort id;
        Guid guid;

        uint cookie;

        int captchaTries = 5;
        int captchaAnswer;

        bool local;
        bool browse;
        bool cloaked;
        bool muzzled;
        bool captcha;
        bool fastping;
        bool compression;
        bool encryption;

        bool logged;

        byte age;
        Gender gender;
        Country country;

        IAvatar avatar;
        IAvatar orgavatar;

        string name;
        string orgname;
        string version;
        string region;
        string message;

        ushort vroom;
        ushort dcport;
        ushort nodeport;

        IPAddress nodeip;
        IPAddress localip;

        IPHostEntry dnsentry;

        ClientFeatures features;

        DateTime spawntime;
        DateTime captchatime;
        DateTime lastupdate;

        List<String> ignored;

        Dictionary<Byte, FloodCounter> counters;

        ObservableCollection<ISharedFile> files;
        ReadOnlyList<ISharedFile> pfiles;

        Dictionary<String, Object> extended_props;

        #endregion

        #region " Properties "

        public ushort Id { 
            get { return id; }
            private set { id = value; }
        }

        public Guid Guid {
            get { return guid; }
            private set {
                if (!guid.Equals(value)) {
                    guid = value;
                    RaisePropertyChanged(() => Guid);
                }
            }
        }

        public uint Cookie { 
            get {
                if (cookie == 0)
                    cookie = (uint)GetHashCode();

                return cookie; 
            }
        }
        

        public ISocket Socket {
            get { return socket; }
        }

        public IServer Server {
            get { return server; }
        }

        public IMonitor Monitor {
            get {
                if (socket != null)
                    return socket.Monitor;

                return null;
            }
        }

        public IClientId ClientId {
            get { return new ClientId(Guid, ExternalIp); }
        }


        public bool Connected {
            get { return socket != null && socket.Connected; }
        }

        public bool IsCaptcha {
            get { return captcha; }
            set {
                if (captcha != value) {
                    captcha = value;
                    RaisePropertyChanged(() => IsCaptcha);

                    var record = server.History.Find((s) => s.Equals(this));

                    if (value) {
                        record.Trusted = false;

                        if (LoggedIn)
                            server.SendPacket((s) => 
                                s != this &&
                                s.Vroom == Vroom, 
                                new Parted(Name));

                        ShowCaptcha();
                    }
                    else {
                        captchaTries = 5;

                        record.Trusted = true;
                        server.PluginManager.OnCaptcha(this, CaptchaEvent.Exit);
                        
                        FinishJoin();
                    }
                }
            }
        }

        public bool LoggedIn {
            get { return logged; }
            private set { logged = value; }
        }
        

        public bool LocalHost {
            get { return local; }
            internal set {
                if (value) {
                    local = value;
                    RaisePropertyChanged(() => LocalHost);
                }
            }
        }

        public bool Browsable {
            get { return browse; }
            private set {
                if (browse != value) {
                    browse = value;
                    RaisePropertyChanged(() => Browsable);
                }
            }
        }

        public bool Cloaked {
            get { return cloaked; }
            set {
                if (cloaked != value) {
                    cloaked = value;
                    RaisePropertyChanged(() => Cloaked);

                    if (cloaked)
                        server.SendPacket((s) =>
                            s != this &&
                            s.Admin < Admin &&
                            s.Vroom == Vroom,
                            new Parted(Name));
                    else
                        server.SendPacket((s) =>
                            s != this &&
                            s.Admin < Admin &&
                            s.Vroom == Vroom,
                            new Join(this));
                }
            }
        }

        public bool Muzzled {
            get { return muzzled; }
            set {
                if (muzzled != value) {
                    muzzled = value;
                    RaisePropertyChanged(() => Muzzled);

                    if (muzzled)
                        server.SendAnnounce(this, Strings.Muzzled);
                    else
                        server.SendAnnounce(this, Strings.Unmuzzled);
                }
            }
        }

        public bool FastPing {
            get { return fastping; }
            private set { fastping = value; }
        }

        public bool Compression {
            get { return compression; }
            private set {
                if (compression != value) {
                    compression = value;
                    RaisePropertyChanged(() => Compression);
                }
            }
        }

        public bool Encryption {
            get { return encryption; }
            private set {
                if (encryption != value) {
                    encryption = value;
                    RaisePropertyChanged(() => Encryption);
                }
            }
        }


        public byte Age {
            get { return age; }
            private set {
                if (age != value) {
                    age = value;
                    RaisePropertyChanged(() => Age);
                }
            }
        }

        public Gender Gender { 
            get { return gender; }
            private set {
                if (gender != value) {
                    gender = value;
                    RaisePropertyChanged(() => Gender);
                }
            }
        }

        public Country Country { 
            get { return country; }
            private set {
                if (country != value) {
                    country = value;
                    RaisePropertyChanged(() => Country);
                }
            }
        }

        public AdminLevel Admin {
            get { return admin; }
            set {
                if (admin != value) {

                    if (LocalHost && value < admin)
                        return;

                    if (value > AdminLevel.Host)
                        value = AdminLevel.Host;

                    admin = value;
                    RaisePropertyChanged(() => Admin);

                    server.SendPacket(this, new OpChange(Admin > 0));
                    server.SendPacket((s) => s.Vroom == Vroom, new ServerUpdate(this));
                }
            }
        }


        public IAvatar Avatar {
            get {
                if (avatar == null)
                    return server.Config.Avatar;

                return avatar; 
            }
            set {
                if (!value.Equals(AresAvatar.Null) && avatar == null || !avatar.Equals(value)) {

                    avatar = value;
                    RaisePropertyChanged(() => Avatar);

                    server.SendPacket((s) =>
                        s.Vroom == Vroom,
                        new ServerAvatar(this));
                }
            }
        }

        public IAvatar OrgAvatar { 
            get { return orgavatar; }
            private set { orgavatar = value; }
        }


        public string Name {
            get { return name; }
            set {
                if (name != value)
                    ChangeName(name, value);
            }
        }

        public string OrgName {
            get { return orgname; }
            private set { orgname = value; }
        }

        public string Version { 
            get { return version; }
            private set { version = value; }
        }

        public string Region { 
            get { return region; }
            internal set {
                if (region != value) {
                    region = value;
                    RaisePropertyChanged(() => Region);
                }
            }
        }

        public string Message { 
            get { return message; }
            set {
                if (message != value) {
                    message = value;
                    RaisePropertyChanged(() => Message);

                    server.SendPacket((s) =>
                        s.Vroom == Vroom,
                        new ServerPersonal(Name, message));
                }
            }
        }


        public ushort Vroom {
            get { return vroom; }
            set {
                if (vroom != value)
                    ChangeVroom(vroom, value);
            }
        }

        public ushort FileCount {
            get { return (ushort)files.Count; }
        }

        public ushort ListenPort { 
            get { return dcport; }
            private set { dcport = value; }
        }

        public ushort NodePort {
            get { return nodeport; }
            private set { nodeport = value; }
        }


        public IPAddress NodeIp {
            get { return nodeip; }
            private set {
                if (nodeip != value) {
                    nodeip = value;
                    RaisePropertyChanged(() => NodeIp);
                }
            }
        }

        public IPAddress LocalIp {
            get { return localip; }
            private set {
                if (localip != value) {
                    localip = value;
                    RaisePropertyChanged(() => LocalIp);
                }
            }
        }

        public IPAddress ExternalIp {
            get {
                if (socket != null)
                    try {
                        return socket.RemoteEndPoint.Address;
                    }
                    catch { }

                return null;
            }
        }

        public IPHostEntry DnsEntry {
            get { return dnsentry; }
            internal set {
                if (dnsentry == null || !dnsentry.Equals(value)) {
                    dnsentry = value;
                    RaisePropertyChanged(() => DnsEntry);
                }
            }
        }

        public ClientFeatures Features {
            get { return features; }
            set {
                if (features != value) {
                    features = value;
                    RaisePropertyChanged(() => Features);
                }
            }
        }


        public IList<String> Ignored {
            get { return ignored; }
        }

        public IReadOnlyList<ISharedFile> Files {
            get { return pfiles; }
        }


        IDictionary<String, Object> IClient.Extended {
            get { return extended_props; }
        }


        public DateTime SpawnTime {
            get { return spawntime; }
            internal set { spawntime = value; }
        }

        public DateTime CaptchaTime {
            get { return captchatime; }
            internal set { captchatime = value; }
        }

        public DateTime LastUpdate {
            get { return lastupdate; }
            internal set { lastupdate = value; }
        }


        internal Dictionary<Byte, FloodCounter> Counters {
            get { return counters; }
            private set { counters = value; }
        }

        #endregion


        internal AresClient(AresServer server, ISocket socket, ushort id) {
            this.id = id;

            this.server = server;
            this.socket = socket;

            this.spawntime = TimeBank.CurrentTime;
            this.lastupdate = this.spawntime;
            
            this.ignored = new List<string>();

            this.files = new ObservableCollection<ISharedFile>();
            this.pfiles = new ReadOnlyList<ISharedFile>(files);

            this.counters = new Dictionary<byte, FloodCounter>();
            this.extended_props = new Dictionary<string, object>();
        }

        public void SendPacket(IPacket packet) {
            if (Connected && LoggedIn)
                socket.SendAsync(packet);
        }

        public void SendPacket(Predicate<IClient> match, IPacket packet) {
            if (match(this)) SendPacket(packet);
        }
        
        private void ShowCaptcha() {

            if (!LoggedIn)
                PerformLogin();
            
            else PerformQuickLogin();

            captchatime = TimeBank.CurrentTime;
            captchaAnswer = Captcha.Create(this);

            server.PluginManager.OnCaptcha(this, CaptchaEvent.Enter);
        }

        private void FinishCaptcha(string message) {
            int sum = 0;

            if (Int32.TryParse(message, out sum)) {
                if (sum == captchaAnswer) {
                    IsCaptcha = false;
                    return;
                }
            }

            if (--captchaTries <= 0) {
                server.Stats.CaptchaBanned++;

                server.SendAnnounce(this, Strings.BannedCaptchaAnswers);
                server.PluginManager.OnCaptcha(this, CaptchaEvent.Banned);

                Ban();
            }
            else {
                server.SendAnnounce(this, String.Format(Strings.InvalidCaptcha, captchaTries));
                server.PluginManager.OnCaptcha(this, CaptchaEvent.Failed);
            }
        }


        internal void PerformLogin() {
            LoggedIn = true;

            server.SendPacket(this, new LoginAck() {
                Username = Name,
                ServerName = server.Config.Name,
            });

            var features = server.PluginManager.OnSendFeatures(this, server.Config.GetFeatures());

            server.SendPacket(this, new Features() {
                Version = Strings.VersionLogin,
                SupportFlag = features,
                SharedTypes = 63,
                Language = server.Config.Language,
                Cookie = this.Cookie,
            });

            server.SendPacket(this, new TopicFirst(server.Config.Topic));
            server.SendUserlist(this);

            if (!IsCaptcha) {
             
                server.SendAvatars(this);
                server.SendPacket(this, new OpChange(Admin > AdminLevel.User));

                server.PluginManager.OnSendJoin(this);
            }
        }

        internal void PerformQuickLogin() {

            server.SendPacket(this, new LoginAck() {
                Username = Name,
                ServerName = server.Config.Name,
            });

            server.SendPacket(this, new TopicFirst(server.Config.Topic));
            server.SendUserlist(this);

            if (!IsCaptcha) {
                if (!String.IsNullOrEmpty(Message))
                    server.SendPacket((s) =>
                        s.Vroom == Vroom,
                        new ServerPersonal(Name, Message));

                server.SendAvatars(this);
                server.SendPacket(this, new OpChange(Admin > 0));

                server.PluginManager.OnSendJoin(this);
            }
        }


        internal void ChangeName(string oldname, string newname) {
            name = newname.FormatUsername();
            if (string.IsNullOrWhiteSpace(name)) {
                name = ExternalIp.AnonUsername();
            }
            RaisePropertyChanged(() => Name);

            server.SendPacket((s) =>
                s != this && 
                s.Vroom == Vroom,
                new Parted() { Username = oldname });

            PerformQuickLogin();
        }

        internal void ChangeVroom(ushort oldvroom, ushort newvroom) {

            if (server.PluginManager.OnVroomJoinCheck(this, newvroom)) {

                server.PluginManager.OnVroomPart(this);

                vroom = newvroom;
                RaisePropertyChanged(() => Vroom);

                server.SendPacket((s) =>
                    s != this &&
                    s.Vroom == oldvroom,
                    new Parted(Name));

                PerformQuickLogin();
                server.PluginManager.OnVroomJoin(this);
            }
        }


        internal void HandleJoin(PacketEventArgs e) {
            Login login = (Login)e.Packet;

            Guid = login.Guid;
            Encryption = login.Encryption == 250;
            ListenPort = login.ListenPort;
            NodeIp = login.NodeIp;
            NodePort = login.NodePort;
            name = login.Username.FormatUsername();
            if (string.IsNullOrWhiteSpace(name)) {
                name = ExternalIp.AnonUsername();
            }
            orgname = name;
            Version = login.Version;
            LocalIp = login.LocalIp;
            Browsable = (login.SupportFlag & 2) == 2;
            Compression = (login.SupportFlag & 4) == 4;
            Age = login.Age;
            Gender = login.Gender;
            Country = login.Country;
            Region = login.Region;
            Features = login.Features;

            if ((Features & ClientFeatures.OPUS_VOICE) == ClientFeatures.OPUS_VOICE)
                Features |= ClientFeatures.VOICE;

            if ((Features & ClientFeatures.PRIVATE_OPUS_VOICE) == ClientFeatures.PRIVATE_OPUS_VOICE)
                Features |= ClientFeatures.PRIVATE_VOICE;
            
            var record = server.History.Add(this);
            var autologin = server.History.Admin.Passwords.Find((s) => s.ClientId.Equals(record));

            admin = (autologin != null) ? autologin.Level : admin;
            admin = (LocalHost) ? AdminLevel.Host : admin;

            if (admin != AdminLevel.User)
                RaisePropertyChanged("Admin");

            if (!LocalHost) {
                DnsHelper.Resolve(record, FinishResolve);
            }
            else if (AllowedJoin(record))
                FinishJoin();
        }

        private void FinishResolve(IRecord record, IPHostEntry entry) {
            if (entry != null)
                DnsEntry = entry;

            if (Connected && AllowedJoin(record)) {

                if (server.Config.BotProtection) {
                    if (!record.Trusted) {
                        IsCaptcha = true;
                        return;
                    }
                }

                FinishJoin();
            }
        }

        private void FinishJoin() {
            PerformLogin();
            server.PluginManager.OnJoin(this);
        }

        /// <summary>
        /// Packets handled in this function are 'internal' cannot be overriden.
        /// </summary>
        internal bool HandlePacket(PacketEventArgs e) {

            if (IsCaptcha) {
                switch ((AresId)e.Packet.Id) {
                    case AresId.MSG_CHAT_CLIENT_FASTPING:
                        fastping = true;
                        break;
                    case AresId.MSG_CHAT_CLIENT_AUTOLOGIN:
                        AutoLogin login = (AutoLogin)e.Packet;
                        Commands.HandleAutoLogin(server, this, login.Sha1Password);
                        break;
                    case AresId.MSG_CHAT_CLIENT_PUBLIC:
                        ClientPublic pub = (ClientPublic)e.Packet;
                        FinishCaptcha(pub.Message);
                        break;
                    case AresId.MSG_CHAT_CLIENT_EMOTE:
                        ClientEmote emote = (ClientEmote)e.Packet;
                        FinishCaptcha(emote.Message);
                        break;
                    case AresId.MSG_CHAT_CLIENT_ADDSHARE:
                        SharedFile addfile = (SharedFile)e.Packet;

                        if (server.PluginManager.OnFileReceived(this, addfile))
                            lock (files) files.Add(addfile);

                        break;
                    case AresId.MSG_CHAT_CLIENT_UPDATE_STATUS:
                        ClientUpdate update = (ClientUpdate)e.Packet;

                        lastupdate = TimeBank.CurrentTime;

                        NodeIp = update.NodeIp;
                        NodePort = update.NodePort;
                        Age = (update.Age != 0) ? update.Age : Age;
                        Gender = (update.Gender != 0) ? update.Gender : Gender;
                        Country = (update.Country != 0) ? update.Country : Country;
                        Region = !String.IsNullOrEmpty(update.Region) ? update.Region : Region;

                        break;
                }

                return true; //don't handle any other packets yet
            }
            else if (LoggedIn) {

                switch ((AresId)e.Packet.Id) {
                    case AresId.MSG_CHAT_CLIENT_FASTPING:
                        fastping = true;
                        return true;
                    case AresId.MSG_CHAT_CLIENT_DUMMY:
                        return true;
                    case AresId.MSG_CHAT_CLIENT_PUBLIC:
                        ClientPublic pub = (ClientPublic)e.Packet;

                        if (!String.IsNullOrEmpty(pub.Message))
                            if (pub.Message.StartsWith("#") && Commands.HandlePreCommand(server, this, pub.Message.Substring(1)))
                                return true;

                        if (Muzzled) {
                            server.SendAnnounce(this, Strings.AreMuzzled);
                            return true;
                        }

                        return false;
                    case AresId.MSG_CHAT_CLIENT_EMOTE:
                        ClientEmote emote = (ClientEmote)e.Packet;

                        if (!String.IsNullOrEmpty(emote.Message))
                            if (emote.Message.StartsWith("#") && Commands.HandlePreCommand(server, this, emote.Message.Substring(1)))
                                return true;

                        if (Muzzled) {
                            server.SendAnnounce(this, Strings.AreMuzzled);
                            return true;
                        }

                        return false;
                    case AresId.MSG_CHAT_CLIENT_COMMAND:
                        Command cmd = (Command)e.Packet;
                        Commands.HandlePreCommand(server, this, cmd.Message);
                        break;
                    case AresId.MSG_CHAT_CLIENT_PVT:
                        Private pvt = (Private)e.Packet;

                        if (Muzzled && !server.Config.MuzzledPMs) {

                            pvt.Message = "[" + Strings.AreMuzzled + "]";
                            server.SendPacket(this, pvt);

                            return true;
                        }

                        return false;
                    case AresId.MSG_CHAT_CLIENT_AUTHREGISTER: {
                            AuthRegister reg = (AuthRegister)e.Packet;
                            Commands.HandleRegister(server, this, reg.Password);
                        }
                        return true;
                    case AresId.MSG_CHAT_CLIENT_AUTHLOGIN: {
                            AuthLogin login = (AuthLogin)e.Packet;
                            Commands.HandleLogin(server, this, login.Password);
                        }
                        return true;
                    case AresId.MSG_CHAT_CLIENT_AUTOLOGIN: {
                            AutoLogin login = (AutoLogin)e.Packet;
                            Commands.HandleAutoLogin(server, this, login.Sha1Password);
                        }
                        return true;
                    case AresId.MSG_CHAT_CLIENT_ADDSHARE:
                        SharedFile addfile = (SharedFile)e.Packet;

                        if (server.PluginManager.OnFileReceived(this, addfile))
                            lock (files) files.Add(addfile);

                        return true;
                    case AresId.MSG_CHAT_CLIENT_IGNORELIST:
                        Ignored ignore = (Ignored)e.Packet;

                        if (ignore.Ignore) {
                            lock (ignored) {
                                if (!ignored.Contains(ignore.Username)) {
                                    ignored.Add(ignore.Username);
                                    server.SendAnnounce(this, String.Format(Strings.Ignored, ignore.Username));
                                }
                            }
                        }
                        else {
                            lock (ignored) {
                                if (ignored.Contains(ignore.Username)) {
                                    ignored.Remove(ignore.Username);
                                    server.SendAnnounce(this, String.Format(Strings.Unignored, ignore.Username));
                                }
                            }
                        }
                        return true;
                    case AresId.MSG_CHAT_CLIENT_UPDATE_STATUS:
                        ClientUpdate update = (ClientUpdate)e.Packet;

                        lastupdate = TimeBank.CurrentTime;

                        NodeIp = update.NodeIp;
                        NodePort = update.NodePort;
                        Age = (update.Age != 0) ? update.Age : Age;
                        Gender = (update.Gender != 0) ? update.Gender : Gender;
                        Country = (update.Country != 0) ? update.Country : Country;
                        Region = !String.IsNullOrEmpty(update.Region) ? update.Region : Region;

                        server.SendPacket((s) => s.Vroom == Vroom, new ServerUpdate(this));
                        return true;
                    case AresId.MSG_CHAT_CLIENT_DIRCHATPUSH:
                        ClientDirectPush push = (ClientDirectPush)e.Packet;

                        if (Encoding.UTF8.GetByteCount(push.Username) < 2) {
                            server.SendPacket(this, new DirectPushError(4));
                            return true;
                        }

                        if (push.TextSync.Length < 16) {
                            server.SendPacket(this, new DirectPushError(3));
                            return true;
                        }

                        IClient target = server.FindUser(s => s.Name == push.Username);

                        if (target == null) {
                            server.SendPacket(this, new DirectPushError(1));
                            return true;
                        }

                        if (target.Ignored.Contains(Name)) {
                            server.SendPacket(this, new DirectPushError(2));
                            return true;
                        }

                        server.SendPacket(this, new DirectPushError(0));
                        server.SendPacket(target, new ServerDirectPush(this, push));

                        return true;
                    case AresId.MSG_CHAT_CLIENT_BROWSE:
                        Browse browse = (Browse)e.Packet;

                        browse.Type = (byte)((browse.Type == 0) ? (byte)255 : browse.Type);
                        browse.Type = (byte)((browse.Type == 8) ? (byte)0 : browse.Type);

                        IClient browse_target = server.FindUser(s => s.Vroom == Vroom && s.Name == browse.Username);

                        if (browse_target == null)
                            return true;

                        else if (browse_target.Files.Count == 0)
                            server.SendPacket(this, new BrowseError(browse.BrowseId));

                        else {
                            server.SendPacket(this, new BrowseStart(browse.BrowseId, (ushort)browse_target.Files.Count));

                            foreach (var file in browse_target.Files) {
                                if (browse.Type == 255 || browse.Type == file.Type)
                                    server.SendPacket(this, new BrowseItem(browse.BrowseId, file));
                            }

                            server.SendPacket(this, new BrowseEnd(browse.BrowseId));
                        }

                        return true;
                    case AresId.MSG_CHAT_CLIENT_SEARCH:
                        Search search = (Search)e.Packet;

                        search.Type = (byte)((search.Type == 0) ? (byte)255 : search.Type);
                        search.Type = (byte)((search.Type == 8) ? (byte)0 : search.Type);

                        foreach (var user in server.Users) {
                            if (user != this && user.Vroom == Vroom) {

                                foreach (var file in user.Files) {
                                    if (file.SearchWords.ContainsAny(search.SearchWords.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries)))
                                        server.SendPacket(this, new SearchHit(search.SearchId, user, file));
                                }
                            }
                        }

                        server.SendPacket(this, new SearchEnd(search.SearchId));
                        return true;
                }

                return false;//wasn't handled
            }
            else return true;//not captcha, not logged, error
        }

        /// <summary>
        /// Packets handled in this function could possibly be overriden by a plugin
        /// </summary>
        internal void HandleOverridePacket(PacketEventArgs e) {

            switch ((AresId)e.Packet.Id) {

                case AresId.MSG_CHAT_CLIENT_PUBLIC:
                    ClientPublic pub = (ClientPublic)e.Packet;

                    if (!String.IsNullOrEmpty(pub.Message)) {

                        server.SendPacket((s) =>
                            s.Vroom == Vroom &&
                           !s.Ignored.Contains(Name), 
                            new ServerPublic(Name, pub.Message));
                    }
                    break;
                case AresId.MSG_CHAT_CLIENT_EMOTE:
                    ClientEmote emote = (ClientEmote)e.Packet;

                    if (!String.IsNullOrEmpty(emote.Message)) {

                        server.SendPacket((s) => 
                            s.Vroom == Vroom &&
                           !s.Ignored.Contains(Name), 
                            new ServerEmote(Name, emote.Message));
                    }
                    break;
                case AresId.MSG_CHAT_CLIENT_PVT: {
                        Private priv = (Private)e.Packet;

                        if (String.IsNullOrEmpty(priv.Message)) return;

                        IClient target = server.FindUser((s) => s.Name == priv.Username);

                        if (target != null) {

                            if (target.Ignored.Contains(Name))
                                server.SendPacket(this, new IgnoringYou(priv.Username));
                            else {
                                priv.Username = Name;
                                server.SendPacket(target, priv);
                            }
                        }
                        else server.SendPacket(this, new Offline(priv.Username));
                    }
                    break;
                case AresId.MSG_CHAT_CLIENT_COMMAND:
                    //Command cmd = (Command)e.Packet;
                    //Not necessary to handle this here anymore
                    break;
                case AresId.MSG_CHAT_CLIENT_PERSONAL_MESSAGE:
                    ClientPersonal personal = (ClientPersonal)e.Packet;
                    Message = personal.Message;
                    break;
                case AresId.MSG_CHAT_CLIENT_AVATAR:
                    ClientAvatar avatar = (ClientAvatar)e.Packet;

                    if (Avatar.Equals(avatar))
                        break;

                    if (avatar.AvatarBytes.Length == 0)
                        Avatar = AresAvatar.Null;
                    else {
                        Avatar = new AresAvatar(avatar.AvatarBytes);

                        if (OrgAvatar == null)
                            OrgAvatar = new AresAvatar(avatar.AvatarBytes);
                    }
                    break;
                case AresId.MSG_CHAT_CLIENT_CUSTOM_DATA: {
                        ClientCustom custom = (ClientCustom)e.Packet;

                        string username = custom.Username;
                        custom.Username = Name;

                        IClient target = server.FindUser((s) => s.Name == username);

                        if (target != null && !target.Ignored.Contains(Name))
                            server.SendPacket(target, custom);
                    }
                    break;
                case AresId.MSG_CHAT_CLIENT_CUSTOM_DATA_ALL:
                    ClientCustomAll customAll = (ClientCustomAll)e.Packet;
                    
                    server.SendPacket((s) =>
                        s != this &&
                        s.Vroom == Vroom,
                        new ClientCustom(Name, customAll));

                    break;
            }
        }

        /*
         * NOTICE -- Occurs before "LoggedIn" is set to true. "AresServer.SendPacket" will fail
         */
        private bool AllowedJoin(IRecord record) {

            if (this.Guid.Equals(Guid.Empty)) {

                socket.SendAsync(new Announce(Errors.InvalidLogin));
                server.PluginManager.OnJoinRejected(this, RejectReason.InvalidLogin);

                Disconnect();
                return false;
            }

            //let onjoincheck run for host but ignore its return
            if (!server.PluginManager.OnJoinCheck(this) && !LocalHost) {

                socket.SendAsync(new Announce(Errors.Rejected));
                server.PluginManager.OnJoinRejected(this, RejectReason.Plugins);

                Disconnect();
                return false;
            }

            // check bans
            if (!LocalHost) {

                if (server.History.Bans.Contains((s) => s.Equals(record))) {

                    socket.SendAsync(new Error(Errors.Banned));
                    server.PluginManager.OnJoinRejected(this, RejectReason.Banned);

                    Disconnect();
                    return false;
                }

                if (server.History.RangeBans.Contains((s) => s.IsMatch(ExternalIp.ToString()))) {

                    socket.SendAsync(new Error(Errors.RangeBanned));
                    server.PluginManager.OnJoinRejected(this, RejectReason.RangeBanned);

                    Disconnect();
                    return false;
                }

                if (DnsEntry != null && server.History.DnsBans.Contains((s) => s.IsMatch(DnsEntry.HostName))) {

                    socket.SendAsync(new Error(Errors.DnsBanned));
                    server.PluginManager.OnJoinRejected(this, RejectReason.DnsBanned);

                    Disconnect();
                    return false;
                }
            }

            //check name hijacking
            int count = 1;
            foreach (var user in server.Users) {
                if (user != this) {

                    if (user.Name == Name) {

                        socket.SendAsync(new Announce(Errors.NameTaken));
                        server.PluginManager.OnJoinRejected(this, RejectReason.NameTaken);

                        Disconnect();
                        return false;
                    }
                    else if (user.ExternalIp.Equals(socket.RemoteEndPoint.Address))
                        count++;
                }

                if (count > server.Config.MaxClones) {

                    socket.SendAsync(new Error(String.Format(Errors.Clones, socket.RemoteEndPoint.Address)));
                    server.PluginManager.OnJoinRejected(this, RejectReason.TooManyBots);

                    Disconnect();
                    return false;
                }
            }

            //Disconnect ghosts after the Name hijack check
            foreach (var user in server.Users)
                if (user != this && user.Guid.Equals(this.Guid))
                    user.Disconnect();

            return true;
        }


        public void Ban() {
            Ban(null);
        }

        public void Ban(Object state) {
            server.Stats.Banned++;
            server.History.Bans.Add(this.ClientId);
            Disconnect(state);
        }


        public void Disconnect() {
            Disconnect(null);
        }

        public void Disconnect(Object state) {
            /*
            if (LoggedIn) {
                logged = false;

                server.History.Add(this);

                server.SendPacket((s) => 
                    s.Vroom == vroom, 
                    new Parted(name));
            }
            */
            if (socket != null)
                socket.Disconnect(state);
        }


        internal void Dispose() {
            LoggedIn = false;

            ((AresTcpSocket)socket).Dispose();

            socket = null;
            guid = Guid.Empty;
            name = null;
            orgname = null;
            version = null;
            region = null;
            message = null;
            avatar = null;
            orgavatar = null;
            localip = null;
            nodeip = null;
            dnsentry = null;

            counters.Clear();
            counters = null;

            files.Clear();
            files = null;
            pfiles = null;

            ignored.Clear();
            ignored = null;

            extended_props.Clear();
            extended_props = null;
        }

        //we don't want a plugin to be able
        //to dispose us directly... bad
        void IDisposable.Dispose() {
            if (Connected) Disconnect();
        }

        public override int GetHashCode() {
            if (cookie == 0)
                cookie = (uint)base.GetHashCode();

            return (int)cookie;
        }
    }
}
