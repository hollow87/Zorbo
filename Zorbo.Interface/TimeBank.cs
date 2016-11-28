using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Zorbo
{
    /// <summary>
    /// Used as an alternative to DateTime.Now for accurate time tracking and speed
    /// </summary>
    public static class TimeBank
    {
        static DateTime startTime;
        static Stopwatch timeTracker;


        static TimeBank() {
            timeTracker = new Stopwatch();

            startTime = DateTime.Now;
            timeTracker.Start();
        }

        /// <summary>
        /// Gets the current time represented as a DateTime object
        /// </summary>
        public static DateTime CurrentTime {
            get { return startTime.Add(timeTracker.Elapsed); }
        }

        /// <summary>
        /// Gets the DateTime value at the time the application was started
        /// </summary>
        public static DateTime ApplicationStartTime {
            get { return startTime; }
        }
    }
}
