using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;
using Zorbo.Packets.Formatters;

namespace Zorbo.Packets
{
    class UdpPacket : IPacket
    {
        public virtual byte Id {
            get { return 0; }
            protected set { }
        }

        public virtual IPacket Clone() {

            var formatter = new ClientFormatter();
            byte[] tmp = formatter.Format(this);

            return formatter.Unformat(tmp[0], tmp, 1, tmp.Length - 1);
        }
    }
}
