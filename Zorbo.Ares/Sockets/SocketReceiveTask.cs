using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Zorbo.Data;
using System.Diagnostics;

namespace Zorbo.Sockets
{
    public sealed class SocketReceiveTask : IOTask<SocketReceiveTask>
    {
        int count = 0;

        EndPoint remoteEp;
        MemoryStream stream;

        AsyncCallback receiveCallback;


        public Int32 Count {
            get { return count; }
            internal set { count = value; }
        }

        public Int32 Transferred {
            get;
            internal set;
        }


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
            get { return (IPEndPoint)remoteEp; }
            internal set {
                if (value == null)
                    remoteEp = null;
                else
                    remoteEp = new IPEndPoint(value.Address, value.Port);
            }
        }


        public SocketReceiveTask(int count) {
            Count = count;
            receiveCallback = new AsyncCallback(ExecuteComplete);
        }


        public void Execute(IOBuffer buffer) {

            if (stream == null)
                stream = new MemoryStream();

            ExecuteReceive(buffer);
        }

        private void ExecuteReceive(IOBuffer buffer) {

            try {
                int count = Math.Min(Count - Transferred, SocketManager.BufferSize);

                if (Socket.Poll(0, SelectMode.SelectRead)) {

                    if (Socket.ProtocolType == ProtocolType.Tcp) {
                        Socket.BeginReceive(buffer.Buffer, buffer.Offset, count, SocketFlags.None, receiveCallback, buffer);
                    }
                    else {
                        Socket.BeginReceiveFrom(buffer.Buffer, buffer.Offset, count, SocketFlags.None, ref remoteEp, receiveCallback, buffer);
                    }
                }
                else OnContinue(buffer);
            }
            catch (ObjectDisposedException) {
                OnCompleted(buffer);
            }
            catch (Exception ex) {
                Exception = ex;
                OnCompleted(buffer);
            }
        }

        private void ExecuteComplete(IAsyncResult ar) {
            IOBuffer args = (IOBuffer)ar.AsyncState;

            try {
                Int32 count = 0;
                SocketError err = SocketError.Success;

                if (Socket.ProtocolType == ProtocolType.Tcp)
                    count = Socket.EndReceive(ar, out err);
                else
                    count = Socket.EndReceiveFrom(ar, ref remoteEp);

                if (count == 0)
                    Exception = new SocketException((int)SocketError.NotConnected);

                else if (err != SocketError.Success)
                    Exception = new SocketException((int)err);

                else {
                    Transferred += count;
                    stream.Write(args.Buffer, args.Offset, count);

                    if (Transferred < Count && Socket.ProtocolType == ProtocolType.Tcp) {
                        OnContinue(args);
                        return;
                    }
                }
            }
            catch (Exception ex) {
                Exception = ex;
            }

            OnCompleted(args);
        }

        private void OnContinue(IOBuffer buffer) {
            buffer.Release();
            Socket.QueueReceive(this);
        }

        private void OnCompleted(IOBuffer buffer) {
            try {
                stream.Position = 0;
                stream.Read(buffer.Buffer, buffer.Offset, Transferred);
                stream.Close();
                stream.Dispose();
                stream = null;
            }
            catch { }

            try {
                var x = Completed;
                if (x != null) x(Socket, new IOTaskCompleteEventArgs<SocketReceiveTask>(this, buffer));
            }
            catch { }

            Transferred = 0;
            buffer.Release();
        }

        public event EventHandler<IOTaskCompleteEventArgs<SocketReceiveTask>> Completed;
    }
}
