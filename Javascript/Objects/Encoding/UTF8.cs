using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class UTF8 : EncodingInstance
    {
        JScript script;

        #region " Constructor "

        public new class Constructor : ClrFunction
        {
            JScript script;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "UTF8", new UTF8(script)) {

                this.script = script;
            }

            [JSCallFunction]
            public UTF8 Call() { return new UTF8(script); }

            [JSConstructorFunction]
            public UTF8 Construct() { return new UTF8(script); }
        }

        #endregion

        protected override string InternalClassName {
            get { return "UTF8"; }
        }

        public UTF8(JScript script)
            : base(script, System.Text.Encoding.UTF8) {

            this.script = script;
            this.PopulateFunctions();
        }
    }
}
