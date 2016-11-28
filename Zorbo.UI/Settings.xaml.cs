using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Forms.Integration;

using Zorbo;
using Zorbo.Interface;
using Zorbo.Plugins;

namespace Zorbo.UI
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public Help Help {
            get { return (Help)base.GetValue(HelpProperty); }
        }

        public Settings(AresServer server) {

            InitializeComponent();
            DataContext = server;

            Loaded += Options_Loaded;
            Unloaded += Options_Unloaded;
        }

        private void Options_Loaded(object sender, RoutedEventArgs e) {

            AresServer server = (AresServer)DataContext;

            foreach (var plugin in ((PluginHost)server.PluginHost).Plugins) {

                TreeViewItem newitem = new TreeViewItem();

                newitem.Header = plugin.Name;
                newitem.Tag = plugin;

                FrameworkElement root = null;

                if (plugin.Plugin is IWPFPlugin) {
                    var iface = (IWPFPlugin)plugin.Plugin;
                    if (iface.GUI == null) continue;

                    root = iface.GUI;
                }
                else if (plugin.Plugin is IWinFormsPlugin) {
                    var iface = (IWinFormsPlugin)plugin.Plugin;
                    if (iface.GUI == null) continue;

                    root = new WindowsFormsHost() { Child = iface.GUI };
                }
                else { continue; }

                root.SetBinding(VisibilityProperty, new Binding("IsSelected") {
                    Source = newitem,
                    Converter = (BooleanToVisibilityConverter)TryFindResource("BooleanToVisibility")
                });

                Grid.SetColumn(root, 1);

                optPlugins.Items.Add(newitem);
                LayoutRoot.Children.Add(root);
            }
        }

        private void Options_Unloaded(object sender, RoutedEventArgs e) {
            AresServer server = (AresServer)DataContext;

            foreach (var plugin in ((PluginHost)server.PluginHost).Plugins) {

                if (plugin.Plugin is IWPFPlugin) {
                    var iface = (IWPFPlugin)plugin.Plugin;
                    if (iface.GUI == null) continue;

                    LayoutRoot.Children.Remove(iface.GUI);
                }
                else if (plugin.Plugin is IWinFormsPlugin) {
                    var iface = (IWinFormsPlugin)plugin.Plugin;
                    if (iface.GUI == null) continue;

                    foreach (var child in LayoutRoot.Children) {
                        if (child is WindowsFormsHost) {
                            var host = (WindowsFormsHost)child;
                            if (host.Child == iface.GUI) {
                                LayoutRoot.Children.Remove(host);
                                host.Child.Dispose();
                                host.Child = null;
                                host.Dispose();
                            }
                        }
                    }
                }
            }
        }


        public static readonly DependencyProperty HelpProperty =
            DependencyProperty.Register(
            "Help",
            typeof(Help),
            typeof(Settings),
            new PropertyMetadata(new Help()));
    }
}
