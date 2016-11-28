using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Zorbo.Interface;

namespace Zorbo.Plugins
{
    public class LoadedPlugin : NotifyObject, ILoadedPlugin
    {
        string name = string.Empty;

        bool enabled = false;
        IPlugin plugin = null;

        public string Name {
            get { return name; }
            set {
                if (name != value) {
                    name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
        }

        public IPlugin Plugin {
            get { return plugin; }
            set {
                if (plugin != value) {
                    plugin = value;
                    RaisePropertyChanged(() => Plugin);
                }
            }
        }

        public Boolean Enabled {
            get { return enabled; }
            set {
                if (enabled != value) {
                    enabled = value;
                    RaisePropertyChanged(() => Enabled);
                }
            }
        }

        public LoadedPlugin(string name, IPlugin plugin) {
            Name = name;
            Plugin = plugin;
        }
    }
}
