using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Zorbo;//some handy extensions and things
using Zorbo.Interface;
using Zorbo.Packets;
using Zorbo.Packets.Ares;

using System.Text.RegularExpressions;

namespace Motd
{
    public class Motd : IPlugin
    {
        string mydir = "";
        string[] motdlines;

        IServer server = null;

        public string Directory {
            get { return mydir; }
            set { mydir = value; }
        }


        private void LoadMotd() {
            Stream stream = null;
            StreamReader reader = null;

            try {
                string file = Path.Combine(Directory, "motd.txt");

                if (File.Exists(file)) {
                    stream = File.Open(file, FileMode.Open, FileAccess.Read);
                    reader = new StreamReader(stream);

                    server.SendAnnounce(String.Format("Motd loaded [{0} bytes]", stream.Length));

                    motdlines = reader
                        .ReadToEnd()
                        .Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                }
            }
            catch { }
            finally {
                if (reader != null) {
                    reader.Close();
                    reader.Dispose();
                }

                if (stream != null)
                    stream.Dispose();
            }
        }

        private void ShowMotd(IClient client) {

            if (motdlines != null) {

                bool canHtml = 
                    server.Config.AllowHtml &&
                   (client.Features & ClientFeatures.HTML) == ClientFeatures.HTML;

                if (canHtml)
                    client.SendPacket(new ServerHtml("<!--MOTDSTART-->"));

                for (int i = 0; i < motdlines.Length; i++) {
                    string line = motdlines[i].Trim();

                    if (SendMotdLine(client, line, canHtml))
                        server.SendAnnounce(client, ReplaceVars(client, line));
                }

                if (canHtml)
                    client.SendPacket(new ServerHtml("<!--MOTDEND-->"));
            }
        }

        private bool SendMotdLine(IClient client, string input, bool canHtml) {

            Regex regex = new Regex("^\\[(?<tag>.+?)=(?<val>.+?)\\]$", RegexOptions.IgnoreCase);
            Match match = regex.Match(input);

            if (match.Success) {
                var group = match.Groups[1];

                string tag = match.Groups[1].Value.ToLower();
                string value = match.Groups[2].Value.Trim();

                string html = string.Empty;

                switch (group.Value.ToLower()) {
                    case "youtube":
                        if (canHtml) {
                            client.SendPacket(new ServerHtml("<!--EMBEDYOUTUBE:" + value + "-->"));
                            html = embed.Replace("LINK", value);
                            client.SendPacket(new ServerHtml(html));
                        }
                        return false;
                    case "image":
                        if (canHtml) {
                            html = "<img src=\"" + value + "\" style=\"max-width: 420px; max-height: 420px;\" alt=\"\" />";
                            client.SendPacket(new ServerHtml(html));
                        }
                        return false;
                    case "poster":
                        if (canHtml) {
                            html = "<img src=\"" + value + "\" style=\"max-width: 75%; display: block; margin-left: auto; margin-right: auto;\" alt=\"\" />";
                            client.SendPacket(new ServerHtml(html));
                        }
                        return false;
                    case "audio":
                        if (canHtml) {
                            html = "<audio src=\"" + value + "\" autoplay />";
                            client.SendPacket(new ServerHtml(html));
                        }
                        return false;
                    case "video":
                        if (canHtml) {
                            html = "<video src=\"" + value + "\" width=\"420\" height=\"315\" controls />";
                            client.SendPacket(new ServerHtml(html));
                        }
                        return false;
                }

            }

            return true;
        }

        private string ReplaceVars(IClient client, string input) {
            Regex regex = new Regex("(?<tag>\\+n|\\+ip|\\+time|\\+dns|\\+vroom)", RegexOptions.IgnoreCase);
            Match match = regex.Match(input);

            while (match.Success) {
                var group = match.Groups[1];
                string replace = string.Empty;

                switch (group.Value.ToLower()) {
                    case "+n":
                        replace = client.Name;
                        break;
                    case "+id":
                        replace = client.Id.ToString();
                        break;
                    case "+ip":
                        replace = client.ExternalIp.ToString();
                        break;
                    case "+time":
                        replace = TimeBank.CurrentTime.ToShortTimeString();
                        break;
                    case "+date":
                        replace = TimeBank.CurrentTime.ToShortDateString();
                        break;
                    case "+dns":
                        replace = (client.DnsEntry != null) ? client.DnsEntry.HostName : "error";
                        break;
                    case "+vroom":
                        replace = client.Vroom.ToString();
                        break;
                }

                input = input.Remove(match.Groups[1].Index, match.Groups[1].Length);
                input = input.Insert(group.Index, replace);

                match = regex.Match(input, group.Index + replace.Length);
            }

            return input;
        }


        public void OnPluginLoaded(IServer server) {
            this.server = server;
            this.server.SendAnnounce("Motd plugin has been loaded!!");

            this.LoadMotd();
        }

        public void OnPluginKilled() {
            this.server.SendAnnounce("Motd plugin has been unloaded!!");
        }

        public void OnCaptcha(IClient client, CaptchaEvent @event) {
        }

        public ServerFeatures OnSendFeatures(IClient client, ServerFeatures features) {
            return features;
        }

        public void OnSendJoin(IClient client) {
            
        }

        public bool OnJoinCheck(IClient client) {
            return true;
        }

        public void OnJoinRejected(IClient client, RejectReason reason) {
        }

        public void OnJoin(IClient client) {
            ShowMotd(client);
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
                    ClientPublic text = (ClientPublic)packet;

                    if (text.Message.StartsWith("#"))
                        HandleCommand(client, text.Message.Substring(1));

                    break;
                case AresId.MSG_CHAT_CLIENT_COMMAND:
                    Command command = (Command)packet;
                    HandleCommand(client, command.Message);

                    break;
            }

            return true;
        }

        private void HandleCommand(IClient client, String text) {
            if (client.Admin >= AdminLevel.Admin) {
                if (text.StartsWith("loadmotd"))
                    LoadMotd();

                else if (text.StartsWith("viewmotd"))
                    ShowMotd(client);
            }
        }

        public void OnAfterPacket(IClient client, IPacket packet) {
        }

        public bool OnFlood(IClient client, IPacket packet) {
            return true;
        }

        public void OnError(IErrorInfo error) {
        }


        private const string embed = "<div style=\"margin-left: 2px;\"><object width=\"420\" height=\"315\">" +
                           "<param name=\"movie\" value=\"https://www.youtube.com/v/LINK?version=3&autoplay=0\"></param>" +
                           "<param name=\"allowScriptAccess\" value=\"always\"></param>" +
                           "<embed src=\"https://www.youtube.com/v/LINK?version=3&autoplay=0\" " +
                           "type=\"application/x-shockwave-flash\" " +
                           "allowscriptaccess=\"always\" " +
                           "wmode=\"opaque\" " +
                           "width=\"420\" height=\"315\"></embed>" +
                           "</object></div>";
    }
}
