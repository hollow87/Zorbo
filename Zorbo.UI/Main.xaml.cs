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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Zorbo.UI
{
    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class Main : Window
    {
        Boolean forced;

        AresServer server;
        System.Windows.Forms.NotifyIcon icon;

        public Main() {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this)) {

                server = new AresServer(Config.Load(Directories.AppData, "Config.xml"));
                server.PropertyChanged += new PropertyChangedEventHandler(Server_PropertyChanged);

                DataContext = server;
            }
        }

        void ForceShow() {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        void ForceClose() {
            forced = true;
            this.Close();
        }

        void Server_PropertyChanged(object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "Running") {

                if (server.Running)
                    btnStart.Content = "Stop";

                else {
                    btnStart.Content = "Start";
                    Config.Save((Config)server.Config, System.IO.Path.Combine(Directories.AppData, "Config.xml"));
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            icon = new System.Windows.Forms.NotifyIcon();

            System.Windows.Forms.ContextMenu cm = new System.Windows.Forms.ContextMenu();
            cm.MenuItems.Add("Close", (s, a) => { ForceClose(); });

            icon.ContextMenu = cm;
            icon.Click += (s, args) => {

                if (Visibility == Visibility.Hidden)
                    this.ForceShow();
                
                else this.Hide();
            };
            
            var stream = Application.GetResourceStream(
                new Uri("/Zorbo.UI;component/Zorbo.ico", UriKind.Relative)).Stream;

            if (stream != null)
                icon.Icon = new System.Drawing.Icon(stream);

            icon.Visible = true;

            server.PluginHost.LoadPlugin("Autoload");

            if (((Config)server.Config).AutoStartServer)
                server.Start();
        }

        private void Window_Closing(object sender, CancelEventArgs e) {

            if (!forced) {
                e.Cancel = true;
                this.Hide();
            }
            else {
                if (server.Running)
                    server.Stop();

                Config.Save((Config)server.Config, System.IO.Path.Combine(Directories.AppData, "Config.xml"));

                icon.Visible = false;
                icon.Dispose();
            }
        }


        private void btnStart_Click(object sender, RoutedEventArgs e) {

            if (btnStart.Content.ToString() == "Start") {
                if (!server.Running)
                    server.Start();
            }
            else {
                if (server.Running)
                    server.Stop();
            }
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e) {
            var settings = new Settings(server);

            settings.Closed += Settings_Closed;
            settings.Owner = this;
            settings.Show();
        }

        private void Settings_Closed(object sender, EventArgs e) {
            var settings = (Settings)sender;
            settings.Closed -= Settings_Closed;

            Config.Save((Config)server.Config, System.IO.Path.Combine(Directories.AppData, "Config.xml"));
        }
    }
}
