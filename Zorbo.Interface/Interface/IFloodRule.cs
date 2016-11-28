using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public interface IFloodRule
    {
        /// <summary>
        /// The packet id to monitor for flooding
        /// </summary>
        byte Id { get; }
        
        /// <summary>
        /// The number of packets to accept before considering it a flood
        /// </summary>
        int Count { get; set; }

        /// <summary>
        /// Gets or sets the amount of time, in milliseconds, after the last packet before the counter is reset
        /// </summary>
        int ResetTimeout { get; set; }

    }
}
