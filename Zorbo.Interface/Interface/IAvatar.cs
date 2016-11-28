using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Zorbo.Interface
{
    public interface IAvatar : IEquatable<IAvatar>
    {
        byte[] SmallBytes { get; }
        byte[] LargeBytes { get; }
    }
}
