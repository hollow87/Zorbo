using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Hashlinks
{
    public class Channel : IHashlink
    {
        public string Name { get; set; }

        public ushort Port { get; set; }

        public IPAddress LocalIp { get; set; }

        public IPAddress ExternalIp { get; set; }


        public byte[] ToByteArray() {

            using (PacketWriter writer = new PacketWriter()) {
                writer.Write(header);
                writer.Write("CHATCHANNEL");
                writer.Write(ExternalIp == null ? IPAddress.Any : ExternalIp);
                writer.Write(Port);
                writer.Write(LocalIp == null ? IPAddress.Any : LocalIp);
                writer.Write(Name);
                writer.Write((byte)0);

                return writer.ToArray();
            }
        }

        public void FromByteArray(byte[] bytes) {

            using (PacketReader reader = new PacketReader(bytes)) {
                reader.Position = 32;
                ExternalIp = reader.ReadIPAddress();
                Port = reader.ReadUInt16();
                LocalIp = reader.ReadIPAddress();
                Name = reader.ReadString();
            }
        }

        static byte[] header = new byte[20];
    }
}
