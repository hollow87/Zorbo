using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;
using Zorbo.Hashlinks;

namespace Javascript.Objects
{
    public class Hashlinks : ObjectInstance
    {
        public Hashlinks(JScript script)
            : base(script.Engine) {

            this.PopulateFunctions();
        }

        protected override string InternalClassName {
            get { return "Hashlinks"; }
        }

        [JSFunction(Name = "encode", IsEnumerable = true, IsWritable = false)]
        public string Encode(ObjectInstance a) {
            if (a is Hashlink)
                return ((Hashlink)a).Encode();

            else if (a.Prototype is Hashlink)
                return TypeConverter.ConvertTo<string>(Engine, a.CallMemberFunction("encode", a));

            return string.Empty;
        }

        [JSFunction(Name = "decode", IsEnumerable = true, IsWritable = false)]
        public object Decode(object a, ObjectInstance b) {

            if (b is Hashlink)
                return ((Hashlink)b).Decode(a);

            else if (b.Prototype is Hashlink)
                return b.CallMemberFunction("decode", a);

            return null;
        }

        [JSFunction(Name = "e67", IsEnumerable = true, IsWritable = false)]
        public ArrayInstance e67(object data, int b) {

            if (data is String || data is ConcatenatedString)
                return HashConvert.e67(System.Text.Encoding.UTF8.GetBytes(data.ToString()), b).ToJSArray(Engine);

            else if (data is ArrayInstance)
                return HashConvert.e67(((ArrayInstance)data).ToArray<byte>(), b).ToJSArray(Engine);

            return null;
        }

        [JSFunction(Name = "d67", IsEnumerable = true, IsWritable = false)]
        public ArrayInstance d67(object data, int b) {

            if (data is String || data is ConcatenatedString)
                return HashConvert.d67(System.Text.Encoding.UTF8.GetBytes(data.ToString()), b).ToJSArray(Engine);

            else if (data is ArrayInstance)
                return HashConvert.d67(((ArrayInstance)data).ToArray<byte>(), b).ToJSArray(Engine);

            return null;
        }
    }
}
