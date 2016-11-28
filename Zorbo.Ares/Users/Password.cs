using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Zorbo.Interface;

namespace Zorbo.Users
{
    public class Password : NotifyObject, IPassword
    {
        string sha1text;

        IClientId clientid;
        AdminLevel level;
        
        public IClientId ClientId {
            get { return clientid; }
            set {
                if (!value.Equals(clientid)) {

                    clientid = value;
                    RaisePropertyChanged(() => ClientId);
                }
            }
        }

        public AdminLevel Level {
            get { return level; }
            set {
                if (level != value) {
                    level = value;
                    RaisePropertyChanged(() => Level);
                }
            }
        }

        public string Sha1Text {
            get { return sha1text; }
            set {
                if (!sha1text.Equals(value)) {
                    sha1text = value;
                    RaisePropertyChanged(() => Sha1Text);
                }
            }
        }


        internal Password(IClient client, string sha1text) {

            this.clientid = client.ClientId;
            this.level = client.Admin;
            this.sha1text = sha1text;
        }

        public Password(IRecord record, AdminLevel level, SecureString pass) {

            this.clientid = record.ClientId;
            this.level = level;
            this.sha1text = CreateSha1Text(pass);
        }

        internal Password(Guid guid, IPAddress ip, string sha1text, AdminLevel level) {

            this.clientid = new ClientId(guid, ip);
            this.level = level;
            this.sha1text = sha1text;
        }

        public Password(Guid guid, IPAddress ip, SecureString pass, AdminLevel level) {

            this.clientid = new ClientId(guid, ip);
            this.level = level;
            this.sha1text = CreateSha1Text(pass);
        }

        public static string CreateSha1Text(string password) {
            using (SHA1 sha1 = SHA1.Create()) {
                return Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }

        public static string CreateSha1Text(SecureString password) {
            using (SHA1 sha1 = SHA1.Create()) {
                return Convert.ToBase64String(sha1.ComputeHash(Encoding.UTF8.GetBytes(password.ToNativeString())));
            }
        }
    }
}
