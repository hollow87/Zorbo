using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Zorbo;
using Zorbo.Interface;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class RangeBanned : Collection
    {
        IHistory history;

        [JSProperty(Name = "count", IsConfigurable = true)]
        public override int Count {
            get { return this.history.RangeBans.Count; }
        }

        protected override string InternalClassName {
            get { return "RangeBanned"; }
        }

        public RangeBanned(JScript script, IHistory history)
            : base(script, ((ClrFunction)script.Engine.Global["Collection"]).InstancePrototype) {

            this.history = history;
            this.PopulateFunctions();
        }

        [JSFunction(Name = "add", IsEnumerable = true, IsWritable = false)]
        public bool Add(RegExpInstance regex) {
            return this.history.RangeBans.Add(regex.Value);
        }

        [JSFunction(Name = "remove", IsEnumerable = true, IsWritable = false)]
        public bool Remove(RegExpInstance regex) {
            return this.history.RangeBans.Remove(regex.Value);
        }

        [JSFunction(Name = "removeAll", IsEnumerable = true, IsWritable = false)]
        public void RemoveAll(object a) {

            if (a is FunctionInstance) {

                var func = (FunctionInstance)a;

                for (int i = (Count - 1); i >= 0; i--) {

                    var ban = this.history.RangeBans[i];
                    object ret = func.Call(Engine.Global, ban.ToRegExpInstance(script.Engine));

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        this.history.RangeBans.RemoveAt(i);
                }
            }
        }

        [JSFunction(Name = "removeAt", IsEnumerable = true, IsWritable = false)]
        public bool RemoveAt(int index) {
            return this.history.RangeBans.RemoveAt(index);
        }

        [JSFunction(Name = "clear", IsEnumerable = true, IsWritable = false)]
        public void Clear() {
            this.history.RangeBans.Clear();
        }

        [JSFunction(Name = "find", IsConfigurable = true, IsWritable = false)]
        public override object Find(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;

                for (int i = 0; i < this.history.RangeBans.Count; i++) {

                    var ban = this.history.RangeBans[i];
                    var regexp = ban.ToRegExpInstance(script.Engine);

                    object ret = func.Call(Engine.Global, regexp);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        return regexp;
                }
            }

            return null;
        }

        [JSFunction(Name = "findAll", IsConfigurable = true, IsWritable = false)]
        public override Collection FindAll(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;
                var list = new List(script);

                for (int i = 0; i < this.history.RangeBans.Count; i++) {

                    var ban = this.history.RangeBans[i];
                    var regexp = ban.ToRegExpInstance(script.Engine);

                    object ret = func.Call(Engine.Global, regexp);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        list.Add(regexp);
                }

                return list;
            }

            return null;
        }

        [JSFunction(Name = "indexOf", IsConfigurable = true, IsWritable = false)]
        public override int IndexOf(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;

                for (int i = 0; i < this.history.RangeBans.Count; i++) {

                    var ban = this.history.RangeBans[i];
                    object ret = func.Call(Engine.Global, ban.ToRegExpInstance(script.Engine));

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        return i;
                }
                return -1;
            }
            else if (a is RegExpInstance) {
                return this.history.RangeBans.FindIndex((s) => s.Equals(((RegExpInstance)a).Value));
            }

            return -1;
        }

        [JSFunction(Name = "contains", IsConfigurable = true, IsWritable = false)]
        public override bool Contains(object a) {
            return (IndexOf(a) > -1);
        }
    }
}
