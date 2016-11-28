using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public abstract class BaseCollection : ObjectInstance
    {
        protected JScript script;

        [JSProperty(Name = "count", IsConfigurable = true)]
        public abstract int Count { get; }

        public BaseCollection(JScript script)
            : base(script.Engine) {
            this.script = script;
            this.PopulateFunctions();
        }

        public BaseCollection(JScript script, ObjectInstance prototype)
            : base(script.Engine, prototype) {
            this.script = script;
            this.PopulateFunctions();
        }

        [JSFunction(Name = "find", IsConfigurable = true, IsWritable = false)]
        public abstract object Find(object a);

        [JSFunction(Name = "findAll", IsConfigurable = true, IsWritable = false)]
        public abstract Collection FindAll(object a);

        [JSFunction(Name = "indexOf", IsConfigurable = true, IsWritable = false)]
        public abstract int IndexOf(object a);

        [JSFunction(Name = "contains", IsConfigurable = true, IsWritable = false)]
        public abstract bool Contains(object a);
    }
}
