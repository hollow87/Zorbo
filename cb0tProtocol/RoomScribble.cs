﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Zorbo;
using Zorbo.Interface;

namespace cb0tProtocol
{
    class RoomScribble {
        bool received;

        int index;
        ushort chunkCount;

        List<byte[]> buffers;

        public uint Size { get; set; }

        public ushort Chunks { get; set; }

        public ushort ChunkCount {
            get { return chunkCount; }
        }

        public int Index {
            get { return index; }
            set { index = value; }
        }

        public long Remaining {
            get { return ChunkCount - Index; }
        }

        public long Received {
            get { return buffers.Sum((s) => s.Length); }
        }

        public bool IsComplete {
            get {
                return received && 
                    (this.chunkCount == Chunks) &&
                    (this.Received == Size);
            }
        }

        public RoomScribble() {
            this.buffers = new List<byte[]>();
        }

        public void Write(byte[] chunk) {
            if (chunk == null)
                throw new ArgumentNullException("chunk");

            if (IsComplete)
                throw new Exception("something wierd happened");

            this.received = true;
            this.chunkCount++;
            this.buffers.Add(chunk);
        }

        public void Write(byte[] chunk, int index, int count) {
            if (chunk == null)
                throw new ArgumentNullException("chunk");

            if (IsComplete)
                throw new Exception("something wierd happened");

            this.received = true;
            this.chunkCount++;
            this.buffers.Add(chunk.Skip(index).Take(count).ToArray());
        }

        public byte[] Read() {
            if (!IsComplete)
                return null;

            return buffers[index++];
        }

        class LoadRequest
        {
            public object State { get; set; }
            
            public WebRequest Request { get; set; }

            public Action<object> Callback { get; set; }

            public LoadRequest() { }
            public LoadRequest(WebRequest request, Action<object> cb, object state) {
                State = state;
                Request = request;
                Callback = cb;
            }
        }

        internal bool Download(Uri uri, Action<object> callback, object state = null) {
            try {
                var request = WebRequest.Create(uri);

                request.Method = "GET";
                request.BeginGetResponse(LoadCallback, new LoadRequest(request, callback, state));

                return true;
            }
            catch { return false; }
        }

        private void LoadCallback(IAsyncResult ar) {
            var lr = (LoadRequest)ar.AsyncState;

            Bitmap bmp1 = null;
            Bitmap bmp2 = null;
            WebResponse response = null;

            try {
                response = lr.Request.EndGetResponse(ar);

                using (var stream = response.GetResponseStream()) {

                    bmp1 = (Bitmap)Bitmap.FromStream(stream);
                    bmp2 = ScaleImage(bmp1, 384, 384);

                    using (var stream2 = new MemoryStream()) {

                        bmp2.Save(stream2, ImageFormat.Jpeg);
                        byte[] tmp = stream2.ToArray();

                        stream2.SetLength(0);
                        Zlib.Compress(stream2, tmp);

                        tmp = new byte[4000];

                        Reset();
                        stream2.Position = 0;

                        while (true) {
                            int count = stream2.Read(tmp, 0, tmp.Length);
                            if (count == 0) break;

                            Write(tmp, 0, count);
                        }

                        Size = (uint)Received;
                        Chunks = ChunkCount;
                    }
                }

                lr.Callback(lr.State);
            }
            catch (Exception ex) {
                lr.Callback(ex);
            }
            finally {
                if (bmp1 != null)
                    bmp1.Dispose();

                if (bmp2 != null)
                    bmp2.Dispose();
            }
        }

        public void Reset() {
            Size = 0;
            Chunks = 0;
            index = 0;
            this.chunkCount = 0;
            this.received = false;
            this.buffers.Clear();
        }

        public static RoomScribble GetScribble(IClient client) {
            object tmp = null;

            if (client.Extended.TryGetValue("Scribble", out tmp))
                return (RoomScribble)tmp;

            var scribble = new RoomScribble();
            client.Extended["Scribble"] = scribble;

            return scribble;
        }

        private static Bitmap ScaleImage(Bitmap bitmap, int maxwidth, int maxheight) {

            int biggest = Math.Max(maxheight, maxwidth);

            if (bitmap.Width == bitmap.Height)
                return new Bitmap(bitmap, new Size(biggest, biggest));

            double p = 0.0;

            if (bitmap.Width > bitmap.Height)
                p = (double)maxwidth / (double)bitmap.Width;
            else
                p = (double)maxheight / (double)bitmap.Height;

            return new Bitmap(bitmap, new Size((int)(bitmap.Width * p), (int)(bitmap.Height * p)));
        }
    }
}
