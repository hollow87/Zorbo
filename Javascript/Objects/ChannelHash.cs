using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo;
using Zorbo.Interface;
using Zorbo.Hashlinks;

using Jurassic;
using Jurassic.Library;

using JScript = Javascript.Script;

namespace Javascript.Objects
{
    public class ChannelHash : Hashlink
    {
        JScript script = null;
        Channel channel = null;
       
        #region " Constructor "

        public new class Constructor : ClrFunction
        {
            JScript script = null;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "ChannelHash", new ChannelHash(script)) {

                this.script = script;
            }

            [JSCallFunction]
            public ChannelHash Call(object a, object b, object c, object d) {
                return Construct(a, b, c, d);
            }

            [JSConstructorFunction]
            public ChannelHash Construct(object a, object b, object c, object d) {

                return new ChannelHash(script) {
                    Name = (!(a is Undefined)) ? TypeConverter.ConvertTo<String>(Engine, a) : "",
                    Port = (!(b is Undefined)) ? TypeConverter.ConvertTo<Int32>(Engine,b) : 0,
                    ExternalIp = (!(c is Undefined)) ? TypeConverter.ConvertTo<String>(Engine, c) : "",
                    LocalIp = (!(d is Undefined)) ? TypeConverter.ConvertTo<String>(Engine, d) : ""
                };
            }
        }

        #endregion

        [JSProperty(Name = "name", IsConfigurable = true)]
        public string Name {
            get { return channel.Name; }
            set { channel.Name = value; } 
        }

        [JSProperty(Name = "port", IsConfigurable = true)]
        public int Port {
            get { return (int)channel.Port; }
            set { channel.Port = (ushort)value; }
        }

        [JSProperty(Name = "localIp", IsConfigurable = true)]
        public String LocalIp {
            get { return channel.LocalIp.ToString(); }
            set {
                IPAddress ip = null;

                if (IPAddress.TryParse(value, out ip))
                    channel.LocalIp = ip;
            }
        }

        [JSProperty(Name = "externalIp", IsConfigurable = true)]
        public String ExternalIp {
            get { return channel.ExternalIp.ToString(); }
            set {
                IPAddress ip = null;

                if (IPAddress.TryParse(value, out ip))
                    channel.ExternalIp = ip;
            }
        }

        public ChannelHash(JScript script)
            : base(script, ((ClrFunction)script.Engine.Global["Hashlink"]).InstancePrototype) {

            this.script = script;
            this.channel = new Channel();

            this.PopulateFunctions();
        }

        protected override string InternalClassName {
            get { return "ChannelHash"; }
        }

        [JSFunction(Name = "encode", IsConfigurable = true, IsWritable = false)]
        public override string Encode() {
            return HashConvert.ToHashlinkString<ChannelHash>(this);
        }

        [JSFunction(Name = "decode", IsConfigurable = true, IsWritable = false)]
        public override object Decode(object a) {

            if (a is String || a is ConcatenatedString) {
                var str = a.ToString();
                return HashConvert.FromHashlinkString<ChannelHash>(str, this);
            }
            else if (a is ArrayInstance) {
                var array = (ArrayInstance)a;
                byte[] tmp = array.ToArray<byte>();

                return HashConvert.FromHashlinkArray<ChannelHash>(tmp, this);
            }

            return null;
        }


        public override byte[] ToByteArray() {
            return channel.ToByteArray();
        }

        public override void FromByteArray(byte[] value) {
            channel.FromByteArray(value);
        }
    }
}
