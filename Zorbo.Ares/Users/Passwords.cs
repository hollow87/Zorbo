using System;
using System.IO;
using System.Net;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Xml.Linq;

using Zorbo.Interface;
using Zorbo.Serialization;
using System.Threading;

namespace Zorbo.Users
{
    public class Passwords : ReadOnlyList<IPassword>, IPasswords
    {
        IServer server;



        public Passwords(IServer server) {
            this.server = server;
        }


        public bool Add(IPassword password) {

            int index = this.FindIndex((s) => s.ClientId.Equals(password.ClientId));

            if (index > -1) {
                this[index].Sha1Text = password.Sha1Text;
                return true;
            }

            Wrapped.Add(password);
            return true;
        }

        public bool Add(IClient client, string password) {

            int index = this.FindIndex((s) => s.ClientId.Equals(client));

            if (index > -1) {
                this[index].Sha1Text = Password.CreateSha1Text(password);
                return true;
            }

            Wrapped.Add(new Password(client, password));
            return true;
        }


        public bool Remove(String password) {
            return RemoveAt(this.FindIndex((s) => s.Sha1Text == password));
        }

        public bool Remove(IPassword password) {
            return RemoveAt(this.FindIndex((s) => s.ClientId.Equals(password.ClientId)));
        }

        public bool RemoveAt(int index) {

            if (index < 0 || index >= this.Count)
                return false;

            Wrapped.RemoveAt(index);
            return true;
        }

        public void RemoveAll(Predicate<IPassword> search) {
            Wrapped.RemoveAll(search);
        }

        public IPassword CheckSha1(IClient client, byte[] password) {

            if (password.Length != 20)
                throw new ArgumentOutOfRangeException("password", "SHA1 password must be 20 bytes in length!");


            using (SHA1 sha1 = SHA1.Create()) {

                foreach (var pass in this) {
                    IPAddress ip = server.ExternalIp;

                    if (client.LocalHost) {
                        if (IPAddress.IsLoopback(client.ExternalIp))
                            ip = IPAddress.Loopback;
                        else
                            ip = server.LocalIp;
                    }

                    using (var writer = new PacketWriter()) {
                        writer.Write(client.Cookie);
                        writer.Write(ip);
                        writer.Write(Convert.FromBase64String(pass.Sha1Text));

                        byte[] c = writer.ToArray();
                        c = sha1.ComputeHash(c);

                        if (password.SequenceEqual(c))
                            return pass;
                    }
                }
            }

            return null;
        }

        public void Clear() {
            Wrapped.Clear();
        }


        public bool Load(string directory) {

            Wrapped.Clear();
            String path = Path.Combine(directory, "Passwords.xml");

            if (File.Exists(path)) {

                FileStream stream = null;

                try {
                    stream = File.Open(path, FileMode.Open, FileAccess.Read);

                    XDocument document = XDocument.Load(stream);

                    var records = from x in document.Root.Elements("Password")
                                  select new Password(
                                      Guid.Parse(x.Element("guid").Value),
                                      IPAddress.Parse(x.Element("address").Value),
                                      x.Element("string").Value, 
                                      (AdminLevel)Int32.Parse(x.Element("level").Value));

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
            String path = Path.Combine(directory, "Passwords.xml");

            try {
                stream = File.Open(path, FileMode.Create, FileAccess.Write);

                var document = new XElement("Passwords",
                               from x in this
                               select new XElement("Password",
                                      new XElement("guid", x.ClientId.Guid.ToString()),
                                      new XElement("address", x.ClientId.ExternalIp.ToString()),
                                      new XElement("string", x.Sha1Text),
                                      new XElement("level", ((int)x.Level).ToString())));

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
