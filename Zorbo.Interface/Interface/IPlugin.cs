using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public interface IWPFPlugin : IPlugin
    {
        /// <summary>
        /// Returns the settings GUI for the plugin
        /// </summary>
        System.Windows.Controls.Control GUI { get; }
    }

    public interface IWinFormsPlugin : IPlugin
    {
        /// <summary>
        /// Returns the settings GUI for the plugin
        /// </summary>
        System.Windows.Forms.Control GUI { get; }
    }

    public interface IPlugin
    {
        /// <summary>
        /// Sets the full path to the directory the plugin was loaded from
        /// </summary>
        string Directory { set; }
        /// <summary>
        /// Called when the plugin is loaded
        /// </summary>
        void OnPluginLoaded(IServer server);
        /// <summary>
        /// Called when the plugin is killed
        /// </summary>
        void OnPluginKilled();
        /// <summary>
        /// Occurs when a user enters, exits, answers, or gets banned by the captcha
        /// </summary>
        void OnCaptcha(IClient client, CaptchaEvent @event);
        /// <summary>
        /// Occurs when a user joins the room for the first time, allows adding or overriding server features
        /// </summary>
        /// <returns></returns>
        ServerFeatures OnSendFeatures(IClient client, ServerFeatures features);
        /// <summary>
        /// Occurs when the user joins the room or any vroom (like userlist, avatars, etc)
        /// </summary>
        void OnSendJoin(IClient client);
        /// <summary>
        /// Checks if a user is allowed to join the room before sending the join message to the room
        /// </summary>
        bool OnJoinCheck(IClient client);
        /// <summary>
        /// Occurs when a user is denied logging into the server
        /// </summary>
        void OnJoinRejected(IClient client, RejectReason reason);
        /// <summary>
        /// Occurs after a user is allowed to join the room
        /// </summary>
        void OnJoin(IClient client);
        /// <summary>
        /// Occurs when a user disconnects from the room (with state, if any, supplied at IClient.Disconnect(state))
        /// </summary>
        void OnPart(IClient client, Object state);
        /// <summary>
        /// Occurs before a user moves from one vroom to another, if function returns false, the user is denied
        /// </summary>
        bool OnVroomJoinCheck(IClient client, UInt16 vroom);
        /// <summary>
        /// Occurs after a user is allowed to move from one vroom to another
        /// </summary>
        void OnVroomJoin(IClient client);
        /// <summary>
        /// Occurs when a user leaves from any vroom
        /// </summary>
        void OnVroomPart(IClient client);
        /// <summary>
        /// Occurs when a user performs the help command
        /// </summary>
        /// <param name="client"></param>
        void OnHelp(IClient client);
        /// <summary>
        /// Occurs when a user logs into the server using a password
        /// </summary>
        void OnLogin(IClient client, IPassword password);
        /// <summary>
        /// Occurs when a user registers a password with the server, if function returns false, the password is rejected
        /// </summary>
        bool OnRegister(IClient client, IPassword password);
        /// <summary>
        /// Occurs when a user sends a shared file item, if function returns false, the file is rejected
        /// </summary>
        bool OnFileReceived(IClient client, ISharedFile file);
        /// <summary>
        /// Occurs just before any packet (minus join, login, regiser), some packets (public, emote, personal, private, etc are
        /// overridable by returning false.
        /// </summary>
        bool OnBeforePacket(IClient client, IPacket packet);
        /// <summary>
        /// Occurs after a packet is received or not overriden by a plugin
        /// </summary>
        void OnAfterPacket(IClient client, IPacket packet);
        /// <summary>
        /// Occurs when a user floods the server with packets
        /// </summary>
        bool OnFlood(IClient client, IPacket packet);
        /// <summary>
        /// Occurs when an unhandled exception occurs in any plugin (all plugins receive notification [for debugging?])
        /// </summary>
        void OnError(IErrorInfo error);
    }
}
