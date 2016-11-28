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
    public class Records : Collection
    {
        IHistory history;

        [JSProperty(Name = "count", IsConfigurable = true)]
        public override int Count {
            get { return this.history.Count; }
        }

        protected override string InternalClassName {
            get { return "Records"; }
        }

        public Records(JScript script, IHistory history)
            : base(script, ((ClrFunction)script.Engine.Global["Collection"]).InstancePrototype) {

            this.history = history;
            this.PopulateFunctions();
        }

        [JSFunction(Name = "clear", IsWritable = false)]
        public void Clear() {
            this.history.Bans.Clear();
        }

        [JSFunction(Name = "find", IsConfigurable = true, IsWritable = false)]
        public override object Find(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;

                for (int i = 0; i < this.history.Count; i++) {

                    var record = new UserRecord(script, this.history[i]);
                    object ret = func.Call(Engine.Global, record);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        return record;
                }
            }

            return null;
        }

        [JSFunction(Name = "findAll", IsConfigurable = true, IsWritable = false)]
        public override Collection FindAll(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;
                var list = new List(script);

                for (int i = 0; i < this.history.Count; i++) {

                    var record = new UserRecord(script, this.history[i]);
                    object ret = func.Call(Engine.Global, record);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        list.Add(record);
                }

                return list;
            }

            return null;
        }

        [JSFunction(Name = "indexOf", IsConfigurable = true, IsWritable = false)]
        public override int IndexOf(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;

                for (int i = 0; i < this.history.Count; i++) {

                    var record = new UserRecord(script, this.history[i]);
                    object ret = func.Call(Engine.Global, record);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        return i;
                }
                return -1;
            }
            else if (a is User) {
                var user = (User)a;
                return this.history.FindIndex((s) => s.ClientId.Equals(user.Client.ClientId));
            }
            else if (a is UserId) {
                return this.history.FindIndex((s) => s.ClientId.Equals((UserId)a));
            }
            else if (a is UserRecord) {
                var record = (UserRecord)a;
                return this.history.IndexOf(record.Record);
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
                    var record = new UserRecord(script, this.history[i]);
                    yield return new PropertyNameAndValue(i.ToString(), new PropertyDescriptor(record, PropertyAttributes.FullAccess));
                }
            }
        }

        public override PropertyDescriptor GetOwnPropertyDescriptor(uint index) {

            if (index < Count) {
                var record = new UserRecord(script, this.history[(int)index]);
                return new PropertyDescriptor(record, PropertyAttributes.FullAccess);
            }

            return new PropertyDescriptor(null, PropertyAttributes.Sealed);
        }
    }
}
