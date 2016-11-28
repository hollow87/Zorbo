using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Xml.Linq;

using Zorbo.Interface;
using System.Threading;

namespace Zorbo.Users
{
    public class Record : IRecord
    {
        public IClientId ClientId { get; set; }

        public string Name { get; set; }

        public string DnsName { get; set; }

        public bool Muzzled { get; set; }

        public bool Trusted { get; set; }

        public DateTime LastSeen { get; set; }

        public bool Equals(IClient other) {
            return (other != null && ClientId.Equals(other.ClientId));
        }

        public bool Equals(IClientId other) {
            return (other != null && ClientId.Equals(other));
        }

        public bool Equals(IRecord other) {
            return (other != null && ClientId.Equals(other.ClientId));
        }
    }


    public class History : ReadOnlyList<IRecord>, IHistory
    {
        IServer server;

        Admin admins;

        Banned bans;
        DnsBanned dnsBans;
        RangeBanned rBans;

        DateTime lastSave;


        public Admin Admin { 
            get { return admins; }
            private set { admins = value; }
        }

        public Banned Bans { 
            get { return bans; }
            private set { bans = value; }
        }

        public DnsBanned DnsBans { 
            get { return dnsBans; }
            private set { dnsBans = value; }
        }

        public RangeBanned RangeBans { 
            get { return rBans; }
            private set { rBans = value; }
        }


        #region " IHistory "

        IAdmins IHistory.Admin {
            get { return admins; }
        }

        IBanned IHistory.Bans {
            get { return bans; }
        }

        IDnsBanned IHistory.DnsBans {
            get { return dnsBans; }
        }

        IRangeBanned IHistory.RangeBans {
            get { return rBans; }
        }

        #endregion


        public History(IServer server) {

            this.server = server;
            this.lastSave = TimeBank.CurrentTime;
        }


        public IRecord Add(IClient client) {

            var record = (Record)this.Find((s) => s.Equals(client));

            if (record == null) {
                record = new Record();
                Wrapped.Add(record);
            }

            record.ClientId = client.ClientId;
            record.Name = client.Name;
            record.DnsName = (client.DnsEntry != null) ? client.DnsEntry.HostName : record.DnsName;
            record.Muzzled = client.Muzzled;
            record.LastSeen = TimeBank.CurrentTime;

            if (record.Trusted && client.IsCaptcha)
                record.Trusted = false;

            if (TimeBank.CurrentTime.Subtract(lastSave).TotalMinutes > 30) {

                lastSave = TimeBank.CurrentTime;
                ThreadPool.QueueUserWorkItem((state) => Save());
            }

            return record;
        }

        public void RemoveAll(Predicate<IRecord> search) {
            Wrapped.RemoveAll(search);
        }


        public void Clear() {
            Wrapped.Clear();
        }


        public bool Load() {

            Wrapped.Clear();
            String path = Path.Combine(Directories.Cache, "Records.xml");

            if (File.Exists(path)) {

                FileStream stream = null;

                try {
                    stream = File.Open(path, FileMode.Open, FileAccess.Read);

                    XDocument document = XDocument.Load(stream);

                    var records = from x in document.Root.Elements("Record")
                                  select new Record() {
                                      ClientId = new ClientId(
                                          Guid.Parse(x.Element("guid").Value), 
                                          IPAddress.Parse(x.Element("address").Value)),
                                      Name = x.Element("name").Value,
                                      DnsName = x.Element("dnshost").Value,
                                      Trusted = bool.Parse(x.Element("trusted").Value),
                                      LastSeen = DateTime.FromBinary(long.Parse(x.Element("lastseen").Value))
                                  };

                    records.ForEach((record) => Wrapped.Add(record));

                    this.admins = new Admin(server);
                    this.bans = new Banned(server);
                    this.dnsBans = new DnsBanned();
                    this.rBans = new RangeBanned();

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

            this.admins = new Admin(server);
            this.bans = new Banned(server);
            this.dnsBans = new DnsBanned();
            this.rBans = new RangeBanned();

            return false;
        }

        public bool Save() {
            Thread.BeginCriticalRegion();

            FileStream stream = null;
            String path = Path.Combine(Directories.Cache, "Records.xml");

            try {
                stream = File.Open(path, FileMode.Create, FileAccess.Write);

                var document = new XElement("Records",
                               from x in this
                               select new XElement("Record",
                                      new XElement("guid", x.ClientId.Guid.ToString()),
                                      new XElement("name", x.Name),
                                      new XElement("dnshost", x.DnsName),
                                      new XElement("address", x.ClientId.ExternalIp.ToString()),
                                      new XElement("trusted", x.Trusted.ToString()),
                                      new XElement("lastseen", x.LastSeen.ToBinary().ToString())));

                document.Save(stream);

                this.admins.Save();
                this.bans.Save(Directories.Cache);
                this.dnsBans.Save(Directories.Cache);
                this.rBans.Save(Directories.Cache);

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
