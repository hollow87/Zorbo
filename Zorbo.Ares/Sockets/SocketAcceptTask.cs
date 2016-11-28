﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Data;

namespace Zorbo.Sockets
{
    public sealed class SocketAcceptTask : IOTask<SocketAcceptTask>
    {
        AsyncCallback acceptComplete;

        public Socket Socket {
            get;
            internal set;
        }

        public Socket AcceptSocket {
            get;
            internal set;
        }

        public Object UserToken {
            get;
            set;
        }

        public Exception Exception {
            get;
            internal set;
        }

        public SocketAcceptTask() {
            acceptComplete = new AsyncCallback(ExecuteComplete);
        }

        public void Execute(IOBuffer buffer) {

            try {
                if (Socket != null)
                    Socket.BeginAccept(acceptComplete, buffer);
            }
            catch (Exception ex) {
                Exception = ex;
                OnCompleted(buffer);
            }
        }

        private void ExecuteComplete(IAsyncResult ar) {
            IOBuffer buff = (IOBuffer)ar.AsyncState;

            try {
                Socket client = Socket.EndAccept(ar);

                if (client == null || !client.Connected)
                    throw new SocketException((int)SocketError.SocketError);

                AcceptSocket = client;
            }
            catch (Exception ex) {
                Exception = ex;
            }

            OnCompleted(buff);
        }

        private void OnCompleted(IOBuffer buffer) {
            try {
                var x = Completed;
                if (x != null) x(Socket, new IOTaskCompleteEventArgs<SocketAcceptTask>(this, buffer));
            }
            catch { }
            buffer.Release();
        }

        public event EventHandler<IOTaskCompleteEventArgs<SocketAcceptTask>> Completed;
    }
}
