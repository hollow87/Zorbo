using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class Collection : BaseCollection
    {
        protected List<Object> items;

        [JSProperty(Name = "count", IsConfigurable = true)]
        public override int Count {
            get { return items.Count; }
        }

        internal List<Object> Items {
            get { return items; }
        }

        #region " Constructor "

        public class Constructor : ClrFunction
        {
            JScript script = null;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "Collection", new Collection(script)) {

                this.script = script;
            }

            [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public Collection Call(object a) {
                return null;
            }

            [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public Collection Construct(object a) {
                return null;
            }
        }

        #endregion

        protected override string InternalClassName {
            get { return "Collection"; }
        }


        private Collection(JScript script)
            : base(script) {

            this.script = script;
            this.items = new List<Object>();

            this.PopulateFunctions();
        }

        public Collection(JScript script, ObjectInstance prototype)
            : base(script, prototype) {

            this.script = script;
            this.items = new List<Object>();

            this.PopulateFunctions();
        }

        [JSFunction(Name = "find", IsConfigurable = true, IsWritable = false)]
        public override object Find(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;

                for (int i = 0; i < items.Count; i++) {
                    object ret = func.Call(Engine.Global, items[i]);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        return items[i];
                }
            }

            return null;
        }

        [JSFunction(Name = "findAll", IsConfigurable = true, IsWritable = false)]
        public override Collection FindAll(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;
                var list = new List(script);

                for (int i = 0; i < items.Count; i++) {
                    object ret = func.Call(Engine.Global, items[i]);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        list.Add(items[i]);
                }

                return list;
            }

            return null;
        }

        [JSFunction(Name = "indexOf", IsConfigurable = true, IsWritable = false)]
        public override int IndexOf(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;

                for (int i = 0; i < items.Count; i++) {
                    object ret = func.Call(Engine.Global, items[i]);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        return i;
                }
                return -1;
            }
            else return items.IndexOf(a);
        }

        [JSFunction(Name = "contains", IsConfigurable = true, IsWritable = false)]
        public override bool Contains(object a) {
            return (IndexOf(a) > -1);
        }

        protected override object GetMissingPropertyValue(string propertyName) {
            return base.GetMissingPropertyValue(propertyName);
        }

        public override IEnumerable<PropertyNameAndValue> Properties {
            get {
                for (int i = 0; i < items.Count; i++)
                    yield return new PropertyNameAndValue(i.ToString(), new PropertyDescriptor(items[i], PropertyAttributes.FullAccess));

                yield return new PropertyNameAndValue(null, new PropertyDescriptor(null, PropertyAttributes.Sealed));
            }
        }

        public override PropertyDescriptor GetOwnPropertyDescriptor(uint index) {

            if (index < items.Count)
                return new PropertyDescriptor(items[(int)index], PropertyAttributes.FullAccess);

            return new PropertyDescriptor(null, PropertyAttributes.Sealed);
        }
    }
}
