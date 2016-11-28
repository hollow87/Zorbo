using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Linq;

using Zorbo.Interface;
using System.Threading;

namespace Zorbo.Users
{
    public class DnsBanned : ReadOnlyList<Regex>, IDnsBanned
    {
        public DnsBanned() : base() {
            Load(Directories.Cache);
        }

        public bool Add(Regex regex) {

            if (Wrapped.Contains((s) => s.Equals(regex)))
                return false;

            Wrapped.Add(regex);
            return true;
        }

        public bool Remove(Regex regex) {

            if (!Wrapped.Contains((s) => s.Equals(regex)))
                return false;

            return Wrapped.Remove(regex);
        }

        public bool RemoveAt(Int32 index) {

            if (index < 0 || index >= Count)
                return false;

            Wrapped.RemoveAt(index);
            return true;
        }

        public void Clear() {
            Wrapped.Clear();
        }

        public bool Load(string directory) {

            Wrapped.Clear();
            String path = Path.Combine(directory, "DnsBans.xml");

            if (File.Exists(path)) {

                FileStream stream = null;

                try {
                    stream = File.Open(path, FileMode.Open, FileAccess.Read);

                    XDocument document = XDocument.Load(stream);

                    var records = from x in document.Root.Elements("pattern")
                                  select new Regex(x.Value);

                    records.ForEach((record) => Wrapped.Add(record));
                    return true;
                }
                catch { }
                finally {
                    if (stream != null) {
                        stream.Close();
                        stream.Dispose();
                    }
                }
            }

            return false;
        }

        public bool Save(string directory) {
            Thread.BeginCriticalRegion();

            FileStream stream = null;
            String path = Path.Combine(directory, "DnsBans.xml");

            try {
                stream = File.Open(path, FileMode.Create, FileAccess.Write);

                var document = new XElement("DnsBans",
                               from x in this
                               select new XElement("pattern", x.ToString()));

                document.Save(stream);
                return true;
            }
            catch { }
            finally {
                if (stream != null) {
                    stream.Close();
                    stream.Dispose();
                }
            }

            Thread.EndCriticalRegion();
            return false;
        }
    }
}
