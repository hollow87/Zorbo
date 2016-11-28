using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class FloodRule : ObjectInstance, IFloodRule
    {
        #region " Constructor "

        public class Constructor : ClrFunction
        {
            JScript script = null;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "FloodRule", new FloodRule(script)) {

                this.script = script;
            }

            [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public FloodRule Call(object a, object b, object c) {
                return Construct(a, b, c);
            }

            [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public FloodRule Construct(object a, object b, object c) {
                
                if (!(a is int) || !(b is int) || !(c is int))
                    return null;

                return new FloodRule(script, InstancePrototype, (int)a, (int)b, (int)c);
            }
        }

        #endregion

        int id = 0;
        int count = 4;
        int reset = 1000;

        [JSProperty(Name = "id")]
        public int Id {
            get { return id; }
        }

        byte IFloodRule.Id {
            get { return (byte)id; }
        }

        [JSProperty(Name = "count")]
        public int Count {
            get { return count; }
            set { count = value; }
        }

        [JSProperty(Name = "resetTimeout")]
        public int ResetTimeout {
            get { return reset; }
            set { reset = value; }
        }

        protected override string InternalClassName {
            get { return "FloodRule"; }
        }

        public FloodRule(JScript script)
            : base(script.Engine) {

            this.PopulateFunctions();
        }

        public FloodRule(JScript script, IFloodRule rule)
            : base(script.Engine) {

            this.id = rule.Id;
            this.count = rule.Count;
            this.reset = rule.ResetTimeout;

            this.PopulateFunctions();
        }

        internal FloodRule(JScript script, int id, int count, int timeout)
            : base(script.Engine) {

            this.id = id > 255 ? 255 : (id < 0) ? 0 : id;
            this.count = count;
            this.reset = timeout;

            this.PopulateFunctions();
        }

        internal FloodRule(JScript script, ObjectInstance proto, int id, int count, int timeout)
            : base(script.Engine, proto) {

            this.id = id > 255 ? 255 : (id < 0) ? 0 : id;
            this.count = count;
            this.reset = timeout;

            this.PopulateFunctions();
        }
    }
}
