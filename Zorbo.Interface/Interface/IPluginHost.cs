using System;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Zorbo.Interface
{
    public interface IPluginHost :
        IReadOnlyList<ILoadedPlugin>,
        INotifyPropertyChanged,
        INotifyCollectionChanged
    {

        bool LoadPlugin(string name);
        bool ImportPlugin(string path);
        void KillPlugin(string name);

        event PluginEventHandler Loaded;
        event PluginEventHandler Killed;
    }

    public delegate void PluginEventHandler(object sender, ILoadedPlugin plugin);
}
