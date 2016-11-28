using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Data;

namespace Zorbo.Sockets
{
    public sealed class SocketConnectTask : IOTask<SocketConnectTask>
    {
        AsyncCallback connectComplete;


        public Socket Socket {
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

        public IPEndPoint RemoteEndPoint {
            get;
            internal set;
        }


        public SocketConnectTask(IPEndPoint remoteEp) {
            RemoteEndPoint = remoteEp;
            connectComplete = new AsyncCallback(ExecuteComplete);
        }

        public void Execute(IOBuffer buffer) {
            try {
                Socket.BeginConnect(RemoteEndPoint, connectComplete, buffer);
            }
            catch (Exception ex) {
                Exception = ex;
                OnCompleted(buffer);
            }
        }

        private void ExecuteComplete(IAsyncResult ar) {
            IOBuffer buff = (IOBuffer)ar.AsyncState;

            try {
                Socket.EndConnect(ar);
            }
            catch (Exception ex) {
                Exception = ex;
            }

            OnCompleted(buff);
        }

        private void OnCompleted(IOBuffer buffer) {
            try {
                var x = Completed;
                if (x != null) x(Socket, new IOTaskCompleteEventArgs<SocketConnectTask>(this, buffer));
            }
            catch { }
            buffer.Release();
        }

        public event EventHandler<IOTaskCompleteEventArgs<SocketConnectTask>> Completed;
    }
}
