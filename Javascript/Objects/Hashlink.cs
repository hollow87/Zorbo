using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo;
using Zorbo.Interface;
using Zorbo.Hashlinks;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class Hashlink : ObjectInstance, IHashlink
    {
        #region " Constructor "

        public class Constructor : ClrFunction
        {
            JScript script = null;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, 
                      "Hashlink", 
                      new Hashlink(script)) {

                this.script = script;
            }

            [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public Hashlink Call(object a) {
                return null;
            }

            [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public Hashlink Construct(object a) {
                return null;
            }
        }

        #endregion

        private Hashlink(JScript script)
            : base(script.Engine) {

            this.PopulateFunctions();
        }

        public Hashlink(JScript script, ObjectInstance proto)
            : base(script.Engine, proto) {

            this.PopulateFunctions();
        }

        protected override string InternalClassName {
            get { return "Hashlink"; }
        }

        [JSFunction(Name = "encode", IsConfigurable = true, IsWritable = true)]
        public virtual string Encode() {
            return null;
        }

        [JSFunction(Name = "decode", IsConfigurable = true, IsWritable = true)]
        public virtual object Decode(object a) {
            return null;
        }


        public virtual byte[] ToByteArray() {
            return null;
        }

        public virtual void FromByteArray(byte[] value) {

        }
    }
}
