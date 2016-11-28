using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using Zorbo.Serialization;

using Zorbo.SharpZip.Zip.Compression;
using Zorbo.SharpZip.Zip.Compression.Streams;

namespace Zorbo.Hashlinks
{
    public static class HashConvert
    {
        public static T FromHashlinkString<T>(string base64hash) where T : IHashlink {

            base64hash = base64hash.TrimStart('\\');

            if (base64hash.StartsWith("arlnk://"))
                base64hash = base64hash.Substring(8);

            return FromHashlinkArray<T>(Convert.FromBase64String(base64hash));
        }

        public static T FromHashlinkString<T>(string base64hash, T hashlink) where T : IHashlink {

            base64hash = base64hash.TrimStart('\\');

            if (base64hash.StartsWith("arlnk://"))
                base64hash = base64hash.Substring(8);

            return FromHashlinkArray<T>(Convert.FromBase64String(base64hash), hashlink);
        }

        public static T FromHashlinkArray<T>(byte[] hashbytes) where T : IHashlink {
            T ret = default(T);
            
            using (var stream = new MemoryStream(d67(hashbytes, 28435)))
            using (var inflater = new InflaterInputStream(stream)) {

                stream.Position = 0;

                using (var output = new MemoryStream()) {
                    int count = 0;
                    byte[] buffer = new byte[2048];

                    while ((count = inflater.Read(buffer, 0, buffer.Length)) > 0)
                        output.Write(buffer, 0, count);

                    output.Flush();
                    output.Position = 0;

                    byte[] tmp = new byte[output.Length];
                    output.Read(tmp, 0, tmp.Length);

                    ret = Activator.CreateInstance<T>();
                    ret.FromByteArray(tmp);
                }
            }

            return ret;
        }

        public static T FromHashlinkArray<T>(byte[] hashbytes, T hashlink) where T : IHashlink {

            if (hashlink == null)
                throw new ArgumentNullException("hashlink");

            using (var stream = new MemoryStream(d67(hashbytes, 28435)))
            using (var inflater = new InflaterInputStream(stream)) {

                stream.Position = 0;

                using (var output = new MemoryStream()) {
                    int count = 0;
                    byte[] buffer = new byte[2048];

                    while ((count = inflater.Read(buffer, 0, buffer.Length)) > 0)
                        output.Write(buffer, 0, count);

                    output.Flush();
                    output.Position = 0;

                    byte[] tmp = new byte[output.Length];
                    output.Read(tmp, 0, tmp.Length);

                    hashlink.FromByteArray(tmp);
                }
            }

            return hashlink;
        }


        public static byte[] ToHashlinkArray<T>(T hashlink) where T : IHashlink {
            return ToHashlinkArray<T>(hashlink);
        }

        public static byte[] ToHashlinkArray<T>(T hashlink, int level) where T : IHashlink {

            using (var stream = new MemoryStream())
            using (var deflater = new DeflaterOutputStream(stream, new Deflater(level))) {

                byte[] buffer = hashlink.ToByteArray();

                deflater.Write(buffer, 0, buffer.Length);
                deflater.Finish();

                return e67(stream.ToArray(), 28435);
            }
        }


        public static string ToHashlinkString<T>(T hashlink) where T : IHashlink {
            return ToHashlinkString<T>(hashlink, 9);
        }

        public static string ToHashlinkString<T>(T hashlink, int level) where T : IHashlink {

            using (var stream = new MemoryStream())
            using (var deflater = new DeflaterOutputStream(stream, new Deflater(level))) {

                byte[] buffer = hashlink.ToByteArray();

                deflater.Write(buffer, 0, buffer.Length);
                deflater.Finish();

                return Convert.ToBase64String(e67(stream.ToArray(), 28435));
            }
        }


        public static byte[] d67(byte[] data, int b) {
            byte[] buffer = (byte[])data.Clone();

            for (int i = 0; i < data.Length; i++) {
                buffer[i] = (byte)((data[i] ^ (b >> 8)) & 255);
                b = ((b + data[i]) * 23219 + 36126) & 65535;
            }

            return buffer;
        }

        public static byte[] e67(byte[] data, int b) {
            byte[] buffer = (byte[])data.Clone();

            for (int i = 0; i < data.Length; i++) {
                buffer[i] = (byte)((data[i] ^ (b >> 8)) & 255);
                b = ((buffer[i] + b) * 23219 + 36126) & 65535;
            }

            return buffer;
        }
    }
}
