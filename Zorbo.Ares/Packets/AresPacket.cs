using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

using Zorbo.Interface;
using Zorbo.Serialization;
using Zorbo.Packets.Formatters;

namespace Zorbo.Packets
{
    public class AresPacket : IPacket
    {
        public virtual byte Id {
            get { return 0; }
            protected set { }
        }

        public virtual IPacket Clone() {

            var formatter = new ClientFormatter();
            byte[] tmp = formatter.Format(this);

            return formatter.Unformat(tmp[2], tmp, 3, tmp.Length - 3);
        }
    }
}
