using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Zorbo.Data;

using Zorbo.Packets;
using Zorbo.Interface;
using Zorbo.Serialization;
using Zorbo.Packets.Ares;
using System.Collections.Concurrent;

namespace Zorbo.Sockets
{
    public sealed class AresTcpSocket : ISocket, IDisposable
    {
        Socket socket;
        IOMonitor monitor;

        volatile bool sending;
        volatile bool receiving;
        volatile bool disconnected;

        ConcurrentQueue<IPacket> outQueue;

        public Socket Socket {
            get { return socket; }
        }

        public Boolean Connected {
            get { return (socket != null && socket.Connected && !disconnected); }
        }


        public IPEndPoint LocalEndPoint {
            get {
                if (socket != null)
                    return socket.LocalEndPoint as IPEndPoint;

                return null;
            }
        }

        public IPEndPoint RemoteEndPoint {
            get {
                
                if (socket != null)
                    return socket.RemoteEndPoint as IPEndPoint;

                return null;
            }
        }

        public IOMonitor Monitor {
            get { return monitor; }
        }

        public IFormatter Formatter {
            get;
            set;
        }


        private AresTcpSocket() {
            this.monitor = new IOMonitor();
            this.monitor.Start();
            this.outQueue = new ConcurrentQueue<IPacket>();
        }

        public AresTcpSocket(IFormatter formatter) 
            : this() {

            this.socket = SocketManager.CreateTcp(AddressFamily.InterNetwork);
            Formatter = formatter;
        }

        public AresTcpSocket(IFormatter formatter, Socket socket) 
            : this() {

            this.socket = socket;
            Formatter = formatter;
        }


        public void Bind(IPEndPoint ep) {

            if (socket == null) {
                socket = SocketManager.CreateTcp(AddressFamily.InterNetwork);
                outQueue = new ConcurrentQueue<IPacket>();
                Monitor.Start();
            }
            
            socket.Bind(ep);
        }


        public void Listen() {
            Listen(25);
        }

        public void Listen(int backlog) {

            if (socket == null) {
                socket = SocketManager.CreateTcp(AddressFamily.InterNetwork);
                outQueue = new ConcurrentQueue<IPacket>();
                monitor.Start();
            }

            socket.Listen(backlog);

            SocketAcceptTask task = new SocketAcceptTask();
            task.Completed += AcceptComplete;

            socket.QueueAccept(task);
        }

        private void AcceptComplete(object sender, IOTaskCompleteEventArgs<SocketAcceptTask> e) {
            SocketAcceptTask task = (SocketAcceptTask)e.Task;

            if (task.Exception == null) {
                var x = Accepted;
                if (x != null) x(this, new AcceptEventArgs(new AresTcpSocket(Formatter, task.AcceptSocket)));
            }

            if (socket != null) socket.QueueAccept(task);
        }


        public void Connect(IPEndPoint ep, Action<ISocket> complete) {
            disconnected = false;

            if (socket == null) {
                socket = SocketManager.CreateTcp(AddressFamily.InterNetwork);
                outQueue = new ConcurrentQueue<IPacket>();
                monitor.Start();
            }

            SocketConnectTask task = new SocketConnectTask(ep);

            task.UserToken = complete;
            task.Completed += ConnectCompleted;

            socket.QueueConnect(task);
        }

        private void ConnectCompleted(object sender, IOTaskCompleteEventArgs<SocketConnectTask> args) {

            args.Task.Completed -= ConnectCompleted;
            ((Action<ISocket>)args.Task.UserToken)(this);
        }


        public void Disconnect() {
            Disconnect(null);
        }

        public void Disconnect(Object state) {
            if (!disconnected) {
                disconnected = true;

                monitor.Stop();

                SocketDisconnectTask task = new SocketDisconnectTask();

                task.UserToken = state;
                task.Completed += OnDisconnected;

                socket.QueueDisconnect(task);
            }
        }

        private void OnDisconnected(object sender, IOTaskCompleteEventArgs<SocketDisconnectTask> e) {
            e.Task.Completed -= OnDisconnected;

            var x = Disconnected;
            if (x != null) x(this, new DisconnectEventArgs(e.Task.UserToken));
        }


        public void SendAsync(IPacket packet) {

            if (sending) {
                outQueue.Enqueue(packet);
                return;
            }

            sending = true;
            SendPacket(packet);
        }

        public void SendAsync(IPacket packet, IPEndPoint endpoint) {
            SendAsync(packet);
        }

        private void SendQueue() {
            IPacket packet = null;

            if (outQueue.TryDequeue(out packet)) {
                sending = true;
                SendPacket(packet);
            }
        }

        private void SendPacket(IPacket packet) {
            byte[] tmp = Formatter.Format(packet);

            SocketSendTask task = new SocketSendTask(tmp);

            task.UserToken = packet;
            task.Completed += SendComplete;

            if (Connected) socket.QueueSend(task);
        }

        private void SendComplete(object sender, IOTaskCompleteEventArgs<SocketSendTask> e) {
            sending = false;
            e.Task.Completed -= SendComplete;

            if (e.Task.Exception == null) {
                Monitor.AddOutput(e.Task.Transferred);
                try {
                    var msg = (IPacket)e.Task.UserToken;

                    var complete = PacketSent;
                    if (complete != null) complete(this, new PacketEventArgs(msg, e.Task.Transferred));

                    if (Connected) SendQueue();
                }
                catch (Exception ex) {
                    OnException(ex);
                }
            }
            else OnException(e.Task.Exception);
        }


        public void ReceiveAsync() {
            if (receiving)
                throw new InvalidOperationException("Socket is already receiving");

            receiving = true;

            SocketReceiveTask task = new SocketReceiveTask(2);

            task.UserToken = true;//receiving header
            task.Completed += ReceiveCompleted;

            if (Connected) socket.QueueReceive(task);
        }

        private void ReceiveCompleted(object sender, IOTaskCompleteEventArgs<SocketReceiveTask> e) {

            if (e.Task.Exception == null) {

                if (e.Task.Transferred > 0) {

                    Monitor.AddInput(e.Task.Transferred);

                    if ((bool)e.Task.UserToken) {
                        e.Task.UserToken = false;

                        ushort length = BitConverter.ToUInt16(e.Buffer.Buffer, e.Buffer.Offset);

                        // Ares has a hard message limit of 4k... enforce?

                        if (length > (SocketManager.BufferSize - 1)) {
                            throw new SocketException((int)SocketError.NoBufferSpaceAvailable);
                        }

                        e.Task.Count = length + 1;
                        socket.QueueReceive(e.Task);
                    }
                    else {
                        e.Task.UserToken = true;

                        byte id = e.Buffer.Buffer[e.Buffer.Offset];
                        OnPacketReceived(id, e.Buffer.Buffer, e.Buffer.Offset + 1, e.Task.Transferred - 1);

                        e.Task.Count = 2;
                        if (Connected) socket.QueueReceive(e.Task);
                    }
                }
            }
            else OnException(e.Task.Exception);
        }


        private void OnException(Exception ex) {
            sending = false;
            receiving = false;

            var x = this.Exception;
            if (x != null) x(this, new ExceptionEventArgs(ex, RemoteEndPoint));
        }

        private void OnPacketReceived(byte id, byte[] payload, int offset, int count) {

            try {
                IPacket msg = Formatter.Unformat(id, payload, offset, count);

                var complete = PacketReceived;
                if (complete != null) complete(this, new PacketEventArgs(msg, count + 1));
            }
            catch (Exception ex) {
                OnException(ex);
            }
        }


        public void Close() {
            Monitor.Stop();

            socket.Destroy();
            socket = null;

            sending = false;
            receiving = false;

            outQueue = null;

            Accepted = null;
            Exception = null;
            PacketSent = null;
            PacketReceived = null;
            Disconnected = null;
        }

        public void Dispose() {
            Close();
            socket = null;
            Formatter = null;
        }


        public event EventHandler<AcceptEventArgs>     Accepted;
        public event EventHandler<ExceptionEventArgs>  Exception;
        public event EventHandler<PacketEventArgs>     PacketSent;
        public event EventHandler<PacketEventArgs>     PacketReceived;
        public event EventHandler<DisconnectEventArgs> Disconnected;
    }
}
