using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using Zorbo;
using Zorbo.Interface;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;
using System.Collections;

namespace Javascript.Objects
{
    public class Banned : Collection
    {
        IHistory history;

        [JSProperty(Name = "count", IsConfigurable = true)]
        public override int Count {
            get { return this.history.Bans.Count; }
        }

        protected override string InternalClassName {
            get { return "Banned"; }
        }

        public Banned(JScript script, IHistory history)
            : base(script, ((ClrFunction)script.Engine.Global["Collection"]).InstancePrototype) {
            
            this.script = script;
            this.history = history;

            this.PopulateFunctions();
        }

        [JSFunction(Name = "add", IsEnumerable = true, IsWritable = false)]
        public bool Add(object a) {

            if (a is User)
                return this.history.Bans.Add(((User)a).Client.ClientId);

            else if (a is UserId)
                return this.history.Bans.Add(((UserId)a));

            else if (a is UserRecord)
                return this.history.Bans.Add(((UserRecord)a).Record.ClientId);

            return false;
        }

        [JSFunction(Name = "remove", IsEnumerable = true, IsWritable = false)]
        public bool Remove(object a) {

            if (a is User)
                return this.history.Bans.Remove(((User)a).Client.ClientId);

            else if (a is UserId) {
                return this.history.Bans.Remove((UserId)a);
            }
            else if (a is UserRecord)
                return this.history.Bans.Remove(((UserRecord)a).Record.ClientId);

            else if (a is FunctionInstance) {

                var func = (FunctionInstance)a;

                for (int i = (Count - 1); i >= 0; i--) {

                    var ban = this.history.Bans[i];
                    var record = new UserRecord(script, this.history.Find((s) => s.Equals(ban)));

                    object ret = func.Call(Engine.Global, record);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        return this.history.Bans.RemoveAt(i);
                }
            }

            return false;
        }

        [JSFunction(Name = "removeAll", IsEnumerable = true, IsWritable = false)]
        public void RemoveAll(object a) {

            if (a is FunctionInstance) {

                var func = (FunctionInstance)a;

                for (int i = (Count - 1); i >= 0; i--) {

                    var ban = this.history.Bans[i];
                    var record = new UserRecord(script, this.history.Find((s) => s.Equals(ban)));

                    object ret = func.Call(Engine.Global, record);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        this.history.Bans.RemoveAt(i);
                }
            }
        }

        [JSFunction(Name = "removeAt", IsEnumerable = true, IsWritable = false)]
        public bool RemoveAt(int index) {
            return this.history.Bans.RemoveAt(index);
        }


        [JSFunction(Name = "clear", IsEnumerable = true, IsWritable = false)]
        public void Clear() {
            this.history.Bans.Clear();
        }
        
        [JSFunction(Name = "find", IsConfigurable = true, IsEnumerable = true, IsWritable = false)]
        public override object Find(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;

                for (int i = 0; i < this.history.Bans.Count; i++) {

                    var ban = this.history.Bans[i];
                    var record = new UserRecord(script, this.history.Find((s) => s.Equals(ban)));

                    object ret = func.Call(Engine.Global, record);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        return record;
                }
            }

            return null;
        }

        [JSFunction(Name = "findAll", IsConfigurable = true, IsEnumerable = true, IsWritable = false)]
        public override Collection FindAll(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;
                var list = new List(script);

                for (int i = 0; i < this.history.Bans.Count; i++) {

                    var ban = this.history.Bans[i];
                    var record = new UserRecord(script, this.history.Find((s) => s.Equals(ban)));

                    object ret = func.Call(Engine.Global, record);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        list.Add(record);
                }

                return list;
            }

            return null;
        }

        [JSFunction(Name = "indexOf", IsConfigurable = true, IsEnumerable = true, IsWritable = false)]
        public override int IndexOf(object a) {

            if (a is FunctionInstance) {
                var func = (FunctionInstance)a;

                for (int i = 0; i < this.history.Bans.Count; i++) {

                    var ban = this.history.Bans[i];
                    var record = new UserRecord(script, this.history.Find((s) => s.Equals(ban)));

                    object ret = func.Call(Engine.Global, record);

                    if (TypeConverter.ConvertTo<bool>(Engine, ret))
                        return i;
                }
                return -1;
            }
            else if (a is User) {
                var user = (User)a;
                return this.history.Bans.IndexOf(new UserId(script, user.Client.Guid, user.Client.ExternalIp));
            }
            else if (a is UserId) {
                return this.history.Bans.IndexOf((UserId)a);
            }
            else if (a is UserRecord) {
                var record = (UserRecord)a;
                return this.history.Bans.IndexOf(new UserId(script, record.Guid, record.ExternalIp));
            }

            return -1;
        }

        [JSFunction(Name = "contains", IsConfigurable = true, IsEnumerable = true, IsWritable = false)]
        public override bool Contains(object a) {
            return (IndexOf(a) > -1);
        }

        public override IEnumerable<PropertyNameAndValue> Properties {
            get {
                for (int i = 0; i < this.Count; i++) {
                    var ban = this.history.Bans[i];
                    var banned = new UserRecord(script, this.history.Find(s => s.Equals(ban)));

                    yield return new PropertyNameAndValue(i.ToString(), new PropertyDescriptor(banned, PropertyAttributes.FullAccess));
                }

                yield return new PropertyNameAndValue(null, new PropertyDescriptor(null, PropertyAttributes.Sealed));
            }
        }

        public override PropertyDescriptor GetOwnPropertyDescriptor(uint index) {

            if (index < Count) {
                var ban = this.history.Bans[(int)index];
                var banned = new UserRecord(script, this.history.Find(s => s.Equals(ban)));

                return new PropertyDescriptor(banned, PropertyAttributes.FullAccess);
            }

            return new PropertyDescriptor(null, PropertyAttributes.Sealed);
        }
    }
}
