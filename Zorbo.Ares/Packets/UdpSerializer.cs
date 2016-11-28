using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Packets
{
    sealed class UdpSerializer : PacketSerializer
    {
        public UdpSerializer(IFormatter formatter) :
            base(formatter) { }

        public override byte[] Serialize<T>(T obj) {

            using (var writer = new PacketWriter()) {

                writer.Write(obj.Id);
                writer.WriteObject(obj, this.Formatter);

                return writer.ToArray();
            }
        }
    }
}
