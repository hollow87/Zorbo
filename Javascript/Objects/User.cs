using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Zorbo.Interface;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class User : ObjectInstance
    {
        JScript script = null;
        IClient user = null;

        Avatar avatar = null;
        Avatar orgavatar = null;

        Monitor monitor = null;

        public IClient Client {
            get { return user; }
        }

        #region " Constructor "

        public class Constructor : ClrFunction
        {
            JScript script = null;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "User", new User(script)) {

                this.script = script;
            }

            [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public User Call(object a) {
                return null;
            }

            [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public User Construct(object a) {
                return null;
            }
        }

        #endregion

        protected override string InternalClassName {
            get { return "User"; }
        }


        private User(JScript script)
            : base(script.Engine) {

            this.script = script;
            this.user = new UserProto();

            this.PopulateFunctions();
        }

        public User(JScript script, IClient client)
            : base(script.Engine, ((ClrFunction)script.Engine.Global["User"]).InstancePrototype) {

            this.script = script;
            this.user = client;
            this.monitor = new Monitor(script, client.Monitor);

            this.PopulateFunctions();
        }

        [JSProperty(Name = "id")]
        public int Id {
            get { return user.Id; }
        }

        [JSProperty(Name = "guid")]
        public string Guid {
            get { return user.Guid.ToString(); }
        }
        
        [JSProperty(Name = "admin")]
        public int Admin {
            get { return (int)user.Admin; }
            set { user.Admin = (AdminLevel)value; }
        }

        [JSProperty(Name = "name")]
        public string Name {
            get { return user.Name; }
            set { user.Name = value; }
        }

        [JSProperty(Name = "orgName")]
        public string OrgName {
            get { return user.OrgName; }
        }

        [JSProperty(Name = "loggedIn")]
        public bool LoggedIn {
            get { return user.LoggedIn; }
        }

        [JSProperty(Name = "connected")]
        public bool Connected {
            get { return user.Connected; }
        }

        [JSProperty(Name = "localHost")]
        public bool LocalHost {
            get { return user.LocalHost; }
        }

        [JSProperty(Name = "fastPing")]
        public bool FastPing {
            get { return user.FastPing; }
        }

        [JSProperty(Name = "isCaptcha")]
        public bool IsCaptcha {
            get { return user.IsCaptcha; }
            set { user.IsCaptcha = value; }
        }

        [JSProperty(Name = "muzzled")]
        public bool Muzzled {
            get { return user.Muzzled; }
            set { user.Muzzled = value; }
        }

        [JSProperty(Name = "cloaked")]
        public bool Cloaked {
            get { return user.Cloaked; }
            set { user.Cloaked = value; }
        }

        [JSProperty(Name = "browsable")]
        public bool Browsable {
            get { return user.Browsable; }
        }

        [JSProperty(Name = "encryption")]
        public bool Encryption {
            get { return user.Encryption; }
        }

        [JSProperty(Name = "compression")]
        public bool Compression {
            get { return user.Compression; }
        }

        [JSProperty(Name = "html")]
        public bool SupportHtml {
            get { return (user.Features & ClientFeatures.HTML) == ClientFeatures.HTML; }
        }

        [JSProperty(Name = "voice")]
        public bool SupportVoice {
            get { return (user.Features & ClientFeatures.VOICE) == ClientFeatures.VOICE; }
        }

        [JSProperty(Name = "privateVoice")]
        public bool SupportPrivateVoice {
            get { return (user.Features & ClientFeatures.PRIVATE_VOICE) == ClientFeatures.PRIVATE_VOICE; }
        }

        [JSProperty(Name = "cookie")]
        public int Cookie {
            get { return (int)user.Cookie; }
        }

        [JSProperty(Name = "age")]
        public int Age {
            get { return user.Age; }
        }

        [JSProperty(Name = "gender")]
        public int Sex {
            get { return (int)user.Gender; }
        }

        [JSProperty(Name = "country")]
        public int Country {
            get { return (int)user.Country; }
        }

        [JSProperty(Name = "vroom")]
        public int Vroom {
            get { return user.Vroom; }
            set { user.Vroom = (ushort)Convert.ToUInt16(value); }
        }

        [JSProperty(Name = "fileCount")]
        public int FileCount {
            get { return user.FileCount; }
        }

        [JSProperty(Name = "nodePort")]
        public int NodePort {
            get { return user.NodePort; }
        }

        [JSProperty(Name = "listenPort")]
        public int ListenPort {
            get { return user.ListenPort; }
        }

        [JSProperty(Name = "nodeIp")]
        public string NodeIp {
            get { return user.NodeIp.ToString(); }
        }

        [JSProperty(Name = "localIp")]
        public string LocalIp {
            get { return user.LocalIp.ToString(); }
        }

        [JSProperty(Name = "externalIp")]
        public string ExternalIp {
            get { return user.ExternalIp.ToString(); }
        }

        [JSProperty(Name = "dnsEntry")]
        public string DnsEntry {
            get { return (user.DnsEntry != null) ? user.DnsEntry.HostName : "error"; }
        }

        [JSProperty(Name = "features")]
        public int Features {
            get { return (int)user.Features; }
        }

        [JSProperty(Name = "avatar")]
        public Avatar Avatar {
            get {
                if (avatar == null)
                    avatar = new Avatar(script, user.Avatar);

                return avatar;
            }
            set {
                this.avatar = value;
                user.Avatar = value;
            }
        }

        [JSProperty(Name = "orgAvatar")]
        public Avatar OrgAvatar {
            get {
                if (orgavatar == null && user.OrgAvatar != null)
                    orgavatar = new Avatar(script, user.OrgAvatar);

                return orgavatar;
            }
        }

        [JSProperty(Name = "version")]
        public string Version {
            get { return user.Version; }
        }

        [JSProperty(Name = "region")]
        public string Region {
            get { return user.Region; }
        }

        [JSProperty(Name = "message")]
        public string Message {
            get { return user.Message; }
            set { user.Message = value; }
        }

        [JSProperty(Name = "monitor")]
        public Monitor Monitor {
            get { return this.monitor; }
        }

        [JSFunction(Name = "ban", IsEnumerable = true, IsWritable = false)]
        public void Ban(object a) {
            object state = null;

            if (!(a is Undefined) && !(a is Null))
                state = a;
            
            user.Ban(state);
        }

        [JSFunction(Name = "disconnect", IsEnumerable = true, IsWritable = false)]
        public void Disconnect(object a) {
            object state = null;

            if (!(a is Undefined) && !(a is Null))
                state = a;
            
            user.Disconnect(state);
        }
    }
}
