using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public interface IFormatter
    {
        byte[] Format(IPacket message);

        IPacket Unformat(byte id, byte[] data);
        IPacket Unformat(byte id, byte[] data, int index, int count);
    }
}
