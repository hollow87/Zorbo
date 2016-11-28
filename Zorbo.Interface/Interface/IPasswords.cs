using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public interface IPasswords : IReadOnlyList<IPassword>
    {
        bool Add(IPassword password);
        bool Add(IClient client, String password);

        bool Remove(String password);
        bool Remove(IPassword password);

        bool RemoveAt(Int32 index);
        void RemoveAll(Predicate<IPassword> search);

        IPassword CheckSha1(IClient client, byte[] password);

        void Clear();
    }
}
