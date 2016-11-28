using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Jurassic.Library;
using Javascript.Objects;

using JScript = Javascript.Script;
using Jurassic;
using Zorbo.Packets.Ares;
using System.Drawing;
using Zorbo;
using System.Drawing.Imaging;
using System.IO;
using Zorbo.SharpZip.Zip.Compression.Streams;

namespace cb0tProtocol
{
    public class Scribble : ObjectInstance
    {
        JScript script = null;
        string source = string.Empty;

        RoomScribble scribble = null;

        [JSProperty(Name = "source")]
        public string Source {
            get { return source; }
            set {
                if (source != value) {
                    source = value;
                    if (scribble != null) {
                        scribble.Reset();
                        scribble = null;
                    }
                }
            }
        }

        protected override string InternalClassName {
            get { return "Scribble"; }
        }

        #region " Constructor "

        public class Constructor : ClrFunction
        {
            JScript script = null;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "Scribble", new Scribble(script)) {

                this.script = script;
            }

            [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public Scribble Call(object a) {
                return Construct(a);
            }

            [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public Scribble Construct(object a) {
                if (a != null && !(a is Undefined)) {
                    if (a is String || a is ConcatenatedString)
                        return new Scribble(script, InstancePrototype, a.ToString());
                }
                return new Scribble(script, InstancePrototype);
            }
        }

        #endregion

        private Scribble(JScript script)
            : base(script.Engine) {

            this.script = script;
            this.PopulateFunctions();
        }

        private Scribble(JScript script, ObjectInstance proto)
            : base(script.Engine, proto) {

            this.script = script;
            this.PopulateFunctions();
        }

        private Scribble(JScript script, ObjectInstance proto, string source)
            : base(script.Engine, proto) {

            this.script = script;
            this.Source = source;
            this.PopulateFunctions();
        }

        [JSFunction(Name = "load", IsEnumerable = true)]
        public bool Load(object state) {

            if (scribble != null) {
                LoadCallback(state);
                return true;
            }

            try {
                Uri toGet = null;
                string path = Path.Combine(script.Directory, Source);

                if (Uri.TryCreate(Source, UriKind.Absolute, out toGet) ||
                    Uri.TryCreate(path, UriKind.Absolute, out toGet)) {

                    if (toGet.IsFile) {
                        FileInfo file = new FileInfo(toGet.AbsolutePath);
                        if (file.Exists && file.Directory.FullName != script.Directory)
                            throw new UnauthorizedAccessException("You are not allowed to access this file.");
                    }
                }
                else throw new UriFormatException(Source);

                scribble = new RoomScribble();
                scribble.Download(toGet, LoadCallback, state);
            }
            catch(Exception ex) {
                OnError(ex);
                return false;
            }

            return true;
        }

        private void LoadCallback(object state) {
            if (state is Exception) {
                OnError((Exception)state);
            }
            else {
                this.CallMemberFunction("onLoad", state);
            }
        }

        [JSFunction(Name = "onLoad", IsEnumerable = true, IsConfigurable = true, IsWritable = true)]
        public virtual void OnLoad(object state) {
            Send(state ?? Undefined.Value);
        }

        [JSFunction(Name = "send", IsEnumerable = true)]
        public void Send(object a) {
            var plugin = cb0tProtocol.Self;

            if (a is Undefined) {
                plugin.SendRoomScribble(plugin.Server.Config.BotName, scribble);
            }
            else if (a is int || a is double) {
                int x  = (int)a;
                plugin.SendRoomScribble((s) => s.Vroom == x, plugin.Server.Config.BotName, scribble);
            }
            else if (a is User) {
                var user = (User)a;
                plugin.SendRoomScribble((s) => s == user.Client, plugin.Server.Config.BotName, scribble);
            }
        }

        private void OnError(Exception ex) {
            this.CallMemberFunction(
                    "onError",
                    new Javascript.Objects.Error(
                        script, 
                        new JavaScriptException(Engine.Error.Construct(ex.Message), 0, null)));
        }

        [JSFunction(Name = "onError", IsEnumerable = true, IsConfigurable = true, IsWritable = true)]
        public virtual void OnError(object state) { }
    }
}
