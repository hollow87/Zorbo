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
    public class FloodRules : Collection
    {
        IList<IFloodRule> rules;

        [JSProperty(Name = "count", IsConfigurable = true)]
        public override int Count {
            get { return rules.Count; }
        }

        protected override string InternalClassName {
            get { return "FloodRules"; }
        }

        public FloodRules(JScript script, IList<IFloodRule> rules)
            : base(script, ((ClrFunction)script.Engine.Global["Collection"]).InstancePrototype) {

            this.rules = rules;
            this.PopulateFunctions();
        }

        [JSFunction(Name = "add", IsWritable = false)]
        public bool Add(object id, object count, object timeout) {

            if (id is FloodRule) {
                rules.Add((FloodRule)id);
                return true;
            }
            else if (!(id is int) || !(count is int) || !(timeout is int))
                return false;

            rules.Add(new FloodRule(script, (int)id, (int)count, (int)timeout));
            return true;
        }

        [JSFunction(Name = "remove", IsWritable = false)]
        public bool Remove(object a) {

            if (a is FloodRule) {
                return rules.Remove((FloodRule)a);
            }

            return false;
        }

        [JSFunction(Name = "removeAll", IsWritable = false)]
        public void RemoveAll(object a) {

            if (a is FunctionInstance) {

                var func = (FunctionInstance)a;

                for (int i = (Count - 1); i >= 0; i--) {

                    object ret = func.Call(
                        Engine.Global,
                        new FloodRule(script, this.rules[i]));

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        this.rules.RemoveAt(i);
                }
            }
        }

        [JSFunction(Name = "removeAt", IsWritable = false)]
        public bool RemoveAt(int index) {

            if (index < 0 || index >= this.rules.Count)
                return false;

            this.rules.RemoveAt(index);
            return true;
        }


        [JSFunction(Name = "clear", IsWritable = false)]
        public void Clear() {
            this.rules.Clear();
        }

        [JSFunction(Name = "find", IsConfigurable = true, IsWritable = false)]
        public override object Find(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;

                for (int i = 0; i < this.Count; i++) {

                    var rule = new FloodRule(script, this.rules[i]);
                    object ret = func.Call(Engine.Global, rule);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        return rule;
                }
            }

            return null;
        }

        [JSFunction(Name = "findAll", IsConfigurable = true, IsWritable = false)]
        public override Collection FindAll(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;
                var list = new List(script);

                for (int i = 0; i < this.Count; i++) {

                    var rule = new FloodRule(script, this.rules[i]);
                    object ret = func.Call(Engine.Global, rule);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        list.Add(rule);
                }

                return list;
            }

            return null;
        }

        [JSFunction(Name = "indexOf", IsConfigurable = true, IsWritable = false)]
        public override int IndexOf(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;

                for (int i = 0; i < this.Count; i++) {

                    var rule = new FloodRule(script, this.rules[i]);
                    object ret = func.Call(Engine.Global, rule);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        return i;
                }
                return -1;
            }
            else if (a is FloodRule) {
                var rule = (FloodRule)a;
                return this.rules.FindIndex((s) => s.Id == rule.Id);
            }

            return -1;
        }

        [JSFunction(Name = "contains", IsConfigurable = true, IsWritable = false)]
        public override bool Contains(object a) {
            return (IndexOf(a) > -1);
        }

        public override IEnumerable<PropertyNameAndValue> Properties {
            get {
                for (int i = 0; i < this.Count; i++) {
                    var rule = new FloodRule(script, this.rules[i]);
                    yield return new PropertyNameAndValue(i.ToString(), new PropertyDescriptor(rule, PropertyAttributes.FullAccess));
                }
            }
        }

        public override PropertyDescriptor GetOwnPropertyDescriptor(uint index) {

            if (index < Count) {
                var rule = new FloodRule(script, this.rules[(int)index]);
                return new PropertyDescriptor(rule, PropertyAttributes.FullAccess);
            }

            return new PropertyDescriptor(null, PropertyAttributes.Sealed);
        }
    }
}
