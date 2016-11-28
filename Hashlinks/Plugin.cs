using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Zorbo;//some handy extensions and things
using Zorbo.Packets;
using Zorbo.Packets.Ares;
using Zorbo.Interface;
using Zorbo.Hashlinks;

namespace Hashlinks
{
    public class Hashlinks : IPlugin
    {
        string mydir = "";
        IServer server = null;

        public string Directory {
            get { return mydir; }
            set { mydir = value; }
        }


        public void OnPluginLoaded(IServer server) {
            this.server = server;
            this.server.SendAnnounce("Hashlinks plugin has been loaded!!");
        }

        public void OnPluginKilled() {
            this.server.SendAnnounce("Hashlinks plugin has been unloaded!!");
        }

        public void OnCaptcha(IClient client, CaptchaEvent @event) {
        }

        public void OnJoinRejected(IClient client, RejectReason reason) {
        }

        public ServerFeatures OnSendFeatures(IClient client, ServerFeatures features) {
            return features;
        }

        public void OnSendJoin(IClient client) {
        }

        public bool OnJoinCheck(IClient client) {
            return true;
        }

        public void OnJoin(IClient client) {
        }

        public void OnPart(IClient client, object state) {
        }

        public bool OnVroomJoinCheck(IClient client, ushort vroom) {
            return true;
        }

        public void OnVroomJoin(IClient client) {
        }

        public void OnVroomPart(IClient client) {
        }

        public void OnHelp(IClient client) {

        }

        public void OnLogin(IClient client, IPassword password) {
        }

        public bool OnRegister(IClient client, IPassword password) {
            return true;
        }

        public bool OnFileReceived(IClient client, ISharedFile file) {
            return true;
        }

        public bool OnBeforePacket(IClient client, IPacket packet) {

            switch ((AresId)packet.Id) {
                case AresId.MSG_CHAT_CLIENT_PUBLIC:
                    ClientPublic pub = (ClientPublic)packet;

                    if (pub.Message.StartsWith("#hashlink")) {

                        server.SendAnnounce("\\\\arlnk://" + 
                            HashConvert.ToHashlinkString(new Channel() {
                                Name = server.Config.Name,
                                Port = server.Config.Port,
                                LocalIp = server.LocalIp,
                                ExternalIp = server.ExternalIp,
                            }));
                    }
                    else if (pub.Message.Length > 9 && pub.Message.StartsWith("#decrypt ")) {

                        var hash = HashConvert.FromHashlinkString<Channel>(pub.Message.Substring(9));

                        server.SendAnnounce(String.Format("Name: {0}", hash.Name));
                        server.SendAnnounce(String.Format("Port: {0}", hash.Port));
                        server.SendAnnounce(String.Format("External Ip: {0}", hash.ExternalIp));
                        server.SendAnnounce(String.Format("Local Ip: {0}", hash.LocalIp));
                    }

                    break;
            }

            return true;
        }

        public void OnAfterPacket(IClient client, IPacket packet) {
        }

        public bool OnFlood(IClient client, IPacket packet) {
            return true;
        }

        public void OnError(IErrorInfo error) {
        }
    }
}
