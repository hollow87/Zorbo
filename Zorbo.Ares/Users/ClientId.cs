using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;

namespace Zorbo.Users
{
    public class ClientId : IClientId
    {
        public Guid Guid { get; set; }

        public IPAddress ExternalIp { get; set; }


        public ClientId(IClient client)
            : this(client.ClientId.Guid, client.ClientId.ExternalIp) {
        }

        public ClientId(Guid guid, IPAddress address) {
            Guid = guid;
            ExternalIp = address;
        }

        public bool Equals(IClient other) {

            return (other != null && (
                other.Guid.Equals(Guid) ||
                other.ExternalIp.Equals(ExternalIp)));
        }

        public bool Equals(IClientId other) {

            return (other != null && (
                other.Guid.Equals(Guid) ||
                other.ExternalIp.Equals(ExternalIp)));
        }

        public bool Equals(IRecord other) {

            return (other != null && (
                other.ClientId.Guid.Equals(Guid) ||
                other.ClientId.ExternalIp.Equals(ExternalIp)));
        }
    }
}
