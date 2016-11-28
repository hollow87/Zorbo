using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo;
using Zorbo.Interface;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;
using System.Net;

namespace Javascript.Objects
{
    public class UserRecord : ObjectInstance
    {
        IRecord record;

        internal IRecord Record {
            get { return record; } 
        }

        protected override string InternalClassName {
            get { return "UserRecord"; }
        }

        #region " Constructor "

        public class Constructor : ClrFunction
        {
            JScript script = null;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "UserRecord", new UserRecord(script)) {

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

        private UserRecord(JScript script)
            : base(script.Engine) {
            this.PopulateFunctions();
        }

        public UserRecord(JScript script, IRecord record)
            : base(script.Engine, ((ClrFunction)script.Engine.Global["UserRecord"]).InstancePrototype) {

            this.record = record;
            this.PopulateFunctions();
        }

        [JSProperty(Name = "guid", IsEnumerable = true)]
        public String Guid {
            get { return record != null ? record.ClientId.Guid.ToString() : System.Guid.Empty.ToString(); }
        }

        [JSProperty(Name = "name", IsEnumerable = true)]
        public String Name {
            get { return record != null ? record.Name : string.Empty; }
        }

        [JSProperty(Name = "trusted", IsEnumerable = true)]
        public bool Trusted {
            get { return record != null ? record.Trusted : false; }
            set { if (record != null) record.Trusted = value; }
        }

        [JSProperty(Name = "muzzled", IsEnumerable = true)]
        public bool Muzzled {
            get { return record != null ? record.Muzzled : false; }
            set { if (record != null) record.Muzzled = value; }
        }

        [JSProperty(Name = "dnsName", IsEnumerable = true)]
        public String DnsName {
            get { return record != null ? record.DnsName : string.Empty; }
        }

        [JSProperty(Name = "externalIp", IsEnumerable = true)]
        public String ExternalIp {
            get { return record != null ? record.ClientId.ExternalIp.ToString() : IPAddress.Any.ToString(); }
        }

        public override bool Equals(object obj) {
            if (record != null && obj is UserRecord) {
                return this.record.Equals(((UserRecord)obj).Record);
            }
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return this.Record.GetHashCode();
        }
    }
}
