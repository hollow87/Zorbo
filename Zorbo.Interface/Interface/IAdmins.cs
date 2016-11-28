using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Zorbo.Interface
{
    public interface IAdmins :
        ICloneable,
        IReadOnlyList<IClient>
    {
        IPasswords Passwords { get; }
    }
}
