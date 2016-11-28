using System;
using System.ComponentModel;
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
    public class Channels : ObjectInstance
    {
        IChannelList channels;
        Monitor monitor = null;

        [JSProperty(Name = "monitor")]
        public Monitor Monitor {
            get { return monitor; }
        }

        [JSProperty(Name = "ackIpHits")]
        public uint AckIpHits {
            get { return (channels != null) ? channels.AckIpHits : 0; }
        }

        [JSProperty(Name = "ackInfoHits")]
        public uint AckInfoHits {
            get { return (channels != null) ? channels.AckInfoHits : 0; }
        }

        [JSProperty(Name = "sendInfoHits")]
        public uint SendInfoHits {
            get { return (channels != null) ? channels.SendInfoHits : 0; }
        }

        [JSProperty(Name = "sendNodeHits")]
        public uint SendNodeHits {
            get { return (channels != null) ? channels.SendNodeHits : 0; }
        }

        [JSProperty(Name = "checkFirewallHits")]
        public uint CheckFirewallHits {
            get { return (channels != null) ? channels.CheckFirewallHits : 0; }
        }

        [JSProperty(Name = "showRoom")]
        public bool Listing {
            get { return (channels != null) ? channels.Listing : false; }
            set { if (channels != null) channels.Listing = value; }
        }

        [JSProperty(Name = "firewallOpen")]
        public bool FirewallOpen {
            get { return (channels != null) ? channels.FirewallOpen : false; }
        }

        [JSProperty(Name = "finishedTestingFirewall")]
        public bool FinishedTestingFirewall {
            get { return (channels != null) ? channels.FinishedTestingFirewall : false; }
        }

        protected override string InternalClassName {
            get { return "Channels"; }
        }

        public Channels(JScript script, IChannelList channels)
            : base(script.Engine) {

            this.channels = channels;
            this.monitor = new Monitor(script, channels.Monitor);

            this.PopulateFunctions();
        }
    }
}
