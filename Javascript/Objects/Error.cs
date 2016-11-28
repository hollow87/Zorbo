using System;
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
    public class Error : ObjectInstance
    {
        JScript script = null;
        JavaScriptException error = null;

        #region " Constructor "

        public class Constructor : ClrFunction
        {
            JScript script = null;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "Error", new Error(script)) {

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

        protected override string InternalClassName {
            get { return "Error"; }
        }

        public Error(JScript script)
            : base(script.Engine) {

            this.script = script;

            this.PopulateFields();
            this.PopulateFunctions();
        }

        public Error(JScript script, JavaScriptException ex)
            : base(script.Engine, ((ClrFunction)script.Engine.Global["Error"]).InstancePrototype) {

            this.error = ex;
            this.script = script;

            this.PopulateFields();
            this.PopulateFunctions();
        }

        [JSProperty(Name = "line")]
        public int Line {
            get { return error != null ? error.LineNumber : 0; }
        }

        [JSProperty(Name = "script")]
        public string Script {
            get { return script.Name; }
        }

        [JSProperty(Name = "source")]
        public string Source {
            get { return error != null ? error.Source : string.Empty; }
        }

        [JSProperty(Name = "trace")]
        public string StackTrace {
            get { return error != null ? error.StackTrace : string.Empty; }
        }

        [JSProperty(Name = "message")]
        public string Message {
            get { return error != null ? error.Message : string.Empty; }
        }
    }
}
