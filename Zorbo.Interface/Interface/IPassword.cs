using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.ComponentModel;

namespace Zorbo.Interface
{
    public interface IPassword : INotifyPropertyChanged
    {
        IClientId ClientId { get; }
        string Sha1Text { get; set; }
        AdminLevel Level { get; set; }
    }
}
