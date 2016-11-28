using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo;
using Zorbo.Interface;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class Config : ObjectInstance
    {
        JScript script = null;

        Avatar avatar = null;
        Avatar orgAvatar = null;

        IServerConfig config = null;

        protected override string InternalClassName {
            get { return "Config"; }
        }

        public Config(JScript script, IServerConfig config)
            : base(script.Engine) {

            this.config = config;
            this.script = script;

            this.PopulateFunctions();
        }

        [JSProperty(Name = "name")]
        public string Name {
            get { return config.Name; }
        }

        [JSProperty(Name = "botName")]
        public string BotName {
            get { return config.BotName; }
        }

        [JSProperty(Name = "topic")]
        public string Topic {
            get { return config.Topic; }
            set { config.Topic = value; }
        }

        [JSProperty(Name = "orgTopic")]
        public string OrgTopic {
            get { return config.OrgTopic; }
        }

        [JSProperty(Name = "port")]
        public int Port {
            get { return (int)config.Port; }
        }

        [JSProperty(Name = "language")]
        public int Language {
            get { return (int)config.Language; }
            set { config.Language = (Language)value; }
        }

        [JSProperty(Name = "avatar")]
        public Avatar Avatar {
            get {
                if (avatar == null)//IAvatar might not be a JS object
                    avatar = new Avatar(script, config.Avatar);

                return avatar;
            }
            set {
                avatar = value;
                config.Avatar = value; 
            }
        }

        [JSProperty(Name = "orgAvatar")]
        public Avatar OrgAvatar {
            get {
                if (orgAvatar == null)
                    orgAvatar = new Avatar(script, config.OrgAvatar);

                return orgAvatar;
            }
        }

        [JSProperty(Name = "botProtection")]
        public bool BotProtection {
            get { return config.BotProtection; }
            set { config.BotProtection = value; }
        }

        [JSProperty(Name = "allowHtml")]
        public bool AllowHtml {
            get { return config.AllowHtml; }
            set { config.AllowHtml = value; }
        }

        [JSProperty(Name = "allowPrivate")]
        public bool AllowPrivate {
            get { return config.AllowPrivate; }
            set { config.AllowPrivate = value; }
        }

        [JSProperty(Name = "allowSharing")]
        public bool AllowSharing {
            get { return config.AllowSharing; }
            set { config.AllowSharing = value; }
        }

        [JSProperty(Name = "allowCompression")]
        public bool AllowCompression {
            get { return config.AllowCompression; }
            set { config.AllowCompression = value; }
        }

        [JSProperty(Name = "allowEncryption")]
        public bool AllowEncryption {
            get { return config.AllowEncryption; }
            set { config.AllowEncryption = value; }
        }

        [JSProperty(Name = "allowVoice")]
        public bool AllowVoice {
            get { return config.AllowVoice; }
            set { config.AllowVoice = value; }
        }

        [JSProperty(Name = "allowOpusVoice")]
        public bool AllowOpusVoice {
            get { return config.AllowOpusVoice; }
            set { config.AllowOpusVoice = value; }
        }

        [JSProperty(Name = "muzzledPMs")]
        public bool MuzzledPMs {
            get { return config.MuzzledPMs; }
            set { config.MuzzledPMs = value; }
        }

        [JSProperty(Name = "maxClones")]
        public int MaxClones {
            get { return config.MaxClones; }
            set { config.MaxClones = (ushort)value; }
        }

        [JSProperty(Name = "maxClients")]
        public int MaxClients {
            get { return config.MaxClients; }
        }

        [JSProperty(Name = "expireOldPasswords")]
        public double ExpireOldPasswords {
            get { return config.ExpireOldPasswords.TotalDays; }
            set { config.ExpireOldPasswords = TimeSpan.FromDays(value); }
        }

        [JSProperty(Name = "showChannel")]
        public bool ShowChannel {
            get { return config.ShowChannel; }
            set { config.ShowChannel = value; }
        }

        [JSProperty(Name = "useBansToBanServers")]
        public bool UseChatroomBansToBanServers {
            get { return config.UseBansToBanServers; }
            set { config.UseBansToBanServers = value; }
        }
    }
}
