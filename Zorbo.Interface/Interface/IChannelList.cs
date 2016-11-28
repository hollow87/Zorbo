using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Zorbo.Interface
{
    public interface IChannelList : INotifyPropertyChanged
    {
        IMonitor Monitor { get; }

        UInt32 AckIpHits { get; }
        UInt32 AckInfoHits { get; }
        UInt32 SendInfoHits { get; }
        UInt32 SendNodeHits { get; }
        UInt32 CheckFirewallHits { get; }

        Boolean Listing { get; set; }
        Boolean FirewallOpen { get; }
        Boolean FinishedTestingFirewall { get; }

        IReadOnlyList<IServerRecord> Servers { get; }
    }
}
