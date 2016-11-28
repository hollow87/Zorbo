using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using System.ComponentModel;

using Zorbo;
using Zorbo.Interface;
using System.Linq.Expressions;

namespace Zorbo.Plugins
{
    public class PluginHost : NotifyObject, IPluginHost
    {
        IServer server;
        ObservableCollection<LoadedPlugin> plugins;

        public int Count {
            get { return plugins.Count((s) => s.Enabled); }
        }

        public ILoadedPlugin this[int index] {
            get {
                lock (plugins) {
                    if (index >= 0 && index < plugins.Count)
                        return plugins[index];
                }
                throw new ArgumentOutOfRangeException("index");
            }
        }

        public ReadOnlyList<LoadedPlugin> Plugins {
            get;
            private set;
        }

        internal IServer Server {
            get { return server; }
        }


        class ErrorInfo : IErrorInfo 
        {
            public string Name { get; internal set; }

            public string Method { get; internal set; }

            public Exception Exception { get; internal set; }

            public ErrorInfo(string name, string method, Exception ex) {
                Name = name;
                Method = method;
                Exception = ex;
            }
        }


        public PluginHost(IServer server) {
            this.server = server;

            plugins = new ObservableCollection<LoadedPlugin>();
            Plugins = new ReadOnlyList<LoadedPlugin>(plugins);

            plugins.CollectionChanged += Wrapper_CollectionChanged;
            ((INotifyPropertyChanged)plugins).PropertyChanged += Wrapper_PropertyChanged;
        }

        public bool Contains(ILoadedPlugin value) {
            lock (plugins)
                return plugins.Contains((s) => s == value);
        }

        public int IndexOf(ILoadedPlugin value) {
            lock (plugins)
                return plugins.FindIndex((s) => s == value);
        }


        public bool LoadPlugin(string name) {
            return LoadPlugin(name, false);
        }

        internal bool LoadPlugin(string name, bool dont_enable) {
            string lowname = name.ToLower();

            if (lowname.EndsWith(".dll")) {

                name = name.Substring(0, name.Length - 4);
                lowname = lowname.Substring(0, lowname.Length - 4);
            }

            lock (plugins) {
                
                int index = plugins.FindIndex(s => s.Name.ToLower() == lowname);

                if (index > -1) {
                    EnablePlugin(plugins[index]);
                    return true;
                }
            }

            String dir = Path.Combine(Directories.Plugins, name);
            String file = Path.Combine(dir, name + ".dll");

            if (!Directory.Exists(dir) || !File.Exists(file))
                return false;

            try {
                Assembly asm = Assembly.LoadFrom(file);

                foreach (var type in asm.GetExportedTypes()) {
                    if (typeof(IPlugin).IsAssignableFrom(type)) {

                        IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                        LoadedPlugin zorbo_plugin = new LoadedPlugin(name, plugin);

                        lock (plugins) plugins.Add(zorbo_plugin);
                        if (!dont_enable) EnablePlugin(zorbo_plugin);

                        return true;
                    }
                }
            }
            catch (PluginLoadException plex) {
                OnError("LoadPlugin", "LoadPlugin", plex);
            }
            catch (Exception ex) {
                OnError("LoadPlugin", "LoadPlugin", new PluginLoadException("The plugin failed to load (See inner exception for details).", ex));
            }
            return false;
        }


        private void EnablePlugin(LoadedPlugin plugin) {
            plugin.Enabled = true;
            RaisePropertyChanged(() => Count);

            OnPluginLoaded(plugin);
        }

        private void OnPluginLoaded(LoadedPlugin plugin) {
            try {
                var x = Loaded;
                if (x != null) x(this, plugin);
            }
            catch (Exception ex) {
                OnError(plugin, "Loaded::EventHandler", ex);
            }

            try {
                plugin.Plugin.Directory = Path.Combine(Directories.Plugins, plugin.Name);
                plugin.Plugin.OnPluginLoaded(Server);
            }
            catch (Exception ex) {
                OnError(plugin, "OnPluginLoaded", ex);
            }
        }


        public bool ImportPlugin(string path) {

            if (File.Exists(path)) {
                try {
                    FileInfo info = new FileInfo(path);

                    if (info.Extension != ".dll")
                        throw new FileLoadException("Invalid plugin file specified. Must be a DLL file");

                    String plugin_path = Path.Combine(Directories.Plugins, info.Name);

                    if (!Directory.Exists(plugin_path))
                        Directory.CreateDirectory(plugin_path);

                    info.CopyTo(plugin_path, true);
                    return LoadPlugin(info.Name);
                }
                catch (Exception ex) {
                    OnError("ImportPlugin", "ImportPlugin", ex);
                }
            }

            return false;
        }


        public void KillPlugin(string name) {
            LoadedPlugin plugin = null;

            lock (plugins) {
                string lowname = name.ToLower();
                int index = plugins.FindIndex(s => s.Name.ToLower() == lowname);

                if (index == -1) return;

                plugin = plugins[index];

                if (plugin.Enabled) {

                    plugin.Enabled = false;
                    RaisePropertyChanged(() => Count);

                    OnPluginKilled(plugin);
                }
            }
        }

        private void OnPluginKilled(LoadedPlugin plugin) {
            try {
                plugin.Plugin.OnPluginKilled();
            }
            catch (Exception ex) {
                OnError(plugin, "OnPluginKilled", ex);
            }

            try {
                var x = Killed;
                if (x != null) x(this, plugin);
            }
            catch (Exception ex) {
                OnError(plugin, "Killed::EventHandler", ex);
            }
        }


        #region " Plugin Functions "

        public void OnError(String name, String method, Exception ex) {
            lock (plugins) {
                ErrorInfo error = new ErrorInfo(name, method, ex);

                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled)
                            plugin.Plugin.OnError(error);
                    }
                    catch { /* ignore errors here.. stack overflow */ }
                }
            }
        }

        private void OnError(LoadedPlugin plugin, String method, Exception ex) {
            OnError(plugin.Name, method, ex);
        }


        public void OnCaptcha(IClient client, CaptchaEvent @event) {
            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        plugin.Plugin.OnCaptcha(client, @event);
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnCaptcha", ex);
                    }
                }
            }
        }


        public void OnJoinRejected(IClient client, RejectReason reason) {
            ((AresServer)server).Stats.Rejected++;

            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled)
                            plugin.Plugin.OnJoinRejected(client, reason);
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnJoinRejected", ex);
                    }
                }
            }
        }

        public ServerFeatures OnSendFeatures(IClient client, ServerFeatures features) {
            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled)
                            features |= plugin.Plugin.OnSendFeatures(client, features);
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnSendFeatures", ex);
                    }
                }
            }
            return features;
        }

        public void OnSendJoin(IClient client) {
            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled)
                            plugin.Plugin.OnSendJoin(client);
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnSendLogin", ex);
                    }
                }
            }
        }

        public bool OnJoinCheck(IClient client) {
            bool ret = true;

            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled && !plugin.Plugin.OnJoinCheck(client))
                            ret = false;
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnJoinCheck", ex);
                    }
                }
            }

            return ret;
        }

        public void OnJoin(IClient client) {
            ((AresServer)server).Stats.Joined++;
            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled)
                            plugin.Plugin.OnJoin(client);
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnJoin", ex);
                    }
                }
            }
        }

        public void OnPart(IClient client, Object state) {
            ((AresServer)server).Stats.Parted++;
            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled)
                            plugin.Plugin.OnPart(client, state);
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnPart", ex);
                    }
                }
            }
        }


        public bool OnVroomJoinCheck(IClient client, UInt16 vroom) {
            bool ret = true;

            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled && !plugin.Plugin.OnVroomJoinCheck(client, vroom))
                            ret = false;
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnVroomJoinCheck", ex);
                    }
                }
            }

            return ret;
        }

        public void OnVroomJoin(IClient client) {
            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled)
                            plugin.Plugin.OnVroomJoin(client);
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnVroomJoin", ex);
                    }
                }
            }
        }

        public void OnVroomPart(IClient client) {
            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled)
                            plugin.Plugin.OnVroomPart(client);
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnVroomPart", ex);
                    }
                }
            }
        }

        public void OnHelp(IClient client) {
            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled)
                            plugin.Plugin.OnHelp(client);
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnHelp", ex);
                    }
                }
            }
        }

        public void OnLogin(IClient client, IPassword password) {
            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled)
                            plugin.Plugin.OnLogin(client, password);
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnLogin", ex);
                    }
                }
            }
        }

        public bool OnRegister(IClient client, IPassword password) {
            bool ret = true;

            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled && !plugin.Plugin.OnRegister(client, password))
                            ret = false;
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnRegister", ex);
                    }
                }
            }

            return ret;
        }

        public bool OnFileReceived(IClient client, ISharedFile file) {
            bool ret = true;

            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled && !plugin.Plugin.OnFileReceived(client, file))
                            ret = false;
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnFileReceived", ex);
                    }
                }
            }

            return ret;
        }


        public bool OnBeforePacket(IClient client, IPacket packet) {
            bool ret = true;
            IPacket clone = packet.Clone();

            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled && !plugin.Plugin.OnBeforePacket(client, clone))
                            ret = false;

                        packet = clone;
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnBeforePacket", ex);
                    }
                }
            }

            return ret;
        }

        public void OnAfterPacket(IClient client, IPacket packet) {

            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled)
                            plugin.Plugin.OnAfterPacket(client, packet);
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnAfterPacket", ex);
                    }
                }
            }
        }

        public bool OnFlood(IClient client, IPacket packet) {
            bool ret = true;

            lock (plugins) {
                foreach (var plugin in plugins) {
                    try {
                        if (plugin.Enabled && !plugin.Plugin.OnFlood(client, packet))
                            ret = false;
                    }
                    catch (Exception ex) {
                        OnError(plugin, "OnFlood", ex);
                    }
                }
            }

            return ret;
        }

        #endregion


        public IEnumerator<ILoadedPlugin> GetEnumerator() {
            return new SafeEnumerator<ILoadedPlugin>(plugins);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return (IEnumerator)GetEnumerator();
        }


        private void Wrapper_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            RaisePropertyChanged(e);
        }

        private void Wrapper_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

            var x = this.CollectionChanged;

            if (x != null) {
                
                switch (e.Action) {
                    case NotifyCollectionChangedAction.Add: {
                            LoadedPlugin[] newitems = new LoadedPlugin[e.NewItems.Count];

                            e.NewItems.CopyTo(newitems, 0);

                            x(this, new NotifyCollectionChangedEventArgs(
                                e.Action,
                                newitems.Select((s) => (IPlugin)s.Plugin).ToList(),
                                e.NewStartingIndex));
                        }
                        break;
                    case NotifyCollectionChangedAction.Remove: {
                            LoadedPlugin[] olditems = new LoadedPlugin[e.OldItems.Count];

                            e.OldItems.CopyTo(olditems, 0);

                            x(this, new NotifyCollectionChangedEventArgs(
                                e.Action,
                                olditems.Select((s) => (IPlugin)s.Plugin).ToList(),
                                e.OldStartingIndex));
                        }
                        break;
                    case NotifyCollectionChangedAction.Move: {
                            LoadedPlugin[] olditems = new LoadedPlugin[e.OldItems.Count];

                            e.OldItems.CopyTo(olditems, 0);

                            x(this, new NotifyCollectionChangedEventArgs(
                                e.Action,
                                olditems.Select((s) => (IPlugin)s.Plugin).ToList(),
                                e.NewStartingIndex,
                                e.OldStartingIndex));
                        }
                        break;
                    case NotifyCollectionChangedAction.Replace: {
                            LoadedPlugin[] newitems = new LoadedPlugin[e.NewItems.Count];
                            LoadedPlugin[] olditems = new LoadedPlugin[e.OldItems.Count];

                            e.NewItems.CopyTo(newitems, 0);
                            e.OldItems.CopyTo(olditems, 0);

                            x(this, new NotifyCollectionChangedEventArgs(
                                e.Action,
                                newitems.Select((s) => (IPlugin)s.Plugin).ToList(),
                                olditems.Select((s) => (IPlugin)s.Plugin).ToList()));
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        x(this, new NotifyCollectionChangedEventArgs(e.Action));
                        break;
                }
            }
        }

        public event PluginEventHandler Loaded;
        public event PluginEventHandler Killed;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
