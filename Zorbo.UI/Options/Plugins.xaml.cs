using System;
using System.IO;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

using Zorbo;
using Zorbo.Plugins;
using Zorbo.Interface;

namespace Zorbo.UI
{
    /// <summary>
    /// Interaction logic for Plugins.xaml
    /// </summary>
    public partial class Plugins : UserControl
    {
        ObservableCollection<AvailablePlugin> files;

        public ObservableCollection<AvailablePlugin> Available {
            get { return files; }
        }

        public class AvailablePlugin : NotifyObject
        {
            public string Name {
                get;
                private set;
            }

            public string State {
                get {
                    if (plugin == null)
                        return "Not loaded";
                    else if (!plugin.Enabled)
                        return "Disabled";
                    else
                        return "Enabled";
                }
            }

            LoadedPlugin plugin = null;
            internal LoadedPlugin Plugin {
                get { return plugin; }
                set {
                    if (plugin != value) {
                        plugin = value;

                        if (plugin != null)
                            plugin.PropertyChanged += Plugin_PropertyChanged;

                        RaisePropertyChanged(() => State);
                    }
                }
            }


            public AvailablePlugin(string name) {
                Name = name;
            }

            private void Plugin_PropertyChanged(object sender, PropertyChangedEventArgs e) {
                if (e.PropertyName == "Enabled") RaisePropertyChanged(() => State);
            }
        }

        public Plugins() {
            InitializeComponent();
            files = new ObservableCollection<AvailablePlugin>();
        }

        void CheckAvailable() {

            PluginHost host = (PluginHost)DataContext;
            foreach (var dir in new DirectoryInfo(Directories.Plugins).GetDirectories()) {

                string file = System.IO.Path.Combine(dir.FullName, dir.Name + ".dll");

                if (File.Exists(file)) {

                    var pname = files.Find((s) => s.Name == dir.Name);

                    if (pname == null) {
                        pname = new AvailablePlugin(dir.Name);
                        files.Add(pname);
                    }

                    pname.Plugin = host.Plugins.Find((s) => s.Name == dir.Name);
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e) {

            if (!DesignerProperties.GetIsInDesignMode(this)) {
                PluginHost host = (PluginHost)DataContext;
                
                ((INotifyCollectionChanged)host.Plugins).CollectionChanged += Plugins_CollectionChanged;
                CheckAvailable();
            }
        }

        void Plugins_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
            CheckAvailable();
        }

        private void btnImport_Click(object sender, RoutedEventArgs e) {

        }

        private void btnLoad_Click(object sender, RoutedEventArgs e) {

            var p = lvAvailable.SelectedItem as AvailablePlugin;
            if (p == null) return;

            PluginHost host = (PluginHost)DataContext;

            if (p.Plugin != null && p.Plugin.Enabled) {
                host.KillPlugin(p.Name);
                btnLoad.Content = "Load";
            }
            else if (host.LoadPlugin(p.Name))
                btnLoad.Content = "Unload";

            CheckAvailable();
        }

        private void lvAvailable_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var p = e.AddedItems[0] as AvailablePlugin;

            if (p == null)
                btnLoad.IsEnabled = false;
            else {
                btnLoad.IsEnabled = true;

                if (p.Plugin != null && p.Plugin.Enabled)
                    btnLoad.Content = "Unload";
                else
                    btnLoad.Content = "Load";
            }
        }
    }
}
