using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Data;
using Zorbo.Packets;
using Zorbo.Interface;
using Zorbo.Serialization;

namespace Zorbo.Sockets
{
    public sealed class AresUdpSocket : ISocket, IDisposable
    {
        Socket socket;
        IOMonitor monitor;
        IPEndPoint receiveEp;


        public Socket Socket {
            get { return socket; }
        }

        public bool Connected { get { return false; } }


        public IOMonitor Monitor {
            get { return monitor; }
        }

        public IFormatter Formatter {
            get;
            set;
        }


        public IPEndPoint LocalEndPoint {
            get {
                if (socket != null)
                    return (IPEndPoint)socket.LocalEndPoint;

                return null;
            }
        }

        public IPEndPoint RemoteEndPoint {
            get { return null; }
        }


        public AresUdpSocket(IFormatter formatter) {
            receiveEp = new IPEndPoint(IPAddress.Any, 0);
            Formatter = formatter;

            this.socket = SocketManager.CreateUdp(AddressFamily.InterNetwork);
            this.monitor = new IOMonitor();
        }


        public void Connect(IPEndPoint ep, Action<ISocket> complete) {
            throw new NotImplementedException("Not used for Udp operations");
        }

        public void Disconnect() {
            throw new NotImplementedException("Not used for Udp operations");
        }

        public void Disconnect(object usertoken) {
            throw new NotImplementedException("Not used for Udp operations");
        }


        public void Bind(IPEndPoint ep) {

            if (socket == null) {
                socket = SocketManager.CreateUdp(AddressFamily.InterNetwork);
            }

            monitor.Start();
            socket.Bind(ep);
        }

        public void Listen() {
            throw new NotImplementedException("Not used for Udp operations");
        }

        public void Listen(int backlog) {
            throw new NotImplementedException("Not used for Udp operations");
        }

        public void SendAsync(IPacket packet) {
            throw new NotImplementedException("Not used for Udp operations");
        }

        public void SendAsync(IPacket packet, IPEndPoint remoteEp) {

            byte[] tmp = Formatter.Format(packet);

            SocketSendTask task = new SocketSendTask(tmp);

            task.UserToken = packet;
            task.RemoteEndPoint = remoteEp;
            task.Completed += SendComplete;

            if (socket != null) socket.QueueSend(task);
        }

        private void SendComplete(object sender, IOTaskCompleteEventArgs<SocketSendTask> e) {
            e.Task.Completed -= SendComplete;

            if (e.Task.Exception == null) {
                Monitor.AddOutput(e.Task.Transferred);
                try {
                    var msg = (IPacket)e.Task.UserToken;

                    var complete = PacketSent;
                    if (complete != null) complete(this, new PacketEventArgs(msg, e.Task.Transferred, e.Task.RemoteEndPoint));
                }
                catch (Exception ex) {
                    OnException(ex, e.Task.RemoteEndPoint);
                }
            }
            else OnException(e.Task.Exception, e.Task.RemoteEndPoint);
        }

        public void ReceiveAsync() {

            SocketReceiveTask task = new SocketReceiveTask(8192);

            task.Completed += ReceiveComplete;
            task.RemoteEndPoint = receiveEp;

            if (socket != null) socket.QueueReceive(task);
        }

        private void ReceiveComplete(object sender, IOTaskCompleteEventArgs<SocketReceiveTask> e) {

            if (e.Task.Exception == null) {

                if (e.Task.Transferred > 0) {
                    Monitor.AddInput(e.Task.Transferred);

                    byte id = e.Buffer.Buffer[e.Buffer.Offset];
                    OnPacketReceived(id, e.Buffer.Buffer, e.Buffer.Offset + 1, e.Task.Transferred - 1, e.Task.RemoteEndPoint);

                    e.Task.RemoteEndPoint = receiveEp;
                    if (socket != null) socket.QueueReceive(e.Task);
                }
            }
            else OnException(e.Task.Exception, e.Task.RemoteEndPoint);
        }


        private void OnException(Exception ex, IPEndPoint remoteEp) {

            var x = this.Exception;
            if (x != null) x(this, new ExceptionEventArgs(ex, remoteEp));
        }

        private void OnPacketReceived(byte id, byte[] payload, int offset, int count, IPEndPoint remoteEp) {
            if (Formatter != null) {
                try {
                    IPacket msg = Formatter.Unformat(id, payload, offset, count);

                    var complete = PacketReceived;
                    if (complete != null) complete(this, new PacketEventArgs(msg, count + 1, remoteEp));
                }
                catch (Exception ex) {
                    OnException(ex, remoteEp);
                }
            }
        }


        public void Close() {
            socket.Destroy();
            socket = null;

            Monitor.Reset();

            Exception = null;
            PacketReceived = null;
        }

        public void Dispose() {
            Close();
            Formatter = null;
        }

        
        public event EventHandler<ExceptionEventArgs>  Exception;
        public event EventHandler<PacketEventArgs>     PacketSent;
        public event EventHandler<PacketEventArgs>     PacketReceived;

        //Not used -- hidden
        event EventHandler<AcceptEventArgs> ISocket.Accepted { add { } remove { } }
        event EventHandler<DisconnectEventArgs> ISocket.Disconnected { add { } remove { } }
    }
}
