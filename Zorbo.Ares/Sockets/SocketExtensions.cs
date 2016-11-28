﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Sockets
{
    public static class SocketExtensions
    {
        static SocketManager IO;

        static SocketExtensions() {
            IO = new SocketManager(10000000);
            IO.Start();
        }


        public static void QueueAccept(this Socket socket, SocketAcceptTask task) {

            if (socket == null)
                throw new ArgumentNullException("socket", "socket cannot be null");

            task.Socket = socket;
            task.Exception = null;
            IO.QueueAccept(task);
        }

        public static void QueueConnect(this Socket socket, SocketConnectTask task) {

            if (socket == null)
                throw new ArgumentNullException("socket", "socket cannot be null");

            task.Socket = socket;
            task.Exception = null;
            IO.QueueConnect(task);
        }

        public static void QueueDisconnect(this Socket socket, SocketDisconnectTask task) {

            if (socket == null)
                throw new ArgumentNullException("socket", "socket cannot be null");

            task.Socket = socket;
            task.Exception = null;
            IO.QueueDisconnect(task);
        }

        public static void QueueSend(this Socket socket, byte[] buffer) {
            QueueSend(socket, buffer, 0, buffer.Length);
        }

        public static void QueueSend(this Socket socket, byte[] buffer, int offset, int count) {
            QueueSend(socket, new SocketSendTask(buffer, offset, count));
        }

        public static void QueueSend(this Socket socket, SocketSendTask task) {

            if (socket == null)
                throw new ArgumentNullException("socket", "socket cannot be null");

            task.Socket = socket;
            task.Exception = null;
            IO.QueueWrite(task);
        }

        public static void QueueReceive(this Socket socket, SocketReceiveTask task) {

            if (socket == null)
                throw new ArgumentNullException("socket", "socket cannot be null");

            task.Socket = socket;
            task.Exception = null;
            IO.QueueRead(task);
        }


        public static void Destroy(this Socket socket) {
            if (socket != null) {
                try {
                    socket.Close(100);
#if !MONO
                    socket.Dispose();
#endif
                }
                catch { }
            }
        }
    }
}
