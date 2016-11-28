using System;
using System.Net;
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
    public class UserId : ObjectInstance, IClientId
    {
        Guid guid;
        IPAddress address;


        [JSProperty(Name = "guid", IsEnumerable = true)]
        public string Guid {
            get { return guid.ToString(); }
            set {
                System.Guid g;

                if (System.Guid.TryParse(value, out g))
                    guid = g;
            }
        }

        [JSProperty(Name = "externalIp", IsEnumerable = true)]
        public string ExternalIp {
            get { return address.ToString(); }
            set {
                IPAddress a;

                if (IPAddress.TryParse(value, out a))
                    address = a;
            }
        }


        Guid IClientId.Guid {
            get { return guid; }
            set { guid = value; }
        }

        IPAddress IClientId.ExternalIp {
            get { return address; }
            set { address = value; }
        }
        

        #region " Constructor "

        public class Constructor : ClrFunction
        {
            JScript script;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "UserId", new UserId(script)) {

                this.script = script;
            }

            [JSCallFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public UserId Call(string a, string b) {

                Guid guid;
                IPAddress address;

                if (System.Guid.TryParse(a, out guid) &&
                    IPAddress.TryParse(b, out address)) {

                    return new UserId(script, guid, address);
                }

                return null;
            }

            [JSConstructorFunction(Flags = JSFunctionFlags.ConvertNullReturnValueToUndefined)]
            public UserId Construct(string a, string b) {
                return Call(a, b);
            }
        }

        #endregion


        public UserId(JScript script)
            : base(script.Engine) { }


        public UserId(JScript script, Guid guid, IPAddress address)
            : base(script.Engine, ((ClrFunction)script.Engine.Global["UserId"]).InstancePrototype) {

            this.guid = guid;
            this.address = address;

            this.PopulateFunctions();
        }

        public UserId(JScript script, string guid, string address)
            : base(script.Engine) {

            this.Guid = guid;
            this.ExternalIp = address;

            this.PopulateFunctions();
        }

        protected override string InternalClassName {
            get { return "UserId"; }
        }

        public bool Equals(IClient other) {

            return (other != null && (
                other.Guid.Equals(Guid) ||
                other.ExternalIp.Equals(ExternalIp)));
        }

        public bool Equals(IClientId other) {

            return (other != null && (
                other.Guid.Equals(Guid) ||
                other.ExternalIp.Equals(ExternalIp)));
        }

        public bool Equals(IRecord other) {

            return (other != null && (
                other.ClientId.Guid.Equals(Guid) ||
                other.ClientId.ExternalIp.Equals(ExternalIp)));
        }
    }
}
