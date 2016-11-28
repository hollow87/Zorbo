using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Zorbo;//some handy extensions and things
using Zorbo.Interface;
using Zorbo.Packets;
using Zorbo.Packets.Ares;

using Jurassic;
using Jurassic.Library;

using Javascript.Objects;

namespace Javascript
{
    public class Jurassic : IPlugin
    {
        string mydir = "";
        IServer server = null;

        static List<Script> scripts = null;
        static Dictionary<string, EmbeddedType> embedded;

        public string Directory {
            get { return mydir; }
            set { mydir = value; }
        }

        internal IServer Server {
            get { return server; }
        }


        internal static Jurassic Self {
            get;
            private set;
        }

        public static List<Script> Scripts {
            get { return scripts; }
        }

        internal static Dictionary<string, EmbeddedType> Embedded {
            get { return embedded; }
        }

        internal sealed class EmbeddedType
        {
            public Type Type { get; set; }
            
            public PropertyAttributes Attributes { get; set; }

            public System.Reflection.ConstructorInfo Ctor { get; set; }

            public EmbeddedType() { }

            public EmbeddedType(Type type, System.Reflection.ConstructorInfo ctor, PropertyAttributes attrs) {
                Type = type;
                Ctor = ctor;
                Attributes = attrs;
            }

            public object Create(Script script) {
                var ptype = Ctor.GetParameters()[0].ParameterType;

                if (ptype == typeof(Script))
                    return Ctor.Invoke(new[] { script });
                else
                    return Ctor.Invoke(new[] { script.Engine });
            }
        }

        static Jurassic() {
            scripts = new List<Script>();
            embedded = new Dictionary<string, EmbeddedType>();
        }

        public Jurassic() { Self = this; }


        public bool EmbedObject(string name, Type prototype, PropertyAttributes attrs) {

            if (!typeof(ObjectInstance).IsAssignableFrom(prototype))
                throw new ArgumentException("Prototype type must inherit from ObjectInstance", "prototype");

            var ctors = prototype.GetConstructors();

            foreach(var ctor in ctors) {

                var @params = ctor.GetParameters();
                if (@params.Length != 1) continue;

                var ptype = @params[0].ParameterType;

                if (ptype != typeof(Script) &&
                    ptype != typeof(ScriptEngine)) continue;

                EmbedRunningScripts(name, new EmbeddedType(prototype, ctor, attrs));
                return true;
            }

            return false;
        }

        private void EmbedRunningScripts(string name, EmbeddedType e) {
            embedded[name] = e;

            foreach (var s in scripts) {
                s.Engine.Global.DefineProperty(
                    name,
                    new PropertyDescriptor(e.Create(s), e.Attributes), true);
            }
        }

        public void OnPluginLoaded(IServer server) {
            this.server = server;
            this.server.SendAnnounce("Jurassic plugin has been loaded!!");
        }

        public void OnPluginKilled() {
            this.server.SendAnnounce("Jurassic plugin has been unloaded!!");
        }


        public void OnCaptcha(IClient client, CaptchaEvent @event) {

            lock (scripts) {
                foreach (var s in scripts) {

                    User user = (User)s.Room.Users.Items.Find((x) => ((User)x).Client == client);
                    if (user == null) continue;

                    try {
                        s.Engine.CallGlobalFunction("onCaptcha", user, (int)@event);
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }
        }

        public ServerFeatures OnSendFeatures(IClient client, ServerFeatures features) {
            return features;
        }

        public void OnSendJoin(IClient client) {

            lock (scripts) {
                foreach (var s in scripts) {

                    User user = (User)s.Room.Users.Items.Find((x) => ((User)x).Client == client);
                    if (user == null) continue;

                    try {
                        s.Engine.CallGlobalFunction("onSendLogin", user);
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }
        }

        public bool OnJoinCheck(IClient client) {
            bool ret = true;

            lock (scripts) {
                foreach (var s in scripts) {
                    User user = new User(s, client);

                    s.Room.Users.Items.Add(user);

                    try {
                        bool b = s.Engine.CallGlobalFunction<bool>("onJoinCheck", user);
                        if (!b) ret = false;
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }

            return ret;
        }

        public void OnJoin(IClient client) {

            lock (scripts) {
                foreach (var s in scripts) {

                    User user = (User)s.Room.Users.Items.Find((x) => ((User)x).Client == client);
                    if (user == null) continue;

                    try {
                        s.Engine.CallGlobalFunction("onJoin", user);
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }
        }

        public void OnJoinRejected(IClient client, RejectReason reason) {

            lock (scripts) {
                foreach (var s in scripts) {

                    int index = s.Room.Users.Items.FindIndex((x) => ((User)x).Client == client);
                    if (index < 0) continue;

                    try {
                        s.Engine.CallGlobalFunction("onJoinRejected", s.Room.Users[index], (int)reason);
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }
        }

        public void OnPart(IClient client, object state) {

            lock (scripts) {
                foreach (var s in scripts) {

                    int index = s.Room.Users.Items.FindIndex((x) => ((User)x).Client == client);
                    if (index < 0) continue;

                    try {
                        s.Engine.CallGlobalFunction("onPart", s.Room.Users.Items[index], state);
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }

                    s.Room.Users.Items.RemoveAt(index);
                }
            }
        }

        public bool OnVroomJoinCheck(IClient client, ushort vroom) {
            bool ret = true;

            lock (scripts) {
                foreach (var s in scripts) {

                    User user = (User)s.Room.Users.Items.Find((x) => ((User)x).Client == client);
                    if (user == null) continue;

                    try {
                        bool b = s.Engine.CallGlobalFunction<bool>("onVroomJoinCheck", user, vroom);
                        if (!b) ret = false;
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }

            return ret;
        }

        public void OnVroomJoin(IClient client) {

            lock (scripts) {
                foreach (var s in scripts) {

                    User user = (User)s.Room.Users.Items.Find((x) => ((User)x).Client == client);
                    if (user == null) continue;

                    try {
                        s.Engine.CallGlobalFunction("onVroomJoin", user);
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }
        }

        public void OnVroomPart(IClient client) {

            lock (scripts) {
                foreach (var s in scripts) {

                    User user = (User)s.Room.Users.Items.Find((x) => ((User)x).Client == client);
                    if (user == null) continue;

                    try {
                        s.Engine.CallGlobalFunction("onVroomPart", user);
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }
        }

        public void OnHelp(IClient client) {
            lock (scripts) {
                foreach (var s in scripts) {

                    User user = (User)s.Room.Users.Items.Find((x) => ((User)x).Client == client);
                    if (user == null) continue;

                    try {
                        s.Engine.CallGlobalFunction("onHelp", user);
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }
        }

        public void OnLogin(IClient client, IPassword password) {
            lock (scripts) {
                foreach (var s in scripts) {

                    User user = (User)s.Room.Users.Items.Find((x) => ((User)x).Client == client);
                    if (user == null) continue;

                    try {
                        s.Engine.CallGlobalFunction<bool>("onLogin", user, new Objects.Password(s, password));
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }
        }

        public bool OnRegister(IClient client, IPassword password) {
            bool ret = true;

            lock (scripts) {
                foreach (var s in scripts) {

                    User user = (User)s.Room.Users.Items.Find((x) => ((User)x).Client == client);
                    if (user == null) continue;

                    try {
                        bool b = s.Engine.CallGlobalFunction<bool>("onRegister", user, new Objects.Password(s, password));
                        if (!b) ret = false;
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }

            return ret;
        }

        public bool OnFileReceived(IClient client, ISharedFile file) {
            bool ret = true;

            /*
            lock (scripts) {
                foreach (var s in scripts) {

                    User user = s.Room.Users.Items.Find((x) => x.Client == client);
                    if (user == null) continue;

                    try {
                        bool b = s.Engine.CallGlobalFunction<bool>("onFileReceived", user, ifile);
                        if (!b) ret = false;
                    }
                    catch (JavaScriptException jex) {
                    }
                }
            }
            */

            return ret;
        }

        public bool OnBeforePacket(IClient client, IPacket packet) {

            switch ((AresId)packet.Id) {
                case AresId.MSG_CHAT_CLIENT_PUBLIC:
                    ClientPublic text = (ClientPublic)packet;

                    if (text.Message.StartsWith("#"))
                        HandleCommand(client, text.Message.Substring(1));

                    break;
                case AresId.MSG_CHAT_CLIENT_COMMAND:

                    Command command = (Command)packet;
                    HandleCommand(client, command.Message);

                    break;
            }

            bool ret = true;

            lock (scripts) {
                foreach (var s in scripts) {

                    User user = (User)s.Room.Users.Items.Find((x) => ((User)x).Client == client);
                    if (user == null) continue;

                    try {
                        bool b = s.Engine.CallGlobalFunction<bool>("onBeforePacket", user, new Packet(s, packet));
                        if (!b) ret = false;
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }

            return ret;
        }

        private void HandleCommand(IClient client, String text) {
            if (client.Admin >= AdminLevel.Admin) {

                if (text.StartsWith("loadscript ") && text.Length > 11)
                    Objects.Script.Load(text.Substring(11));
                
                else if (text.StartsWith("killscript ") && text.Length > 11)
                    Objects.Script.Kill(text.Substring(11));
            }
        }

        public void OnAfterPacket(IClient client, IPacket packet) {

            lock (scripts) {
                foreach (var s in scripts) {

                    User user = (User)s.Room.Users.Items.Find((x) => ((User)x).Client == client);
                    if (user == null) continue;

                    try {
                        s.Engine.CallGlobalFunction("onAfterPacket", user, new Packet(s, packet));
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }
        }

        public bool OnFlood(IClient client, IPacket packet) {
            bool ret = true;

            lock (scripts) {
                foreach (var s in scripts) {
                    try {
                        User user = (User)s.Room.Users.Items.Find((x) => ((User)x).Client == client);
                        if (user == null) continue;

                        bool b = s.Engine.CallGlobalFunction<bool>("onFlood", user, new Packet(s, packet));
                        if (!b) ret = false;
                    }
                    catch (JavaScriptException jex) {
                        OnError(jex);
                    }
                    finally { s.ResetCounters(); }
                }
            }


            return ret;
        }

        public void OnError(IErrorInfo error) {

            if (error.Exception is JavaScriptException) 
                OnError((JavaScriptException)error.Exception);
        }


        internal void OnError(JavaScriptException jex) {

            lock (scripts) {
                foreach (var s in scripts) {
                    try {
                        s.Engine.CallGlobalFunction("onError", new Objects.Error(s, jex));
                    }
                    catch (JavaScriptException) { }
                    finally { s.ResetCounters(); }
                }
            }
        }
    }
}
