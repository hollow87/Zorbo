using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public interface IHashlink
    {
        byte[] ToByteArray();
        void FromByteArray(byte[] value);
    }
}
