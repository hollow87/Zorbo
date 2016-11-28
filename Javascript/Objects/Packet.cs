using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo;
using Zorbo.Packets;
using Zorbo.Packets.Ares;
using Zorbo.Interface;
using Zorbo.Serialization;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;
using System.Net;

namespace Javascript.Objects
{
    public class Packet : ObjectInstance
    {
        IPacket packet = null;

        protected override string InternalClassName {
            get { return "Packet"; }
        }


        public class GetProperty : FunctionInstance
        {
            IPacket packet = null;
            System.Reflection.PropertyInfo property = null;

            public GetProperty(JScript script, IPacket packet, System.Reflection.PropertyInfo prop)
                : base(script.Engine) {

                this.packet = packet;
                this.property = prop;
            }

            public override object CallLateBound(object thisObject, params object[] argumentValues) {
                if (!this.property.CanRead)
                    return Undefined.Value;

                object ret = this.property.GetValue(packet, null);
                //packets generally use unsigned shorts or ints

                if (ret is byte || ret is ushort || ret is uint)
                    ret = Convert.ToInt32(ret);

                else if (ret is byte[])
                    ret = ((byte[])ret).ToJSArray(Engine);

                else if (ret is IPAddress)
                    ret = ((IPAddress)ret).ToString();

                return ret;
            }
        }

        public class SetProperty : FunctionInstance
        {
            IPacket packet = null;
            System.Reflection.PropertyInfo property = null;

            public SetProperty(JScript script, IPacket packet, System.Reflection.PropertyInfo prop)
                : base(script.Engine) {

                this.packet = packet;
                this.property = prop;
            }

            public override object CallLateBound(object thisObject, params object[] argumentValues) {
                if (this.property.CanWrite) {
                    Object ret = argumentValues[0];
                    Type strtype = typeof(String);

                    //avatar, custom, unknown packets all use byte array members
                    if (property.PropertyType == typeof(byte[])) {
                        if (!(ret is ArrayInstance))
                            return Undefined.Value;

                        ret = ((ArrayInstance)ret).ToArray<byte>();
                    }
                    else if (property.PropertyType == typeof(IPAddress)) {
                        if (ret is ArrayInstance) {
                            var array = (ArrayInstance)ret;
                            if (array.Length < 4) return Undefined.Value;

                            ret = new IPAddress(array.ToArray<byte>(4, true));
                        }
                        else if (ret is String || ret is ConcatenatedString) {
                            IPAddress ip = null;
                            if (IPAddress.TryParse(ret.ToString(), out ip))
                                ret = ip;

                            else return Undefined.Value;
                        }
                    }
                    else if (property.PropertyType == strtype)
                        if (ret.GetType() != strtype)
                            ret = ret.ToString();

                    this.property.SetValue(packet, ret, null);
                }
                return null;
            }
        }


        public Packet(JScript script, IPacket packet)
            : base(script.Engine) {

            var props = TypeDictionary.GetRecord(packet.GetType());

            this.packet = packet;
            this.DefineProperty("id", new PropertyDescriptor((int)packet.Id, PropertyAttributes.Enumerable | PropertyAttributes.Sealed), true);

            foreach (var prop in props) {
                var info = prop.Property;

                string name = info.Name;
                name = name[0].ToString().ToLower() + name.Substring(1);

                this.DefineProperty(
                    name,
                    new PropertyDescriptor(
                        new GetProperty(script, packet, info),
                        new SetProperty(script, packet, info),
                        PropertyAttributes.Enumerable | PropertyAttributes.Sealed),
                    true);
            }
        }
    }
}
