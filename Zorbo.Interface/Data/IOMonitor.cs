﻿using System;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;

using Zorbo.Interface;

namespace Zorbo.Data
{
    public class IOMonitor : NotifyObject, IMonitor
    {
        DateTime update;

        long speedIn = 0;
        long speedOut = 0;

        long lastIn = 0;
        long lastOut = 0;

        long totalIn = 0;
        long totalOut = 0;

        long currentIn = 0;
        long currentOut = 0;

        volatile bool running;


        public long SpeedIn {
            get { return speedIn; }
        }

        public long SpeedOut {
            get { return speedOut; }
        }

        public long LastBytesIn {
            get { return lastIn; }
        }

        public long LastBytesOut {
            get { return lastOut; }
        }

        public long TotalBytesIn {
            get { return totalIn; }
        }

        public long TotalBytesOut {
            get { return totalOut; }
        }

        public bool Running { get { return running; } }


        static Timer timer;
        static List<IOMonitor> monitors;

        static IOMonitor() {
            monitors = new List<IOMonitor>();

            timer = new Timer((state) => {

                for (int i = 0; i < monitors.Count; i++)
                    monitors[i].UpdateSpeed();
            },
            null,
            500,
            500);
        }


        public IOMonitor() {
            update = TimeBank.CurrentTime;
        }


        public void Start() {
            monitors.Add(this);
            running = true;
        }

        public virtual void Reset() {
            if (running)
                Stop();

            speedIn = 0;
            speedOut = 0;
            lastIn = 0;
            lastOut = 0;
            totalIn = 0;
            totalOut = 0;
            currentIn = 0;
            currentOut = 0;
        }

        public void Stop() {
            monitors.Remove(this);
            running = false;
        }


        public void AddInput(long numbytes) {
            if (running) {
                Interlocked.Add(ref totalIn, numbytes);
                Interlocked.Add(ref currentIn, numbytes);
                Interlocked.Exchange(ref lastIn, numbytes);

                RaisePropertyChanged(() => LastBytesIn);
                RaisePropertyChanged(() => TotalBytesIn);
            }
        }

        public void AddOutput(long numbytes) {
            if (running) {
                Interlocked.Add(ref totalOut, numbytes);
                Interlocked.Add(ref currentOut, numbytes);
                Interlocked.Exchange(ref lastOut, numbytes);

                RaisePropertyChanged(() => LastBytesOut);
                RaisePropertyChanged(() => TotalBytesOut);
            }
        }


        protected void UpdateSpeed() {
            DateTime now = TimeBank.CurrentTime;

            if (now.Subtract(update).TotalSeconds >= 1) {
                update = now;

                Interlocked.Exchange(ref speedIn, currentIn);
                Interlocked.Exchange(ref speedOut, currentOut);

                currentIn = 0;
                currentOut = 0;

                RaisePropertyChanged(() => SpeedIn);
                RaisePropertyChanged(() => SpeedOut);
            }
        }
    }
}
