using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;

using Jurassic;
using Jurassic.Library;

using Javascript.Objects;
using Zorbo.Packets;

namespace Javascript
{
    public class Script
    {
        Room room;
        ScriptEngine engine;

        Dictionary<String, Int32> counters;
        static StringBuilder default_script;


        public string Name {
            get;
            private set;
        }

        public string Directory {
            get { return Path.Combine(Jurassic.Self.Directory, "Scripts", Name); }
        }

        public ScriptEngine Engine {
            get { return engine; }
        }

        internal Room Room {
            get { return room; }
        }

        internal Dictionary<String, Int32> Counters {
            get { return counters; }
        }

        static Script() {
            default_script = new StringBuilder();
            default_script.AppendLine("function onCaptcha(userobj, event) { }");
            default_script.AppendLine("function onSendLogin(userobj) { }");
            default_script.AppendLine("function onJoinCheck(userobj) { return true;}");
            default_script.AppendLine("function onJoinRejected(userobj, reason) { }");
            default_script.AppendLine("function onJoin(userobj) { }");
            default_script.AppendLine("function onPart(userobj, state) { }");
            default_script.AppendLine("function onVroomJoinCheck(userobj) { return true;}");
            default_script.AppendLine("function onVroomJoin(userobj) { }");
            default_script.AppendLine("function onVroomPart(userobj) { }");
            default_script.AppendLine("function onHelp(userobj) { }");
            default_script.AppendLine("function onLogin(userobj, passobj) { }");
            default_script.AppendLine("function onRegister(userobj, passobj) { return true;}");
            default_script.AppendLine("function onFileReceived(userobj, file) { return true;}");
            default_script.AppendLine("function onBeforePacket(userobj, packet) { return true;}");
            default_script.AppendLine("function onAfterPacket(userobj, packet) { }");
            default_script.AppendLine("function onFlood(userobj, packet) { return true;}");
            default_script.AppendLine("function onError(error) { }");
        }

        public Script(String name) {
            this.Name = name;
            this.counters = new Dictionary<String, Int32>();

            counters.Add("print", 0);
            counters.Add("text", 0);
            counters.Add("emote", 0);
            counters.Add("private", 0);
            counters.Add("website", 0);
            counters.Add("html", 0);

            this.engine = new ScriptEngine();

            //ENUMERATIONS 

            engine.Global.DefineProperty("AresId",
                new PropertyDescriptor(new Objects.Enum(this, typeof(AresId)), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("Language",
                new PropertyDescriptor(new Objects.Enum(this, typeof(Language)), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("Country",
                new PropertyDescriptor(new Objects.Enum(this, typeof(Country)), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("Gender",
                new PropertyDescriptor(new Objects.Enum(this, typeof(Gender)), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("AdminLevel",
                new PropertyDescriptor(new Objects.Enum(this, typeof(AdminLevel)), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("CaptchaEvent",
                new PropertyDescriptor(new Objects.Enum(this, typeof(CaptchaEvent)), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("RejectReason",
                new PropertyDescriptor(new Objects.Enum(this, typeof(RejectReason)), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("ClientFeatures",
                new PropertyDescriptor(new Objects.Enum(this, typeof(ClientFeatures)), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("ServerFeatures",
                new PropertyDescriptor(new Objects.Enum(this, typeof(ServerFeatures)), PropertyAttributes.Sealed), true);

            // INSTANCE CLASSES - NOT ENUMERATED 

            engine.Global.DefineProperty("Error",
                new PropertyDescriptor(new Objects.Error.Constructor(this), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("Collection",
                new PropertyDescriptor(new Objects.Collection.Constructor(this), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("List",
                new PropertyDescriptor(new Objects.List.Constructor(this), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("ReadOnlyList",
                new PropertyDescriptor(new Objects.ReadOnlyList.Constructor(this), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("Monitor",
                new PropertyDescriptor(new Objects.Monitor.Constructor(this), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("RoomStats",
                new PropertyDescriptor(new Objects.RoomStats.Constructor(this), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("HttpRequest",
                new PropertyDescriptor(new Objects.HttpRequest.Constructor(this), PropertyAttributes.Sealed), true);
            
            engine.Global.DefineProperty("Hashlink",
                new PropertyDescriptor(new Objects.Hashlink.Constructor(this), PropertyAttributes.Sealed), true);
            
            engine.Global.DefineProperty("ChannelHash",
                new PropertyDescriptor(new Objects.ChannelHash.Constructor(this), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("Avatar",
                new PropertyDescriptor(new Objects.Avatar.Constructor(this), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("FloodRule",
                new PropertyDescriptor(new Objects.FloodRule.Constructor(this), PropertyAttributes.Sealed), true);
            
            engine.Global.DefineProperty("EncodingInstance",
                new PropertyDescriptor(new Objects.EncodingInstance.Constructor(this), PropertyAttributes.Sealed), true);
            
            engine.Global.DefineProperty("UserId",
                new PropertyDescriptor(new Objects.UserId.Constructor(this), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("User",
                new PropertyDescriptor(new Objects.User.Constructor(this), PropertyAttributes.Sealed), true);

            engine.Global.DefineProperty("UserRecord",
                new PropertyDescriptor(new Objects.UserRecord.Constructor(this), PropertyAttributes.Sealed), true);

            // GLOBAL (STATIC) CLASSES

            this.room = new Room(this, Jurassic.Self.Server);

            engine.SetGlobalFunction("user", new Func<Object, User>(room.FindUser));
            engine.SetGlobalFunction("print", new Action<Object, Object>(room.SendAnnounce));

            engine.Global.DefineProperty("user",
                new PropertyDescriptor(Engine.Global["user"], PropertyAttributes.Sealed | PropertyAttributes.Enumerable), true);

            engine.Global.DefineProperty("print",
                new PropertyDescriptor(Engine.Global["print"], PropertyAttributes.Sealed | PropertyAttributes.Enumerable), true);

            engine.Global.DefineProperty("Channels",
                new PropertyDescriptor(new Objects.Channels(this, Jurassic.Self.Server.Channels), PropertyAttributes.Sealed | PropertyAttributes.Enumerable), true);

            engine.Global.DefineProperty("File",
                new PropertyDescriptor(new Objects.File(this), PropertyAttributes.Sealed | PropertyAttributes.Enumerable), true);

            engine.Global.DefineProperty("Script",
                new PropertyDescriptor(new Objects.Script(this), PropertyAttributes.Sealed | PropertyAttributes.Enumerable), true);

            engine.Global.DefineProperty("Encoding",
                new PropertyDescriptor(new Objects.Encoding(this, true), PropertyAttributes.Sealed | PropertyAttributes.Enumerable), true);

            engine.Global.DefineProperty("Base64",
                new PropertyDescriptor(new Objects.Base64(this), PropertyAttributes.Sealed | PropertyAttributes.Enumerable), true);

            engine.Global.DefineProperty("Hashlinks",
                new PropertyDescriptor(new Objects.Hashlinks(this), PropertyAttributes.Sealed | PropertyAttributes.Enumerable), true);

            engine.Global.DefineProperty("Room",
                new PropertyDescriptor(room, PropertyAttributes.Sealed | PropertyAttributes.Enumerable), true);

            foreach(var embedded in Jurassic.Embedded) {
                engine.Global.DefineProperty(
                    embedded.Key,
                    new PropertyDescriptor(embedded.Value.Create(this), embedded.Value.Attributes), true);
            } 

            Eval(default_script.ToString());
        }

        public object Eval(string code) {
            return this.engine.Evaluate(code);
        }

        public T Eval<T>(string code) {
            return this.engine.Evaluate<T>(code);
        }

        public void Unload() {
            this.room = null;
            this.engine = null;
        }

        internal void ResetCounters() {
            var keys = counters.Select((s) => s.Key);
            foreach (var key in keys.ToArray()) counters[key] = 0;
        }
    }
}
