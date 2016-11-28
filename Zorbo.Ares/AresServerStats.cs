using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using Zorbo.Data;
using Zorbo.Interface;

namespace Zorbo
{
    public class AresServerStats : IOMonitor, IServerStats
    {
        int peakusers;
        int joined;
        int parted;
        int rejected;
        int banned;
        int captchabanned;
        int invalidlogins;
        int floodtiggered;
        int packetssent;
        int packetsrecv;

        public int PeakUsers {
            get { return peakusers; }
            internal set {
                if (peakusers != value) {
                    Interlocked.Exchange(ref peakusers, value);
                    RaisePropertyChanged(() => PeakUsers);
                }
            }
        }

        public int Joined {
            get { return joined; }
            internal set {
                if (joined != value) {
                    Interlocked.Exchange(ref joined, value);
                    RaisePropertyChanged(() => Joined);
                }
            }
        }

        public int Parted {
            get { return parted; }
            internal set {
                if (parted != value) {
                    Interlocked.Exchange(ref parted, value);
                    RaisePropertyChanged(() => Parted);
                }
            }
        }

        public int Rejected {
            get { return rejected; }
            internal set {
                if (rejected != value) {
                    Interlocked.Exchange(ref rejected, value);
                    RaisePropertyChanged(() => Rejected);
                }
            }
        }

        public int Banned {
            get { return banned; }
            internal set {
                if (banned != value) {
                    Interlocked.Exchange(ref banned, value);
                    RaisePropertyChanged(() => Banned);
                }
            }
        }

        public int CaptchaBanned {
            get { return captchabanned; }
            internal set {
                if (captchabanned != value) {
                    Interlocked.Exchange(ref captchabanned, value);
                    RaisePropertyChanged(() => CaptchaBanned);
                }
            }
        }

        public int InvalidLogins {
            get { return invalidlogins; }
            internal set {
                if (invalidlogins != value) {
                    Interlocked.Exchange(ref invalidlogins, value);
                    RaisePropertyChanged(() => InvalidLogins);
                }
            }
        }

        public int FloodsTriggered {
            get { return floodtiggered; }
            internal set {
                if (floodtiggered != value) {
                    Interlocked.Exchange(ref floodtiggered, value);
                    RaisePropertyChanged(() => FloodsTriggered);
                }
            }
        }

        public int PacketsSent {
            get { return packetssent; }
            internal set {
                if (packetssent != value) {
                    Interlocked.Exchange(ref packetssent, value);
                    RaisePropertyChanged(() => PacketsSent);
                }
            }
        }

        public int PacketsReceived {
            get { return packetsrecv; }
            internal set {
                if (packetsrecv != value) {
                    Interlocked.Exchange(ref packetsrecv, value);
                    RaisePropertyChanged(() => PacketsReceived);
                }
            }
        }

        public override void Reset() {
            base.Reset();
            peakusers = 0;
            joined = 0;
            parted = 0;
            rejected = 0;
            banned = 0;
            captchabanned = 0;
            invalidlogins = 0;
            floodtiggered = 0;
        }
    }
}
