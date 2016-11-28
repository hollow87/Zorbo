using Commands.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Zorbo.Interface;
using Zorbo.Packets;
using Zorbo.Packets.Ares;

namespace Commands
{
    public class Commands : IWPFPlugin
    {
        string mydir = string.Empty;
        IServer server = null;

        public string Directory {
            get { return mydir; }
            set { mydir = value; }
        }

        public Control GUI {
            get { return null; }
        }

        public void OnPluginLoaded(IServer server) {
            this.server = server;
            Processor.LoadHelp();
            server.SendAnnounce("Zorbo Commands Plugin Loaded");
        }

        public void OnPluginKilled() {
        }

        public bool OnBeforePacket(IClient client, IPacket packet) {

            switch ((AresId)packet.Id) {

                case AresId.MSG_CHAT_CLIENT_PUBLIC:
                    ClientPublic pub = (ClientPublic)packet;

                    if (!String.IsNullOrEmpty(pub.Message)) {

                        if (pub.Message.StartsWith("#"))
                            if (!Processor.HandleCommand(server, client, pub.Message.Substring(1)))
                                return false; //hide text
                    }
                    break;
                case AresId.MSG_CHAT_CLIENT_COMMAND:
                    Command command = (Command)packet;

                    if (!Processor.HandleCommand(server, client, command.Message))
                        return false;

                    break;
            }
            return true;
        }

        public void OnAfterPacket(IClient client, IPacket packet) {
        }

        public void OnCaptcha(IClient client, CaptchaEvent @event) {
        }

        public void OnHelp(IClient client) {
            Processor.SendHelp(server, client);
        }

        public ServerFeatures OnSendFeatures(IClient client, ServerFeatures features) {
            return features;
        }

        public void OnSendJoin(IClient client) {
        }

        public void OnJoin(IClient client) {
        }

        public bool OnJoinCheck(IClient client) {
            return true;
        }

        public void OnJoinRejected(IClient client, RejectReason reason) {
        }

        public void OnPart(IClient client, object state) {
        }

        public void OnLogin(IClient client, IPassword password) {
        }

        public bool OnRegister(IClient client, IPassword password) {
            return true;
        }

        public void OnVroomJoin(IClient client) {
            server.SendAnnounce(client, string.Format(Strings.VroomNotice, client.Vroom));
        }

        public bool OnVroomJoinCheck(IClient client, ushort vroom) {
            return true;
        }

        public void OnVroomPart(IClient client) {
        }

        public bool OnFileReceived(IClient client, ISharedFile file) {
            return true;
        }

        public bool OnFlood(IClient client, IPacket packet) {
            return true;
        }

        public void OnError(IErrorInfo error) {
        }
    }
}
