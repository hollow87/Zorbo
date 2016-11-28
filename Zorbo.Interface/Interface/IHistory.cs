using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public interface IHistory : IReadOnlyList<IRecord>
    {
        IAdmins Admin { get; }

        IBanned Bans { get; }
        IDnsBanned DnsBans { get; }
        IRangeBanned RangeBans { get; }

        IRecord Add(IClient client);
        void RemoveAll(Predicate<IRecord> search);

        void Clear();
    }
}
