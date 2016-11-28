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
    public class UTF32 : EncodingInstance
    {
        JScript script;

        #region " Constructor "

        public new class Constructor : ClrFunction
        {
            JScript script;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "UTF32", new UTF32(script)) {

                this.script = script;
            }

            [JSCallFunction]
            public UTF32 Call() { return new UTF32(script); }

            [JSConstructorFunction]
            public UTF32 Construct() { return new UTF32(script); }
        }

        #endregion


        public UTF32(JScript script)
            : base(script, System.Text.Encoding.UTF32) {

            this.script = script;
            this.PopulateFunctions();
        }

        protected override string InternalClassName {
            get { return "UTF32"; }
        }
    }
}
