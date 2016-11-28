using Jurassic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo;
using Zorbo.Interface;

using JScript = Javascript.Script;
using System.ComponentModel;

namespace Javascript.Objects
{
    public class RoomStats : Monitor
    {
        IServerStats stats;

        [JSProperty(Name = "peakUsers", IsConfigurable = true)]
        public int PeakUsers {
            get { return stats != null ? stats.PeakUsers : 0; }
        }

        [JSProperty(Name = "joined", IsConfigurable = true)]
        public int Joined {
            get { return stats != null ? stats.Joined : 0; }
        }

        [JSProperty(Name = "parted", IsConfigurable = true)]
        public int Parted {
            get { return stats != null ? stats.Parted : 0; }
        }

        [JSProperty(Name = "rejected", IsConfigurable = true)]
        public int Rejected {
            get { return stats != null ? stats.Rejected : 0; }
        }

        [JSProperty(Name = "banned", IsConfigurable = true)]
        public int Banned {
            get { return stats != null ? stats.Banned : 0; }
        }

        [JSProperty(Name = "captchaBanned", IsConfigurable = true)]
        public int CaptchaBanned {
            get { return stats != null ? stats.CaptchaBanned : 0; }
        }

        [JSProperty(Name = "invalidLogins", IsConfigurable = true)]
        public int InvalidLogins {
            get { return stats != null ? stats.InvalidLogins : 0; }
        }

        [JSProperty(Name = "floodsTriggered", IsConfigurable = true)]
        public int FloodsTriggered {
            get { return stats != null ? stats.FloodsTriggered : 0; }
        }

        [JSProperty(Name = "packetsSent", IsConfigurable = true)]
        public int PacketsSent {
            get { return stats != null ? stats.PacketsSent : 0; }
        }

        [JSProperty(Name = "packetsReceived", IsConfigurable = true)]
        public int PacketsReceived {
            get { return stats != null ? stats.PacketsReceived : 0; }
        }

        [JSProperty(Name = "speedIn", IsConfigurable = true)]
        public override double SpeedIn {
            get { return stats != null ? stats.SpeedIn : 0; }
        }

        [JSProperty(Name = "speedOut", IsConfigurable = true)]
        public override double SpeedOut {
            get { return stats != null ? stats.SpeedOut : 0; }
        }

        [JSProperty(Name = "totalBytesIn", IsConfigurable = true)]
        public override double TotalBytesIn {
            get { return stats != null ? stats.TotalBytesIn : 0; }
        }

        [JSProperty(Name = "totalBytesOut", IsConfigurable = true)]
        public override double TotalBytesOut {
            get { return stats != null ? stats.TotalBytesOut : 0; }
        }

        #region " Constructor "

        public new class Constructor : ClrFunction
        {
            JScript script = null;

            public Constructor(JScript script)
                : base(script.Engine.Function.InstancePrototype, "RoomStats", new RoomStats(script)) {

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

        protected override string InternalClassName {
            get { return "RoomStats"; }
        }

        protected RoomStats(JScript script)
            : base(script, ((ClrFunction)script.Engine.Global["Monitor"]).InstancePrototype) {
            this.PopulateFunctions();
        }

        public RoomStats(JScript script, IServerStats stats)
            : base(script, stats) {

            this.stats = stats;
            this.PopulateFunctions();
        }
    }
}
