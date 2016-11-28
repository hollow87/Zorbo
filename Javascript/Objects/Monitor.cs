using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class Monitor : ObjectInstance
    {
        JScript script = null;
        IMonitor monitor = null;

        [JSProperty(Name = "speedIn", IsConfigurable = true)]
        public virtual double SpeedIn {
            get { return monitor != null ? monitor.SpeedIn : 0; }
        }

        [JSProperty(Name = "speedOut", IsConfigurable = true)]
        public virtual double SpeedOut {
            get { return monitor != null ? monitor.SpeedOut : 0; }
        }

        [JSProperty(Name = "totalBytesIn", IsConfigurable = true)]
        public virtual double TotalBytesIn {
            get { return monitor != null ? monitor.TotalBytesIn : 0; }
        }

        [JSProperty(Name = "totalBytesOut", IsConfigurable = true)]
        public virtual double TotalBytesOut {
            get { return monitor != null ? monitor.TotalBytesOut : 0; }
        }

        protected override string InternalClassName {
            get { return "Monitor"; }
        }

        #region " Constructor "

        public class Constructor : ClrFunction
        {
            JScript script = null;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "Monitor", new Monitor(script)) {

                this.script = script;
            }

            [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public User Call(object a) {
                return null;
            }

            [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public User Construct(object a) {
                return null;
            }
        }

        #endregion

        private Monitor(JScript script)
            : base(script.Engine) {

            this.script = script;
            this.PopulateFunctions();
        }

        protected Monitor(JScript script, ObjectInstance proto)
            : base(script.Engine, proto) {

            this.script = script;
            this.PopulateFunctions();
        }

        public Monitor(JScript script, IMonitor monitor)
            : base(script.Engine, ((ClrFunction)script.Engine.Global["Monitor"]).InstancePrototype) {

            this.script = script;
            this.monitor = monitor;

            this.PopulateFunctions();
        }
    }
}
