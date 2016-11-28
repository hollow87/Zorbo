using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Reflection;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Xml;
using System.Xml.Serialization;

using Zorbo.Interface;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using Zorbo.Resources;

namespace Zorbo.UI
{
    [XmlRoot("Config")]
    public class Config : NotifyObject, IServerConfig
    {
        ushort port = 34567;

        IAvatar avatar = AresAvatar.Null;
        IAvatar orgAvatar = AresAvatar.Null;

        string name = string.Empty;
        string botname = string.Empty;
        string topic = string.Empty;
        string orgTopic = string.Empty;

        uint banlength = 0;

        ushort maxClones = 4;
        ushort maxClients = 500;

        bool autostart = false;
        bool autoload = false;

        bool allowHtml = false;
        bool allowPrivate = true;
        bool allowSharing = true;
        bool allowCompression = false;
        bool allowEncryption = false;
        bool allowVoice = true;
        bool allowOpusVoice = true;

        bool hideIps = true;
        bool muzzledPms = true;
        bool serverbans = false;
        bool botProtection = true;
        bool showOnChannelList = true;

        Language language = Language.English;
        TimeSpan expirepass = TimeSpan.FromDays(90);

        [XmlElement("name")]
        public string Name {
            get { return name; }
            set {
                if (name != value) {
                    name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
        }

        [XmlElement("botname")]
        public string BotName {
            get { return botname; }
            set {
                if (botname != value) {
                    botname = value;
                    RaisePropertyChanged(() => BotName);
                }
            }
        }

        [XmlElement("topic")]
        public string Topic {
            get { return topic; }
            set {
                if (topic != value) {
                    topic = value;

                    if (string.IsNullOrEmpty(OrgTopic))
                        OrgTopic = topic;

                    RaisePropertyChanged(() => Topic);
                }
            }
        }

        [XmlIgnore]
        public string OrgTopic {
            get { return orgTopic; }
            private set {
                if (orgTopic != value) {
                    orgTopic = value;
                    RaisePropertyChanged(() => OrgTopic);
                }
            }
        }

        [XmlElement("port")]
        public ushort Port {
            get { return port; }
            set {
                if (port != value) {
                    port = value;
                    RaisePropertyChanged(() => Port);
                }
            }
        }

        [XmlElement("banlength")]
        public uint BanLength {
            get { return banlength; }
            set {
                if (banlength != value) {
                    banlength = value;
                    RaisePropertyChanged(() => BanLength);
                }
            }
        }

        [XmlIgnore]
        public IAvatar Avatar {
            get { return avatar; }
            set {
                if (avatar != null && avatar.Equals(value))
                    return;
            
                avatar = (value != null) ? value : AresAvatar.Null;

                if (avatar != AresAvatar.Null)
                    OrgAvatar = avatar;

                RaisePropertyChanged(() => Avatar);
            }
        }

        [XmlIgnore]
        public IAvatar OrgAvatar {
            get { return orgAvatar; }
            private set {
                if (orgAvatar != null && orgAvatar.Equals(value))
                    return;

                orgAvatar = (value != null) ? value : AresAvatar.Null;
                RaisePropertyChanged(() => OrgAvatar);
            }
        }

        [XmlElement("maxclones")]
        public ushort MaxClones {
            get { return maxClones; }
            set {
                if (maxClones != value) {
                    maxClones = value;
                    RaisePropertyChanged(() => MaxClones);
                }
            }
        }

        [XmlElement("maxclients")]
        public ushort MaxClients {
            get { return maxClients; }
            set {
                if (maxClients != value) {
                    maxClients = value;
                    RaisePropertyChanged(() => MaxClients);
                }
            }
        }

        [XmlElement("lang")]
        public Language Language {
            get { return language; }
            set {
                if (language != value) {
                    language = value;
                    RaisePropertyChanged(() => Language);
                }
            }
        }

        [XmlIgnore]
        public IEnumerable<Language> LanguageValues {
            get {
                return Enum.GetValues(typeof(Language)).Cast<Language>();
            }
        }

        [XmlElement("html")]
        public bool AllowHtml {
            get { return allowHtml; }
            set {
                if (allowHtml != value) {
                    allowHtml = value;
                    RaisePropertyChanged(() => AllowHtml);
                }
            }
        }

        [XmlElement("private")]
        public bool AllowPrivate {
            get { return allowPrivate; }
            set {
                if (allowPrivate != value) {
                    allowPrivate = value;
                    RaisePropertyChanged(() => AllowPrivate);
                }
            }
        }

        [XmlElement("sharing")]
        public bool AllowSharing {
            get { return allowSharing; }
            set {
                if (allowSharing != value) {
                    allowSharing = value;
                    RaisePropertyChanged(() => AllowSharing);
                }
            }
        }

        [XmlElement("compress")]
        public bool AllowCompression {
            get { return allowCompression; }
            set {
                if (allowCompression != value) {
                    allowCompression = value;
                    RaisePropertyChanged(() => AllowCompression);
                }
            }
        }

        [XmlElement("encrypt")]
        public bool AllowEncryption {
            get { return allowEncryption; }
            set {
                if (allowEncryption != value) {
                    allowEncryption = value;
                    RaisePropertyChanged(() => AllowEncryption);
                }
            }
        }

        [XmlElement("voice")]
        public bool AllowVoice {
            get { return allowVoice; }
            set {
                if (allowVoice != value) {
                    allowVoice = value;
                    RaisePropertyChanged(() => AllowVoice);
                }
            }
        }

        [XmlElement("opus")]
        public bool AllowOpusVoice {
            get { return allowOpusVoice; }
            set {
                if (allowOpusVoice != value) {
                    allowOpusVoice = value;
                    RaisePropertyChanged(() => AllowOpusVoice);
                }
            }
        }

        [XmlElement("hideips")]
        public Boolean HideIPs {
            get { return hideIps; }
            set {
                if (hideIps != value) {
                    hideIps = value;
                    RaisePropertyChanged(() => HideIPs);
                }
            }
        }

        [XmlElement("muzzlepm")]
        public Boolean MuzzledPMs {
            get { return muzzledPms; }
            set {
                if (muzzledPms != value) {
                    muzzledPms = value;
                    RaisePropertyChanged(() => MuzzledPMs);
                }
            }
        }

        [XmlElement("showroom")]
        public Boolean ShowChannel {
            get { return showOnChannelList; }
            set {
                if (showOnChannelList != value) {
                    showOnChannelList = value;
                    RaisePropertyChanged(() => ShowChannel);
                }
            }
        }

        [XmlElement("botprotect")]
        public Boolean BotProtection {
            get { return botProtection; }
            set {
                if (botProtection != value) {
                    botProtection = value;
                    RaisePropertyChanged(() => BotProtection);
                }
            }
        }

        [XmlElement("autostart")]
        public Boolean AutoStartServer {
            get { return autostart; }
            set {
                if (autostart != value) {
                    autostart = value;
                    RaisePropertyChanged(() => AutoStartServer);
                }
            }
        }

        [XmlElement("autoload")]
        public Boolean LoadWithWindows {
            get { return autoload; }
            set {
                if (autoload != value) {
                    autoload = value;
                    CheckRegistry(autoload);
                    RaisePropertyChanged(() => LoadWithWindows);
                }
            }
        }

        [XmlElement("serverbans")]
        public Boolean UseBansToBanServers {
            get { return serverbans; }
            set {
                if (serverbans != value) {
                    serverbans = value;
                    RaisePropertyChanged(() => UseBansToBanServers);
                }
            }
        }

        [XmlElement("passexpire")]
        public Int64 ExpireOldPasswords {
            get { return expirepass.Ticks; }
            set {
                if (expirepass.Ticks != value) {

                    expirepass = TimeSpan.FromTicks(value);
                    RaisePropertyChanged(() => ExpireOldPasswords);
                }
            }
        }

        TimeSpan IServerConfig.ExpireOldPasswords {
            get { return expirepass; }
            set {
                if (!expirepass.Equals(value)) {
                    expirepass = value;
                    RaisePropertyChanged(() => ExpireOldPasswords);
                }
            }
        }

        public Config() { }

        public ServerFeatures GetFeatures() {

            ServerFeatures ret = ServerFeatures.NONE;

            if (AllowPrivate) ret |= ServerFeatures.PRIVATE;
            if (AllowSharing) ret |= ServerFeatures.SHARING;
            if (AllowCompression) ret |= ServerFeatures.COMPRESSION;
            if (AllowVoice) ret |= ServerFeatures.VOICE;
            if (AllowOpusVoice) ret |= ServerFeatures.OPUS_VOICE;
            if (AllowHtml) ret |= ServerFeatures.HTML;

            return ret;
        }

        private void CheckRegistry(bool Enable) {

            RegistryKey rkStartUp = Registry.CurrentUser;
            RegistryKey StartupPath;

            string name = "Zorbo Server";
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AppDomain.CurrentDomain.FriendlyName);

            StartupPath = rkStartUp.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);

            if (Enable && (string)StartupPath.GetValue(name) != path)
                StartupPath.SetValue(name, path, RegistryValueKind.ExpandString);

            else if (!Enable && StartupPath.GetValue(name) != null)
                StartupPath.DeleteValue(name);
        }


        public static Config Load(string directory, string filename) {
            Thread.BeginCriticalRegion();

            Stream stream = null;

            XmlTextReader reader = null;
            XmlSerializer serializer = null;

            Config config = null;
            
            try {
                stream = File.Open(Path.Combine(directory, filename), FileMode.Open, FileAccess.Read);
                reader = new XmlTextReader(stream);

                serializer = new XmlSerializer(typeof(Config));
                config = (Config)serializer.Deserialize(reader);

                if (string.IsNullOrEmpty(config.Name))
                    config.Name = Strings.DefaultName;

                if (string.IsNullOrEmpty(config.BotName))
                    config.BotName = Strings.DefaultBotName;

                if (string.IsNullOrEmpty(config.Topic))
                    config.Topic = Strings.DefaultTopic;

                config.Avatar = LoadAvatar(directory);
                config.CheckRegistry(config.LoadWithWindows);
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception) {

                config = new Config();
                config.Avatar = LoadAvatar(directory);

                try { File.Delete(filename); }
                catch { }
            }
            finally {
                if (reader != null)
                    reader.Close();

                if (stream != null) {
                    stream.Close();
                    stream.Dispose();
                }
            }

            Thread.EndCriticalRegion();
            return config;
        }

        private static IAvatar LoadAvatar(string directory) {

            byte[] small = null;
            byte[] large = null;

            string smallpath = Path.Combine(directory, "avatar_small.jpg");
            string largepath = Path.Combine(directory, "avatar_large.jpg");

            if (File.Exists(smallpath)) {
                small = File.ReadAllBytes(smallpath);

                if (File.Exists(largepath)) {
                    large = File.ReadAllBytes(largepath);
                    return new AresAvatar(small, large);
                }
                else return new AresAvatar(small);
            }
            else return new AresAvatar(new byte[0]);
        }


        public static void Save(Config config, string filename) {

            if (config == null)
                throw new ArgumentNullException("config");

            Thread.BeginCriticalRegion();

            Stream stream = null;

            XmlTextWriter writer = null;
            XmlSerializer serializer = null;

            try {
                FileInfo file = new FileInfo(filename);

                if (config.Avatar != null && config.Avatar.SmallBytes.Length > 0) {
                    File.WriteAllBytes(Path.Combine(file.Directory.FullName, "avatar_small.jpg"), config.Avatar.SmallBytes);
                    File.WriteAllBytes(Path.Combine(file.Directory.FullName, "avatar_large.jpg"), config.Avatar.LargeBytes);
                }

                stream = file.Open(FileMode.Create, FileAccess.Write);
                writer = new XmlTextWriter(stream, Encoding.UTF8);

                writer.Formatting = Formatting.Indented;
                writer.Indentation = 2;
                writer.IndentChar = '\x20';

                serializer = new XmlSerializer(typeof(Config));
                serializer.Serialize(writer, config);

                writer.Flush();
            }
            catch (UnauthorizedAccessException) { }
            finally {
                if (writer != null)
                    writer.Close();

                if (stream != null) {
                    stream.Close();
                    stream.Dispose();
                }
            }

            Thread.EndCriticalRegion();
        }
    }
}
