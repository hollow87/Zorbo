using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

using Jurassic.Library;

using Zorbo;//some handy extensions and things
using Zorbo.Interface;
using Zorbo.Packets;
using Zorbo.Packets.Ares;

using cb0tProtocol.Packets;

namespace cb0tProtocol
{
    public class cb0tProtocol : IPlugin
    {
        string mydir = "";
        IServer server = null;

        IFormatter formatter = null;

        static cb0tProtocol self = null;

        public string Directory {
            get { return mydir; }
            set { mydir = value; }
        }

        internal IServer Server {
            get { return server; }
        }

        internal static cb0tProtocol Self {
            get { return self; }
        }

        public void OnPluginLoaded(IServer server) {
            self = this;
            this.server = server;
            this.server.PluginHost.Loaded += Host_LoadedPlugin;

            this.formatter = new AdvancedFormatter();

            foreach (var user in server.Users) {
                user.Extended["CustomFont"] = null;
                user.Extended["SupportEmote"] = false;
                user.Extended["VoiceIgnore"] = new List<String>();
                user.Extended["CustomEmote"] = new List<ClientEmoteItem>();
            }

            foreach (var user in server.Users)
                OnSendJoin(user);

            foreach(var plugin in server.PluginHost)
                Host_LoadedPlugin(server.PluginHost, plugin);

            this.server.SendAnnounce("cb0tProtocol plugin has been loaded!!");
        }

        //Bit of a CLR hack to superimpose new Javascript objects into the Zorbo Javascript Plugin. 
        //this requires references to Jurassic.dll as well as the plugin (Javascript.dll)
        private void Host_LoadedPlugin(object sender, ILoadedPlugin plugin) {
            if (plugin.Plugin is Javascript.Jurassic) {
                var js = (Javascript.Jurassic)plugin.Plugin;

                js.EmbedObject("Scribble", typeof(Scribble.Constructor), PropertyAttributes.Configurable);
            }
        }

        public void OnPluginKilled() {
            this.server.PluginHost.Loaded -= Host_LoadedPlugin;

            // Send packets to inform cb0t we've stopped supporting the advanced features
            var voice_support = new Advanced(new ServerVoiceSupport() {
                Enabled = false,
                HighQuality = false
            });

            foreach (var user in server.Users) {

                if (user.LoggedIn) {

                    user.SendPacket(voice_support);

                    bool pubvoice = (user.Features & ClientFeatures.VOICE) == ClientFeatures.VOICE;
                    bool privoice = (user.Features & ClientFeatures.PRIVATE_VOICE) == ClientFeatures.PRIVATE_VOICE;

                    if (pubvoice || privoice) {

                        server.SendPacket(new Advanced(new ServerVoiceSupportUser() {
                            Username = user.Name,
                            Public = false,
                            Private = false,
                        }));
                    }

                    if ((bool)user.Extended["SupportEmote"]) {

                        server.SendPacket(new Advanced(new ServerEmoteSupport(0)));

                        foreach (var emote in (List<ClientEmoteItem>)user.Extended["CustomEmote"])
                            server.SendPacket(new Advanced(new ServerEmoteDelete() {
                                Username = user.Name,
                                Shortcut = emote.Shortcut,
                            }));
                    }
                }

                user.Extended.Remove("CustomFont");
                user.Extended.Remove("SupportEmote");
                user.Extended.Remove("VoiceIgnore");
                user.Extended.Remove("CustomEmote");
            }

            this.server.SendAnnounce("cb0tProtocol plugin has been unloaded!!");
        }


        public void OnCaptcha(IClient client, CaptchaEvent @event) {
        }

        public ServerFeatures OnSendFeatures(IClient client, ServerFeatures features) {

            if ((features & ServerFeatures.ROOM_SCRIBBLES) != ServerFeatures.ROOM_SCRIBBLES) {
                features |= ServerFeatures.ROOM_SCRIBBLES;
            }

            if ((features & ServerFeatures.PRIVATE_SCRIBBLES) != ServerFeatures.PRIVATE_SCRIBBLES) {
                features |= ServerFeatures.PRIVATE_SCRIBBLES;
            }

            return features;
        }

        public void OnSendJoin(IClient client) {

            var features = server.Config.GetFeatures();

            bool voice = (features & ServerFeatures.VOICE) == ServerFeatures.VOICE;
            bool opus = (features & ServerFeatures.OPUS_VOICE) == ServerFeatures.OPUS_VOICE;

            if (voice || opus) {
                client.SendPacket(new Advanced(new ServerVoiceSupport() {
                    Enabled = true,
                    HighQuality = opus,
                }));
            }

            client.SendPacket(new Advanced(new ServerEmoteSupport(16)));

            for (int i = 0; i < server.Users.Count; i++) {
                IClient target = server.Users[i];

                if (target != client && 
                    target.LoggedIn &&
                    target.Vroom == client.Vroom) {

                    var font = (ServerFont)target.Extended["CustomFont"];
                    if (font != null) client.SendPacket(font);

                    if (voice || opus) {

                        bool pubvoice = (target.Features & ClientFeatures.VOICE) == ClientFeatures.VOICE;
                        bool privoice = (target.Features & ClientFeatures.PRIVATE_VOICE) == ClientFeatures.PRIVATE_VOICE;

                        if (pubvoice || privoice) {
                            client.SendPacket(new Advanced(new ServerVoiceSupportUser() {
                                Username = target.Name,
                                Public = pubvoice,
                                Private = privoice,
                            }));
                        }
                    }
                }
            }
        }

        public bool OnJoinCheck(IClient client) {

            client.Extended["CustomFont"] = null;
            client.Extended["SupportEmote"] = false;
            client.Extended["VoiceIgnore"] = new List<String>();
            client.Extended["CustomEmote"] = new List<ClientEmoteItem>();

            return true;
        }

        public void OnJoinRejected(IClient client, RejectReason reason) {
        }

        public void OnJoin(IClient client) {
            //Support features sent in OnSendLogin
        }

        public void OnPart(IClient client, Object state) {
        }

        public bool OnVroomJoinCheck(IClient client, UInt16 vroom) {
            return true;
        }

        public void OnVroomJoin(IClient client) {
        }

        public void OnVroomPart(IClient client) {
        }

        public void OnHelp(IClient client) {
        }

        public void OnLogin(IClient client, IPassword password) {
        }

        public bool OnRegister(IClient client, IPassword password) {
            return true;
        }

        public bool OnFileReceived(IClient client, ISharedFile file) {
            return true;
        }

        public bool OnBeforePacket(IClient client, IPacket packet) {

            if ((AresId)packet.Id == AresId.MSG_CHAT_CLIENT_CUSTOM_DATA) {

                var custom = (ClientCustom)packet;

                if (custom.Username == server.Config.BotName && 
                    custom.CustomId.StartsWith("cb0t_scribble")) {

                    custom.Username = client.Name;
                    server.SendPacket((s) => s != client && s.Vroom == client.Vroom, custom);
                }
            }

            return true;
        }

        public void OnAfterPacket(IClient client, IPacket packet) {

            switch((AresId)packet.Id) {
                case AresId.MSG_CHAT_CLIENT_PUBLIC:
                    ClientPublic pub = (ClientPublic)packet;
                    if (pub.Message.StartsWith("#"))
                        OnCommand(client, pub.Message.Substring(1));
                    break;
                case AresId.MSG_CHAT_CLIENT_COMMAND:
                    Command cmd = (Command)packet;
                    OnCommand(client, cmd.Message);
                    break;
            }
            
            if ((AdvancedId)packet.Id == AdvancedId.MSG_CHAT_ADVANCED_FEATURES_PROTOCOL) {

                Unknown unknown = (Unknown)packet;

                ushort length = BitConverter.ToUInt16(unknown.Payload, 0);
                IPacket advanced = formatter.Unformat(unknown.Payload[2], unknown.Payload, 3, length);

                switch ((AdvancedId)advanced.Id) {
                    case AdvancedId.MSG_CHAT_CLIENT_CUSTOM_ADD_TAGS:
                        break;
                    case AdvancedId.MSG_CHAT_CLIENT_CUSTOM_REM_TAGS:
                        break;
                    case AdvancedId.MSG_CHAT_CLIENT_CUSTOM_FONT:
                        ClientFont font = (ClientFont)advanced;

                        ServerFont userfont = (ServerFont)client.Extended["CustomFont"];

                        if (userfont == null) {
                            userfont = new ServerFont();
                            client.Extended["CustomFont"] = userfont;
                        }

                        userfont.Username = client.Name;
                        userfont.Size = font.Size;
                        userfont.Name = font.Name;
                        userfont.NameColor = font.NameColor;
                        userfont.TextColor = font.TextColor;
                        userfont.NameColor2 = font.NameColor2;
                        userfont.TextColor2 = font.TextColor2;

                        if (string.IsNullOrEmpty(userfont.NameColor2))
                            userfont.NameColor2 = userfont.NameColor.ToHtmlColor();

                        if (string.IsNullOrEmpty(userfont.TextColor2))
                            userfont.TextColor2 = userfont.TextColor.ToHtmlColor();

                        Advanced send = new Advanced(userfont);

                        server.SendPacket((s) => s.Vroom == client.Vroom, send);

                        if (String.IsNullOrWhiteSpace(font.Name))
                            client.Extended["CustomFont"] = null;

                        break;
                    case AdvancedId.MSG_CHAT_CLIENT_VC_SUPPORTED:
                        ClientVoiceSupport vcs = (ClientVoiceSupport)advanced;

                        if (vcs.Public)
                            client.Features |= ClientFeatures.VOICE;

                        if (vcs.Private)
                            client.Features |= ClientFeatures.PRIVATE_VOICE;

                        server.SendPacket((s) =>
                            s.Vroom == client.Vroom,
                            new Advanced(new ServerVoiceSupportUser() {
                                Username = client.Name,
                                Public = vcs.Public,
                                Private = vcs.Private,
                            }));
                        break;
                    case AdvancedId.MSG_CHAT_CLIENT_VC_FIRST:
                        ClientVoiceFirst vcf = (ClientVoiceFirst)advanced;

                        server.SendPacket((s) =>
                            (s.Vroom == client.Vroom) &&
                            (s.Features & ClientFeatures.VOICE) == ClientFeatures.VOICE &&
                            !((List<String>)s.Extended["VoiceIgnore"]).Contains(client.Name),

                            new Advanced(new ServerVoiceFirst() {
                                Username = client.Name,
                                Chunk = vcf.Chunk
                            }));

                        break;
                    case AdvancedId.MSG_CHAT_CLIENT_VC_FIRST_TO: {

                            ClientVoiceFirstTo vcf2 = (ClientVoiceFirstTo)advanced;

                            IClient target = server.FindUser((s) => 
                                s.Name == vcf2.Username &&
                                s.Vroom == client.Vroom);

                            if (target == null) return;

                            if ((client.Features & ClientFeatures.PRIVATE_VOICE) != ClientFeatures.PRIVATE_VOICE)
                                client.SendPacket(new Advanced(new ServerVoiceNoPrivate() {
                                    Username = target.Name
                                }));

                            else if (((List<String>)target.Extended["VoiceIgnore"]).Contains(client.Name))
                                client.SendPacket(new Advanced(new ServerVoiceIgnore() {
                                    Username = target.Name
                                }));

                            else target.SendPacket(new Advanced(new ServerVoiceFirstFrom() {
                                Username = client.Name,
                                Chunk = vcf2.Chunk
                            }));
                        }
                        break;
                    case AdvancedId.MSG_CHAT_CLIENT_VC_CHUNK:
                        ClientVoiceChunk vcc = (ClientVoiceChunk)advanced;

                        server.SendPacket((s) =>
                            (s.Vroom == client.Vroom) &&
                            (s.Features & ClientFeatures.VOICE) == ClientFeatures.VOICE &&
                            !((List<String>)s.Extended["VoiceIgnore"]).Contains(client.Name),

                            new Advanced(new ServerVoiceChunk() {
                                Username = client.Name,
                                Chunk = vcc.Chunk
                            }));

                        break;
                    case AdvancedId.MSG_CHAT_CLIENT_VC_CHUNK_TO: {

                            ClientVoiceChunkTo vcc2 = (ClientVoiceChunkTo)advanced;

                            IClient target = server.FindUser((s) => 
                                s.Name == vcc2.Username &&
                                s.Vroom == client.Vroom);

                            if (target == null) return;

                            if ((client.Features & ClientFeatures.PRIVATE_VOICE) == ClientFeatures.PRIVATE_VOICE)
                                client.SendPacket(new Advanced(new ServerVoiceNoPrivate() {
                                    Username = target.Name
                                }));

                            else if (((List<String>)target.Extended["VoiceIgnore"]).Contains(client.Name))
                                client.SendPacket(new Advanced(new ServerVoiceIgnore() {
                                    Username = target.Name
                                }));

                            else target.SendPacket(new Advanced(new ServerVoiceFirstFrom() {
                                Username = client.Name,
                                Chunk = vcc2.Chunk
                            }));
                        }
                        break;
                    case AdvancedId.MSG_CHAT_CLIENT_VC_IGNORE:
                        ClientVoiceIgnore vci = (ClientVoiceIgnore)advanced;
                        List<String> ignores = (List<String>)client.Extended["VoiceIgnore"];

                        if (ignores.Contains(vci.Username)) {

                            ignores.RemoveAll((s) => s == vci.Username);
                            server.SendAnnounce(client, String.Format("You are now allowing voice chat from {0}", vci.Username));
                        }
                        else {
                            ignores.Add(vci.Username);
                            server.SendAnnounce(client, String.Format("You are now ignoring voice chat from {0}", vci.Username));
                        }
                        break;
                    case AdvancedId.MSG_CHAT_CLIENT_SUPPORTS_CUSTOM_EMOTES:
                        client.Extended["SupportEmote"] = true;

                        foreach (var user in server.Users) {
                            if (user.Vroom == client.Vroom) {

                                var emotes = (List<ClientEmoteItem>)client.Extended["CustomEmote"];

                                if (emotes.Count > 0) {
                                    foreach (var emote in emotes)
                                        client.SendPacket(new Advanced(new ServerEmoteItem() {
                                            Username = user.Name,
                                            Shortcut = emote.Shortcut,
                                            Size = emote.Size,
                                            Image = emote.Image,
                                        }));
                                }
                            }
                        }
                        break;
                    case AdvancedId.MSG_CHAT_SERVER_CUSTOM_EMOTES_ITEM: {
                            client.Extended["SupportEmote"] = true;
                            ClientEmoteItem item = (ClientEmoteItem)advanced;

                            ((List<ClientEmoteItem>)client.Extended["CustomEmote"]).Add(item);

                            if (client.Cloaked) {
                                server.SendPacket((s) =>
                                    s.Admin >= client.Admin &&
                                    s.Vroom == client.Vroom &&
                              (bool)s.Extended["SupportEmote"],
                                    new Advanced(new ServerEmoteItem() {
                                        Username = client.Name,
                                        Shortcut = item.Shortcut,
                                        Size = item.Size,
                                        Image = item.Image,
                                    }));
                            }
                            else {
                                server.SendPacket((s) =>
                                    s.Vroom == client.Vroom &&
                              (bool)s.Extended["SupportEmote"],
                                    new Advanced(new ServerEmoteItem() {
                                        Username = client.Name,
                                        Shortcut = item.Shortcut,
                                        Size = item.Size,
                                        Image = item.Image,
                                    }));
                            }
                        }
                        break;
                    case AdvancedId.MSG_CHAT_CLIENT_CUSTOM_EMOTE_DELETE: {
                            ClientEmoteDelete item = (ClientEmoteDelete)advanced;

                            var emotes = ((List<ClientEmoteItem>)client.Extended["CustomEmote"]);
                            int index = emotes.FindIndex(s => s.Shortcut == item.Shortcut);

                            if (index > -1) {
                                emotes.RemoveAt(index);

                                server.SendPacket((s) =>
                                    s.Vroom == client.Vroom &&
                              (bool)s.Extended["SupportEmote"],
                                    new Advanced(new ServerEmoteDelete() {
                                        Username = client.Name,
                                        Shortcut = item.Shortcut,
                                    }));
                            }
                        }
                        break;
                    case AdvancedId.MSG_CHAT_CLIENT_ROOM_SCRIBBLE_FIRST:
                        OnScribbleFirst(client, (ClientScribbleFirst)advanced);
                        break;
                    case AdvancedId.MSG_CHAT_CLIENT_ROOM_SCRIBBLE_CHUNK:
                        OnScribbleChunk(client, (ClientScribbleChunk)advanced);
                        break;
                }
            }
        }

        private void OnCommand(IClient client, string text) {

            if (text.Length > 9 && text.Substring(0, 9) == "scribble ") {
                Uri uri = null;
                var scribble = RoomScribble.GetScribble(client);
                if (Uri.TryCreate(text.Substring(9), UriKind.Absolute, out uri)) {
                    if (uri.IsFile) {
                        server.SendAnnounce("Not valid");
                    }
                    else {
                        scribble.Download(uri, (s) => {
                            SendRoomScribble((c) => c.Vroom == client.Vroom, client.Name, scribble);
                        }, null);
                    }
                }
            }

        }

        private void OnScribbleFirst(IClient client, ClientScribbleFirst first) {
            var scribble = RoomScribble.GetScribble(client);

            scribble.Reset();
            scribble.Size = first.Size;
            scribble.Chunks = (ushort)(first.Chunks + 1);//scribble object counts first chunk
            scribble.Write(first.Data);

            if (scribble.IsComplete) SendRoomScribble(client, scribble);
        }

        private void OnScribbleChunk(IClient client, ClientScribbleChunk chunk) {
            var scribble = RoomScribble.GetScribble(client);

            scribble.Write(chunk.Data);

            if (scribble.IsComplete) SendRoomScribble(client, scribble);
        }

        //Used for Sending scribble objects from another client

        private void SendRoomScribble(IClient client, RoomScribble scribble) {

            SendRoomScribble(
                (s) => s != client && s.Vroom == client.Vroom,
                client.Name,
                scribble);
        }

        //Used for Sending scribble objects from javascript

        internal void SendRoomScribble(string name, RoomScribble scribble) {
            scribble.Index = 0;

            int length = Math.Min((int)scribble.Received, 4000);
            byte[] buffer = scribble.Read();

            server.SendAnnounce(string.Format("\x000314--- From {0}", name));
            server.SendPacket(new ClientCustom(server.Config.BotName, "cb0t_scribble_first", buffer));

            while (scribble.Remaining > 0) {
                buffer = scribble.Read();

                if (scribble.Remaining > 0)
                    server.SendPacket(new ClientCustom(server.Config.BotName, "cb0t_scribble_chunk", buffer));
                else
                    server.SendPacket(new ClientCustom(server.Config.BotName, "cb0t_scribble_last", buffer));
            }
        }

        internal void SendRoomScribble(Predicate<IClient> pred, string name, RoomScribble scribble) {
            scribble.Index = 0;

            int length = Math.Min((int)scribble.Received, 4000);
            byte[] buffer = scribble.Read();

            server.SendAnnounce(pred, string.Format("\x000314--- From {0}", name));
            server.SendPacket(pred, new ClientCustom(server.Config.BotName, "cb0t_scribble_first", buffer));

            while (scribble.Remaining > 0) {
                buffer = scribble.Read();

                if (scribble.Remaining > 0)
                    server.SendPacket(pred, new ClientCustom(server.Config.BotName, "cb0t_scribble_chunk", buffer));
                else
                    server.SendPacket(pred, new ClientCustom(server.Config.BotName, "cb0t_scribble_last", buffer));
            }
        }

        public bool OnFlood(IClient client, IPacket packet) {
            return true;
        }

        public void OnError(IErrorInfo error) {
            server.SendAnnounce(error.Name + " has caused: " + error.Exception.Message);
        }
    }
}
