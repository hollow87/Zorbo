using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Zorbo.Data;

namespace Zorbo.Interface
{
    public interface ISocket : IDisposable
    {
        /// <summary>
        /// True if the socket has an established connection, otherwise false
        /// </summary>
        Boolean Connected { get; }

        /// <summary>
        /// Gets a value that represents an object used to track the IO of the socket
        /// </summary>
        IOMonitor Monitor { get; }

        /// <summary>
        /// Gets a value that represents the message formatter currently used by the socket
        /// </summary>
        IFormatter Formatter { get; set; }

        /// <summary>
        /// Gets the local endpoint currently associated with the socket
        /// </summary>
        IPEndPoint LocalEndPoint { get; }

        /// <summary>
        /// Gets the remote endpoint currently associated with the socket
        /// </summary>
        IPEndPoint RemoteEndPoint { get; }

        
        void Connect(IPEndPoint ep, Action<ISocket> complete);

        void Disconnect();
        void Disconnect(object usertoken);

        void Bind(IPEndPoint ep);

        void Listen();
        void Listen(int backlog);

        void SendAsync(IPacket packet);
        void SendAsync(IPacket packet, IPEndPoint remoteEp);

        void ReceiveAsync();

        event EventHandler<AcceptEventArgs> Accepted;
        event EventHandler<ExceptionEventArgs> Exception;
        event EventHandler<PacketEventArgs> PacketSent;
        event EventHandler<PacketEventArgs> PacketReceived;
        event EventHandler<DisconnectEventArgs> Disconnected;
    }
}
