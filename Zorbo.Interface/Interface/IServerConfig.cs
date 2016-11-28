using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Zorbo.Interface
{
    public interface IServerConfig : INotifyPropertyChanged
    {
        
        /// <summary>
        /// The server's name
        /// </summary>
        string Name { get; }
        /// <summary>
        /// The name of the server bot in the userlist
        /// </summary>
        string BotName { get; }
        /// <summary>
        /// The server's topic
        /// </summary>
        string Topic { get; set; }
        /// <summary>
        /// The server's original topic
        /// </summary>
        string OrgTopic { get; }
        /// <summary>
        /// The port the server will listen for connections on
        /// </summary>
        ushort Port { get; }
        /// <summary>
        /// Default avatar for the server
        /// </summary>
        IAvatar Avatar { get; set; }
        /// <summary>
        /// Default avatar for the server
        /// </summary>
        IAvatar OrgAvatar { get; }
        /// <summary>
        /// The number of clients allowed per ipaddress
        /// </summary>
        ushort MaxClones { get; set; }
        /// <summary>
        /// The maximum number of clients allowed to be in the room at once
        /// </summary>
        ushort MaxClients { get; }
        /// <summary>
        /// The common (often preferred) language of the server
        /// </summary>
        Language Language { get; set; }

        //********* EXPERIMENTAL 

        /// <summary>
        /// True if the server supports HTML, otherwise false
        /// </summary>
        Boolean AllowHtml { get; set; }

        /// <summary>
        /// True if the server allows private messaging, otherwise false
        /// </summary>
        Boolean AllowPrivate { get; set; }

        /// <summary>
        /// True if the server supports file sharing, otherwise false
        /// </summary>
        Boolean AllowSharing { get; set; }

        /// <summary>
        /// True if the server supports packet compression, otherwise false
        /// </summary>
        Boolean AllowCompression { get; set; }

        /// <summary>
        /// True if the server supports packet encryption, otherwise false
        /// </summary>
        Boolean AllowEncryption { get; set; }

        /// <summary>
        /// True if the server supports voice packets, otherwise false
        /// </summary>
        Boolean AllowVoice { get; set; }

        /// <summary>
        /// True if the server supports opus voice packets, otherwise false
        /// </summary>
        Boolean AllowOpusVoice { get; set; }

        //******** END EXPERIMENTAL

        /// <summary>
        /// True to hide IP addresses from other users, otherwise false
        /// </summary>
        Boolean HideIPs { get; set; }
        /// <summary>
        /// True if users who are muzzled can send pm's, otherwise false
        /// </summary>
        Boolean MuzzledPMs { get; set; }
        /// <summary>
        /// True to show the channel on the channel list, otherwise false.
        /// </summary>
        Boolean ShowChannel { get; set; }
        /// <summary>
        /// True to use captcha-like protection on the server for automated clients, otherwise false
        /// </summary>
        Boolean BotProtection { get; set; }
        /// <summary>
        /// True to use IPAddresses, Banned Ranges, and Dns Bans to ban server ip's from udp lists, otherwise false
        /// </summary>
        Boolean UseBansToBanServers { get; set; }
        /// <summary>
        /// The number of days a ban of any kind is effective for (0 for unlimited)
        /// </summary>
        UInt32 BanLength { get; set; }
        /// <summary>
        /// The length of time to keep a password if it doesn't get used, a TimeSpan == 0 for infinite
        /// </summary>
        TimeSpan ExpireOldPasswords { get; set; }
        /// <summary>
        /// Returns the ServerFeatures flags that represent the values in this IServerConfig instance
        /// </summary>
        ServerFeatures GetFeatures();
    }
}
