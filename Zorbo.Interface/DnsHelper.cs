using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using Zorbo.Interface;

namespace Zorbo
{
    public delegate void DnsEntryCallback();

    public static class DnsHelper
    {
        public static void Resolve(IPAddress ip, Action<IPAddress, IPHostEntry> callback) {
            IPAddress tmpip = new IPAddress(ip.GetAddressBytes());

            try {
                Dns.BeginGetHostEntry(tmpip,
                    (ar) => {
                        IPHostEntry ret = null;

                        try {
                            ret = Dns.EndGetHostEntry(ar);
                        }
                        catch (SocketException) { }
                        catch (Exception ex) { OnDnsError(tmpip, ex); }
                        finally {
                            ((Action<IPAddress, IPHostEntry>)ar.AsyncState)(tmpip, ret);
                        }

                    }, callback);
            }
            catch (SocketException) {
                callback(tmpip, null);
            }
            catch (Exception ex) {
                OnDnsError(tmpip, ex);
                callback(tmpip, null);
            }
        }

        public static void Resolve(IRecord record, Action<IRecord, IPHostEntry> callback) {
            try {
                Dns.BeginGetHostEntry(
                    record.ClientId.ExternalIp, 
                    ResolveCallback, 
                    new object[] { record, callback });
            }
            catch (Exception ex) {
                OnDnsError(record.ClientId.ExternalIp, ex);
                callback(record, null);
            }

        }

        private static void ResolveCallback(IAsyncResult ar) {
            IPHostEntry ret = null;
            object[] args = (object[])ar.AsyncState;
            
            var record = (IRecord)args[0];
            var callback = (Action<IRecord, IPHostEntry>)args[1];

            try {
                ret = Dns.EndGetHostEntry(ar);
            }
            catch (Exception ex) { OnDnsError(record.ClientId.ExternalIp, ex); }
            finally { callback(record, ret); }
        }


        private static void OnDnsError(IPAddress ip, Exception ex) {
#if DEBUG
            Console.WriteLine("Dns Error ({0}): {1}", ip, ex.Message);
#endif
            DnsError?.Invoke(ip, new ExceptionEventArgs(ex, ip));
        }


        public static event DnsErrorHandler DnsError;
        public delegate void DnsErrorHandler(object sender, ExceptionEventArgs e);
    }
}
