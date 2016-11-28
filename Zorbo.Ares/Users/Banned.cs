using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
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
    public class Banned : ReadOnlyList<IClientId>, IBanned
    {
        IServer server;

        public Banned(IServer server) {
            this.server = server;
            Load(Directories.Cache);
        }

        public bool Add(IClientId banned) {

            if (this.Contains((s) => s.Equals(banned)))
                return false;

            Wrapped.Add(banned);
            return true;
        }

        public bool Remove(IClientId banned) {

            int index = this.FindIndex((s) => s.Equals(banned));
            return RemoveAt(index);
        }

        public bool Remove(Predicate<IClientId> search) {

            int index = this.FindIndex(search);
            if (index < 0) return false;

            return RemoveAt(index);
        }

        public bool RemoveAt(Int32 index) {

            if (index < 0 || index >= this.Count)
                return false;

            Wrapped.RemoveAt(index);
            return true;
        }

        public void RemoveAll(Predicate<IClientId> search) {
            Wrapped.RemoveAll(search);
        }


        public void Clear() {
            Wrapped.Clear();
        }


        public bool Load(string directory) {
            
            Wrapped.Clear();
            String path = Path.Combine(directory, "Bans.xml");

            if (File.Exists(path)) {

                FileStream stream = null;

                try {
                    stream = File.Open(path, FileMode.Open, FileAccess.Read);

                    XDocument document = XDocument.Load(stream);

                    var records = from x in document.Root.Elements("Banned")
                                  select new ClientId(
                                      Guid.Parse(x.Element("guid").Value),
                                      IPAddress.Parse(x.Element("address").Value));

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
            String path = Path.Combine(directory, "Bans.xml");

            try {
                stream = File.Open(path, FileMode.Create, FileAccess.Write);

                var document = new XElement("Bans",
                               from x in this
                               select new XElement("Banned",
                                      new XElement("guid", x.Guid.ToString()),
                                      new XElement("address", x.ExternalIp.ToString())));

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
