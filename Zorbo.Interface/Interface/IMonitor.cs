﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public interface IMonitor : INotifyPropertyChanged
    {
        long SpeedIn { get; }
        long SpeedOut { get; }
        long LastBytesIn { get; }
        long LastBytesOut { get; }
        long TotalBytesIn { get; }
        long TotalBytesOut { get; }
    }
}
