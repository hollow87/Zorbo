using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public interface IBanned : IReadOnlyList<IClientId>
    {
        bool Add(IClientId banned);
        bool Remove(IClientId banned);
        bool Remove(Predicate<IClientId> search);
        bool RemoveAt(Int32 index);
        void RemoveAll(Predicate<IClientId> search);
        void Clear();
    }
}
