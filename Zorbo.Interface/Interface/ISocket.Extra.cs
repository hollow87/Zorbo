using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public enum SocketProtocol 
    {
        TCP,
        UDP,
    }

    public enum SocketFamily 
    {
        IPv4,
        IPv6,
    }

    public enum SocketState 
    {
        Fault,
        Listening,
        Connecting,
        Connected,
        Disconnecting,
        Disconnected,
        Disposed,
    }


    public class AcceptEventArgs : EventArgs
    {
        public ISocket Socket { get; set; }

        public AcceptEventArgs(ISocket socket) {
            Socket = socket;
        }
    }

    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception { get; set; }

        public IPEndPoint RemoteEndPoint { get; set; }

        public ExceptionEventArgs() { }

        public ExceptionEventArgs(Exception ex, IPAddress address) {
            Exception = ex;
            RemoteEndPoint = new IPEndPoint(address, 0);
        }

        public ExceptionEventArgs(Exception ex, IPEndPoint remoteEp) {
            Exception = ex;
            RemoteEndPoint = remoteEp;
        }
    }
    
    public class PacketEventArgs : EventArgs
    {
        public IPacket Packet { get; private set; }

        public int Transferred { get; private set; }

        public IPEndPoint RemoteEndPoint { get; set; }

        
        public PacketEventArgs(IPacket packet, int transferred) {
            Packet = packet;
            Transferred = transferred;
        }

        public PacketEventArgs(IPacket packet, int transferred, IPEndPoint ep) {
            Packet = packet;
            Transferred = transferred;
            RemoteEndPoint = ep;
        }
    }

    public class DisconnectEventArgs : EventArgs
    {
        public Object UserToken { get; set; }

        public DisconnectEventArgs() { }

        public DisconnectEventArgs(object userToken) {
            UserToken = userToken;
        }
    }
}
