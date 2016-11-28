using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Zorbo.Interface
{
    public interface IServerRecord : INotifyPropertyChanged
    {
        ushort Port { get; }
        
        uint AckCount { get; }
        uint TryCount { get; }
        
        DateTime LastAcked { get; }
        DateTime LastSendIPs { get; }
        DateTime LastAskedFirewall { get; }

        IPAddress ExternalIp { get; }
    }
}
