using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public interface IPacket
    {
        byte Id { get; }
        IPacket Clone();
    }
}
