using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Zorbo.Data
{
    public class IOManager<TTask> 
        where TTask : class, IOTask
    {
        int stackSize = 0;
        
        EventHandler readCompleted = null;
        EventHandler writeCompleted = null;

        volatile bool running = false;

        protected Thread worker = null;
        protected IOBufferManager buffers = null;

        protected Queue<TTask> readQueue = null;
        protected Queue<TTask> writeQueue = null;

        protected Stack<IOBuffer> readPool = null;
        protected Stack<IOBuffer> writePool = null;

        public bool Alive {
            get { return running && (worker != null && worker.IsAlive); }
        }


        public IOManager(int stackSize, int bufferSize) {
            this.stackSize = stackSize;

            readPool = new Stack<IOBuffer>();
            readQueue = new Queue<TTask>();

            writePool = new Stack<IOBuffer>();
            writeQueue = new Queue<TTask>();

            readCompleted = new EventHandler(ExecuteReadComplete);
            writeCompleted = new EventHandler(ExecuteWriteComplete);

            buffers = new IOBufferManager(bufferSize * stackSize * 2, bufferSize);
        }


        public void Start() {
            if (running)
                return;

            running = true;

            worker = new Thread(WorkerLoop);
            worker.IsBackground = true;

            Initialize();

            worker.Start();
            
        }

        public void Stop() {
            running = false;

            worker.Abort();
            worker.Join(200);
            worker = null;

            Dispose();
        }


        public virtual void QueueRead(TTask task) {
            lock(readQueue) readQueue.Enqueue(task);
        }
        
        public virtual void QueueWrite(TTask task) {;
            lock (writeQueue) writeQueue.Enqueue(task);
        }


        protected virtual void Initialize() {

            for (int i = 0; i < stackSize * 2; i++) {

                IOBuffer buff = new IOBuffer();
                var slot = buffers.GetBuffer();

                buff.SetBuffer(slot.Buffer, slot.Offset, 0);

                if (i % 2 == 0) {
                    buff.Completed += readCompleted;
                    readPool.Push(buff);
                }
                else {
                    buff.Completed += writeCompleted;
                    writePool.Push(buff);
                }
            }
        }

        protected virtual void Dispose() {
            readPool.Clear();
            readQueue.Clear();
            writePool.Clear();
            writeQueue.Clear();
        }


        protected virtual void WorkerLoop() {
            int exec = 0;

            while (Alive) {

                ExecuteRead();
                ExecuteWrite();

                if (++exec > 20) {
                    exec = 0;
                    Thread.Sleep(3);
                }
            }
        }

        protected void ExecuteRead() {
            IOBuffer args = null;
            TTask task = null;

            lock (readPool)
                lock (readQueue) {

                    if (readPool.Count == 0 || readQueue.Count == 0) {
                        return;
                    }

                    args = readPool.Pop();
                    task = readQueue.Dequeue();

                    task.Execute(args);
                }
        }

        private void ExecuteReadComplete(object sender, EventArgs e) {
            lock (readPool) readPool.Push((IOBuffer)sender);
        }


        protected void ExecuteWrite() {
            IOBuffer args = null;
            TTask task = null;
            
            lock (writePool)
                lock (writeQueue) {

                    if (writePool.Count == 0 || writeQueue.Count == 0) {
                        return;
                    }

                    args = writePool.Pop();
                    task = writeQueue.Dequeue();

                    task.Execute(args);
                }
        }

        private void ExecuteWriteComplete(object sender, EventArgs e) {
            lock (writePool) writePool.Push((IOBuffer)sender);
        }
    }
}
