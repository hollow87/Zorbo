using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Zorbo.SharpZip.Zip.Compression.Streams;

namespace Zorbo
{
    public static class Zlib
    {
        public static byte[] Compress(byte[] input) {

            using (var mem = new MemoryStream()) {
                using (var zlib = new DeflaterOutputStream(mem)) {
                    zlib.Write(input, 0, input.Length);
                    zlib.Flush();
                    zlib.Close();
                    return mem.ToArray();
                }
            }
        }

        public static void Compress(Stream stream, byte[] input) {

            using (var zlib = new DeflaterOutputStream(stream)) {
                zlib.IsStreamOwner = false;
                zlib.Write(input, 0, input.Length);
                zlib.Flush();
                zlib.Close();
            }
        }

        public static byte[] Decompress(byte[] input) {

            using (var mem = new MemoryStream()) {
                using (var zlib = new InflaterInputStream(mem)) {
                    zlib.Write(input, 0, input.Length);
                    zlib.Flush();
                    zlib.Close();
                    return mem.ToArray();
                }
            }
        }

        public static void Decompress(Stream stream, byte[] input) {

            using (var zlib = new InflaterInputStream(stream)) {
                zlib.IsStreamOwner = false;
                zlib.Write(input, 0, input.Length);
                zlib.Flush();
                zlib.Close();
            }
        }
    }
}
