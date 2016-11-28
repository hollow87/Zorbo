using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Zorbo.Interface
{
    public interface IClientId : 
        IEquatable<IClient>, 
        IEquatable<IClientId>,
        IEquatable<IRecord>
    {
        Guid Guid { get; set; }
        IPAddress ExternalIp { get; set; }
    }
}
