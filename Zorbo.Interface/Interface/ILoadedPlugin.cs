using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Zorbo.Interface
{
    public interface ILoadedPlugin : INotifyPropertyChanged
    {
        string Name { get; }
        
        IPlugin Plugin { get; }

        bool Enabled { get; set; }
    }
}
