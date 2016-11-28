using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo
{
    public class FloodCounter
    {
        public int Count { get; set; }

        public DateTime Last { get; set; }

        public FloodCounter() { }

        public FloodCounter(int count, DateTime last) {
            Last = last;
            Count = count;
        }
    }
}
