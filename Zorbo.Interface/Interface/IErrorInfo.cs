using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public interface IErrorInfo
    {
        string Name { get; }
        string Method { get; }

        Exception Exception { get; }
    }
}
