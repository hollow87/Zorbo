using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Zorbo.Interface
{
    public interface IServer : INotifyPropertyChanged
    {
        Boolean Running { get; }

        IPAddress LocalIp { get; }
        IPAddress ExternalIp { get; }

        IHistory History { get; }
        IServerStats Stats { get; }
        IServerConfig Config { get; }
        IChannelList Channels { get; }
        IPluginHost PluginHost { get; }

        IReadOnlyList<IClient> Users { get; }
        IList<IFloodRule> FloodRules { get; }

        IClient FindUser(Predicate<IClient> client);

        void SendPacket(IPacket packet);
        void SendPacket(IClient user, IPacket packet);
        void SendPacket(Predicate<IClient> match, IPacket packet);

        void SendText(string sender, string text);
        void SendText(IClient sender, string text);
        void SendText(string target, string sender, string text);
        void SendText(IClient target, string sender, string text);
        void SendText(IClient target, IClient sender, string text);
        void SendText(Predicate<IClient> match, string sender, string text);
        void SendText(Predicate<IClient> match, IClient sender, string text);

        void SendEmote(string sender, string text);
        void SendEmote(IClient sender, string text);
        void SendEmote(string target, string sender, string text);
        void SendEmote(IClient target, string sender, string text);
        void SendEmote(IClient target, IClient sender, string text);
        void SendEmote(Predicate<IClient> match, string sender, string text);
        void SendEmote(Predicate<IClient> match, IClient sender, string text);

        void SendPrivate(string target, string sender, string text);
        void SendPrivate(IClient target, string sender, string text);
        void SendPrivate(IClient target, IClient sender, string text);
        void SendPrivate(Predicate<IClient> match, string sender, string text);
        void SendPrivate(Predicate<IClient> match, IClient sender, string text);

        void SendAnnounce(string text);
        void SendAnnounce(IClient target, string text);
        void SendAnnounce(Predicate<IClient> target, string text);

        void SendWebsite(string address, string caption);
        void SendWebsite(string target, string address, string caption);
        void SendWebsite(IClient target, string address, string caption);
        void SendWebsite(Predicate<IClient> match, string address, string caption);

        void SendHtml(string html);
        void SendHtml(string target, string html);
        void SendHtml(IClient target, string html);
        void SendHtml(Predicate<IClient> match, string html);
    }
}
