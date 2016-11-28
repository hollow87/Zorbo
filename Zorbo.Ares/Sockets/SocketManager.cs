using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

using Zorbo.Data;

namespace Zorbo.Sockets
{
    public sealed class SocketManager : IOManager<IOTask>
    {
        Int32 maxOutPackets = 0;

        EventHandler acceptComplete = null;
        EventHandler connectComplete = null;
        EventHandler disconnectComplete = null;
        
        Stack<IOBuffer> connPool = null;
        Queue<SocketConnectTask> connQueue = null;

        Stack<IOBuffer> discPool = null;
        Queue<SocketDisconnectTask> discQueue = null;

        Stack<IOBuffer> acceptPool = null;
        Queue<SocketAcceptTask> acceptQueue = null;

        public const int BufferSize = 8 * 1024;

        public SocketManager(int maxOutgoingPackets, int stackSize = 60)
            : base(stackSize, BufferSize) {

            maxOutPackets = maxOutgoingPackets;

            connPool = new Stack<IOBuffer>();
            discPool = new Stack<IOBuffer>();
            acceptPool = new Stack<IOBuffer>();

            connQueue = new Queue<SocketConnectTask>();
            discQueue = new Queue<SocketDisconnectTask>();
            acceptQueue = new Queue<SocketAcceptTask>();

            acceptComplete = new EventHandler(ExecuteAcceptComplete);
            connectComplete = new EventHandler(ExecuteConnectComplete);
            disconnectComplete = new EventHandler(ExecuteDisconnectComplete);
        }

        public void QueueAccept(SocketAcceptTask task) {
            lock (acceptQueue) acceptQueue.Enqueue(task);
        }

        public void QueueConnect(SocketConnectTask task) {
            lock (connQueue) connQueue.Enqueue(task);
        }

        public override void QueueWrite(IOTask task) {
            if (writeQueue.Count < maxOutPackets)
                base.QueueWrite(task);
        }
        
        public void QueueDisconnect(SocketDisconnectTask task) {
            lock (discQueue) discQueue.Enqueue(task);
        }


        protected override void Initialize() {
            base.Initialize();

            for (int i = 0; i < 5; i++) {
                var buff = new IOBuffer();
                buff.Completed += acceptComplete;

                acceptPool.Push(buff);
            }

            for (int i = 0; i < 10; i++) {
                var buff = new IOBuffer();
                buff.Completed += connectComplete;

                connPool.Push(buff);
            }

            for (int i = 0; i < 5; i++) {
                var buff = new IOBuffer();
                buff.Completed += disconnectComplete;

                discPool.Push(buff);
            }
        }

        protected override void Dispose() {
            base.Dispose();
            acceptPool.Clear();
            acceptQueue.Clear();
            connPool.Clear();
            connQueue.Clear();
            discPool.Clear();
            discQueue.Clear();
        }

        volatile int exec = 0;
        protected override void WorkerLoop() {
            while (Alive) {
                ExecuteAccept();
                ExecuteConnect();
                ExecuteRead();
                ExecuteWrite();
                ExecuteDisconnect();
                if (++exec >= 10) {
                    exec = 0;
                    Thread.Sleep(3);
                }
            }
        }


        private void ExecuteAccept() {
            IOBuffer args = null;
            SocketAcceptTask task = null;

            lock(acceptPool)
                lock (acceptQueue) {

                    if (acceptPool.Count == 0 || acceptQueue.Count == 0)
                        return;

                    args = acceptPool.Pop();
                    task = acceptQueue.Dequeue();

                    task.Execute(args);
                }
        }

        private void ExecuteAcceptComplete(object sender, EventArgs e) {
            lock (acceptPool) acceptPool.Push((IOBuffer)sender);
        }


        private void ExecuteConnect() {
            IOBuffer args = null;
            SocketConnectTask task = null;

            lock (connPool)
                lock (acceptQueue) {

                    if (connPool.Count == 0 || connQueue.Count == 0)
                        return;

                    args = connPool.Pop();
                    task = connQueue.Dequeue();

                    task.Execute(args);
                }
        }

        private void ExecuteConnectComplete(object sender, EventArgs e) {
            lock (connPool) connPool.Push((IOBuffer)sender);
        }


        private void ExecuteDisconnect() {
            IOBuffer args = null;
            SocketDisconnectTask task = null;

            lock (discPool)
                lock (discQueue) {

                    if (discPool.Count == 0 || discQueue.Count == 0)
                        return;

                    args = discPool.Pop();
                    task = discQueue.Dequeue();

                    task.Execute(args);
                }
        }

        private void ExecuteDisconnectComplete(object sender, EventArgs e) {
            lock (discPool) discPool.Push((IOBuffer)sender);
        }

        public static Socket CreateTcp(AddressFamily family) {

            var socket = new Socket(
                family,
                SocketType.Stream,
                ProtocolType.Tcp);

            socket.Blocking = false;
            socket.UseOnlyOverlappedIO = true;
            socket.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);

            socket.SendBufferSize = SocketManager.BufferSize;
            socket.ReceiveBufferSize = SocketManager.BufferSize;

            return socket;
        }

        public static Socket CreateUdp(AddressFamily family) {

            var socket = new Socket(
                family,
                SocketType.Dgram,
                ProtocolType.Udp);

            socket.Blocking = false;
            socket.UseOnlyOverlappedIO = true;
            socket.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);

            socket.SendBufferSize = SocketManager.BufferSize;
            socket.ReceiveBufferSize = SocketManager.BufferSize;

            return socket;
        }
    }
}
