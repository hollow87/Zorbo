using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo;
using Zorbo.Interface;

using Jurassic;
using Jurassic.Library;

namespace Javascript.Objects
{
    public class Base64 : ObjectInstance
    {
        public Base64(Javascript.Script script)
            : base(script.Engine) {

            this.PopulateFunctions();
        }

        protected override string InternalClassName {
            get { return "Base64"; }
        }

        [JSFunction(Name = "encode", IsEnumerable = true, IsWritable = false)]
        public string Encode(object a) {

            if (a is String || a is ConcatenatedString) {
                string str = a.ToString();

                byte[] tmp = System.Text.Encoding.ASCII.GetBytes(str);
                return Convert.ToBase64String(tmp);
            }
            else if (a is ArrayInstance) {
                var array = (ArrayInstance)a;

                byte[] tmp = array.ToArray<byte>();
                return Convert.ToBase64String(tmp);
            }
            else return null;
        }

        [JSFunction(Name = "decode", IsEnumerable = true, IsWritable = false)]
        public ArrayInstance Decode(object a) {

            if (a is String || a is ConcatenatedString) {
                string str = a.ToString();
                byte[] tmp = Convert.FromBase64String(str);

                return Engine.Array.New(tmp.Select((s) => (object)(int)s).ToArray());
            }
            else if (a is ArrayInstance) {
                var array = (ArrayInstance)a;

                byte[] tmp = array.ToArray<byte>();
                string str = System.Text.Encoding.ASCII.GetString(tmp);

                tmp = Convert.FromBase64String(str);

                return Engine.Array.New(tmp.Select((s) => (object)(int)s).ToArray());
            }
            else return null;
        }
    }
}
