﻿using System;
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
    public class File : ObjectInstance
    {
        JScript script;

        public File(JScript script)
            : base(script.Engine) {

            this.script = script;
            this.PopulateFunctions();
        }
        
        [JSFunction(Name = "length", IsEnumerable = true, IsWritable = false)]
        public NumberInstance Length(object a) {
            
            if ((a is String || a is ConcatenatedString)) {

                FileInfo file = new FileInfo(Path.Combine(Jurassic.Self.Directory, "Scripts", script.Name, a.ToString()));
                return script.Engine.Number.Construct((double)file.Length);
            }

            return script.Engine.Number.Construct(0);
        }
        
        [JSFunction(Name = "create", IsEnumerable = true, IsWritable = false)]
        public bool Create(object a) {

            if ((a is String || a is ConcatenatedString)) {

                string path = Path.Combine(Jurassic.Self.Directory, "Scripts", script.Name, a.ToString());

                if (System.IO.File.Exists(path)) {

                    Stream stream = System.IO.File.Create(path);

                    stream.Close();
                    stream.Dispose();

                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "delete", IsEnumerable = true, IsWritable = false)]
        public bool Delete(object a) {

            if ((a is String || a is ConcatenatedString)) {

                string path = Path.Combine(Jurassic.Self.Directory, "Scripts", script.Name, a.ToString());

                if (System.IO.File.Exists(path)) {

                    System.IO.File.Delete(path);
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "rename", IsEnumerable = true, IsWritable = false)]
        public bool Rename(object a, object b) {

            if ((a is String || a is ConcatenatedString) &&
                (b is String || b is ConcatenatedString)) {

                string path = Path.Combine(Jurassic.Self.Directory, "Scripts", script.Name, a.ToString());

                if (System.IO.File.Exists(path)) {

                    System.IO.File.Move(path, Path.Combine(Jurassic.Self.Directory, "Scripts", script.Name, b.ToString()));
                    System.IO.File.Delete(path);

                    return true;
                }
            }
            return false;
        }


        [JSFunction(Name = "read", IsEnumerable = true, IsWritable = false, Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public string Read(object a) {

            if (a is String || a is ConcatenatedString) {

                string path = Path.Combine(Jurassic.Self.Directory, "Scripts", script.Name, a.ToString());

                if (System.IO.File.Exists(path))
                    return System.IO.File.ReadAllText(path);
            }

            return null;
        }

        [JSFunction(Name = "readLines", IsEnumerable = true, IsWritable = false, Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
        public ArrayInstance ReadLines(object a) {

            if (a is String || a is ConcatenatedString) {

                string path = Path.Combine(Jurassic.Self.Directory, "Scripts", script.Name, a.ToString());

                if (System.IO.File.Exists(path)) {

                    ArrayInstance ret = Engine.Array.Construct();

                    string[] lines = System.IO.File.ReadAllLines(path);
                    lines.ForEach((s) => ret.Push(s));

                    return ret;
                }
            }

            return null;
        }


        [JSFunction(Name = "write", IsEnumerable = true, IsWritable = false)]
        public bool Write(object a, object b) {

            if ((a is String || a is ConcatenatedString) &&
                (b is String || b is ConcatenatedString)) {

                string path = Path.Combine(Jurassic.Self.Directory, "Scripts", script.Name, a.ToString());

                try {
                    System.IO.File.WriteAllText(path, b.ToString());
                    return true;
                }
                catch { }
            }

            return false;
        }

        [JSFunction(Name = "writeLines", IsEnumerable = true, IsWritable = false)]
        public bool WriteLines(object a, ArrayInstance b) {

            if (b != null && (a is String || a is ConcatenatedString)) {

                string path = Path.Combine(Jurassic.Self.Directory, "Scripts", script.Name, a.ToString());

                System.IO.File.WriteAllLines(path, b.ToArray<string>());
                return true;
            }

            return false;
        }


        [JSFunction(Name = "append", IsEnumerable = true, IsWritable = false)]
        public bool Append(object a, object b) {

            if ((a is String || a is ConcatenatedString) &&
                (b is String || b is ConcatenatedString)) {

                string path = Path.Combine(Jurassic.Self.Directory, "Scripts", script.Name, a.ToString());

                if (System.IO.File.Exists(path)) {

                    System.IO.File.AppendAllText(path, b.ToString());
                    return true;
                }
            }

            return false;
        }

        [JSFunction(Name = "appendLines", IsEnumerable = true, IsWritable = false)]
        public bool AppendLines(object a, ArrayInstance b) {

            if (b != null && (a is String || a is ConcatenatedString)) {

                string path = Path.Combine(Jurassic.Self.Directory, "Scripts", script.Name, a.ToString());

                if (!System.IO.File.Exists(path))
                    System.IO.File.Create(path).Dispose();

                System.IO.File.AppendAllLines(path, b.ToArray<string>());
                return true;
            }

            return false;
        }


        protected override string InternalClassName {
            get { return "File"; }
        }
    }
}
