using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;

namespace Zorbo.Serialization
{
    public class PacketSerializer
    {
        protected IFormatter Formatter {
            get;
            private set;
        }

        public PacketSerializer(IFormatter formatter) {
            Formatter = formatter;
        }

        public virtual byte[] Serialize<T>(T obj) where T : class, IPacket {

            using (var writer = new PacketWriter()) {
                
                writer.Position = 2;

                writer.Write(obj.Id);
                writer.WriteObject(obj, Formatter);

                writer.Position = 0;
                writer.Write((ushort)(writer.Length - 3));

                return writer.ToArray();
            }
        }


        public virtual T Deserialize<T>(byte[] input) where T : class, IPacket {
            return Deserialize<T>(input, 0, input.Length);
        }

        public virtual T Deserialize<T>(byte[] input, int offset, int count) where T : class, IPacket {

            using (var reader = new PacketReader(input, offset, count))
                return reader.ReadObject<T>(Formatter);
        }
    }
}
