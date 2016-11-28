using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo
{
    public static class Logging
    {
        public static void Write(string text) {
            try {
#if !SSDEBUG
                Console.Write(text);
#endif
                File.AppendAllText(Path.Combine(Directories.Logging, "ServerLog.txt"), text, Encoding.UTF8);
            }
            catch { }
        }

        public static void WriteLine(string line) {
            try {
#if !SSDEBUG
                Console.WriteLine(line);
#endif
                File.AppendAllText(Path.Combine(Directories.Logging, "ServerLog.txt"), line + Environment.NewLine, Encoding.UTF8);
            }
            catch { }
        }

        public static void WriteLines(string[] lines) {
#if !SSDEBUG
            lines.ForEach((s) => Console.WriteLine(s));
#endif
            try {
                File.AppendAllLines(Path.Combine(Directories.Logging, "ServerLog.txt"), lines);
            }
            catch { }
        }

        public static void WriteCrash(string[] lines) {
#if !SSDEBUG
            lines.ForEach((s) => Console.WriteLine(s));
#endif
            try {
                File.AppendAllLines(Path.Combine(Directories.Logging, "Crash.txt"), lines);
            }
            catch { }
        }


        public static string Read() {
            try {
                return File.ReadAllText(Path.Combine(Directories.Logging, "ServerLog.txt"));
            }
            catch { return String.Empty; }
        }

        public static string[] ReadLines() {
            try {
                return File.ReadAllLines(Path.Combine(Directories.Logging, "ServerLog.txt"));
            }
            catch { return new string[0]; }
        }
    }
}
