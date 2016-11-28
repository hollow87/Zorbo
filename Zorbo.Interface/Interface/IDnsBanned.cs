using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Zorbo.Interface
{
    public interface IDnsBanned : IReadOnlyList<Regex>
    {
        bool Add(Regex regex);

        bool Remove(Regex regex);
        bool RemoveAt(Int32 index);

        void Clear();
    }
}
