using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public interface IRecord :
        IEquatable<IClient>,
        IEquatable<IClientId>,
        IEquatable<IRecord>
    {
        IClientId ClientId { get; }

        string Name { get; }
        string DnsName { get; }

        bool Muzzled { get; set; }
        bool Trusted { get; set; }

        DateTime LastSeen { get; }
    }
}
