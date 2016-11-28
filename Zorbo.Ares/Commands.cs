using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Zorbo.Interface;
using Zorbo.Plugins;
using Zorbo.Resources;
using Zorbo.Hashlinks;
using Zorbo.Packets;
using Zorbo.Packets.Ares;
using Zorbo.Users;

namespace Zorbo
{
    public static class Commands
    {
        private static void SendHelp(IServer server, IClient client) {

            server.SendAnnounce(client, "[Zorbo Help Display]");
            server.SendAnnounce(client, "#help - initiates the help display");
            server.SendAnnounce(client, "#login <password> - attempts to login as an administrator");
            server.SendAnnounce(client, "#register <password> - registers a password with the server");

            ((PluginHost)server.PluginHost).OnHelp(client);
        }


        internal static void HandleLogin(IServer server, IClient client, string password) {

            password = Password.CreateSha1Text(password);
            var pass = server.History.Admin.Passwords.Find((s) => s.Sha1Text == password);

            if (pass == null) {
                ((AresServer)server).Stats.InvalidLogins++;
                server.SendAnnounce(client, Strings.InvalidLogin);
            }
            else {
                pass.ClientId.Guid = client.Guid;
                pass.ClientId.ExternalIp = client.ExternalIp;

                server.SendAnnounce(client, String.Format(Strings.LoggedIn, pass.Level));
                client.Admin = pass.Level;

                ((PluginHost)server.PluginHost).OnLogin(client, pass);
            }
        }

        internal static void HandleAutoLogin(IServer server, IClient client, byte[] password) {

            var pass = server.History.Admin.Passwords.CheckSha1(client, password);

            if (pass == null) {
                ((AresServer)server).Stats.InvalidLogins++;
                server.SendAnnounce(client, Strings.InvalidLogin);
            }
            else {
                pass.ClientId.Guid = client.Guid;
                pass.ClientId.ExternalIp = client.ExternalIp;

                server.SendAnnounce(client, String.Format(Strings.LoggedIn, pass.Level));
                client.Admin = pass.Level;

                ((PluginHost)server.PluginHost).OnLogin(client, pass);
            }
        }

        internal static void HandleRegister(IServer server, IClient client, string password) {
            var passes = server.History.Admin.Passwords;
            var pass = new Password(client, password);

            if (passes.Add(pass)) {

                server.SendAnnounce(client, Strings.Registered);
                ((PluginHost)server.PluginHost).OnRegister(client, pass);
            }
        }


        internal static bool HandlePreCommand(IServer server, IClient client, string text) {
            string cmd = String.Empty;
            string args = String.Empty;

            int sep = text.IndexOf('\x20');

            if (sep == -1)
                cmd = text;
            else {
                cmd = text.Substring(0, sep);

                if (text.Length > (sep + 1))
                    args = text.Substring(sep + 1);
            }

            switch (cmd) {
                case "help":
                    SendHelp(server, client);
                    break;
                case "login":
                    HandleLogin(server, client, args);
                    return true;
                case "register":
                    HandleRegister(server, client, args);
                    return true;
            }

            return false;
        }
    }
}
