using System;
using System.IO;
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
    public class Script : ObjectInstance
    {
        protected override string InternalClassName {
            get { return "Script"; }
        }

        public Script(JScript script)
            : base(script.Engine) {
            
            this.PopulateFunctions();
        }

        [JSFunction(Name = "load", IsEnumerable = true, IsWritable = false)]
        public static bool Load(string name) {

            name = name.ToLower();
            
            var script = Jurassic.Scripts.Find((s) => s.Name.ToLower() == name);
            if (script != null) Kill(name);
            
            string path = Path.Combine(Jurassic.Self.Directory, "Scripts", name, name + ".js");

            try {
                script = new JScript(name);

                Jurassic.Self.Server.Users.ForEach((s) => script.Room.Users.Items.Add(new User(script, s)));
                Jurassic.Scripts.Add(script);

                script.Eval(System.IO.File.ReadAllText(path));
                script.ResetCounters();

                return true;
            }
            catch (JavaScriptException jex) {
                Jurassic.Self.OnError(jex);
                Kill(name);
            }
            return false;
        }

        [JSFunction(Name = "create", IsEnumerable = true, IsWritable = false)]
        public static bool Create(string name) {

            name = name.ToLower();

            var script = Jurassic.Scripts.Find((s) => s.Name.ToLower() == name);
            if (script != null) Kill(name);

            try {
                script = new JScript(name);

                Jurassic.Self.Server.Users.ForEach((s) => script.Room.Users.Items.Add(new User(script, s)));
                Jurassic.Scripts.Add(script);

                script.ResetCounters();

                return true;
            }
            catch (JavaScriptException jex) {
                Jurassic.Self.OnError(jex);
                Kill(name);
            }

            return false;
        }

        [JSFunction(Name = "eval", IsEnumerable = true, IsWritable = false)]
        public static object Eval(string name, string code) {
            object ret = null;
            name = name.ToLower();
            
            var script = Jurassic.Scripts.Find((s) => s.Name.ToLower() == name);
            if (script == null) return string.Empty;

            try {
                ret = script.Eval(code);
            }
            catch (JavaScriptException jex) {
                Jurassic.Self.OnError(jex);
            }
            finally { script.ResetCounters(); }

            return ret;
        }

        [JSFunction(Name = "kill", IsEnumerable = true, IsWritable = false)]
        public static void Kill(string name) {

            name = name.ToLower();

            int index = Jurassic.Scripts.FindIndex((s) => s.Name.ToLower() == name);
            if (index < 0) return;

            Jurassic.Scripts[index].Unload();
            Jurassic.Scripts.RemoveAt(index);
        }
    }
}
