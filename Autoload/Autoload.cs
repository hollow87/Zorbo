using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;

using Zorbo;
using Zorbo.Interface;

namespace Autoload
{
    public class Autoload : IWPFPlugin
    {
        string mydir = "";

        IServer server = null;
        Settings settings = null;


        public Control GUI {
            get { return settings; }
        }

        public string Directory {
            get { return mydir; }
            set { mydir = value; }
        }

        public ObservableCollection<String> Plugins {
            get;
            private set;
        }

        private void LoadFile() {

            if (Thread.CurrentThread != settings.Dispatcher.Thread)
                settings.Dispatcher.BeginInvoke(new Action(() => LoadFile2()));

            else LoadFile2();
        }

        private void LoadFile2() {

            Plugins.Clear();

            Stream stream = null;
            StreamReader reader = null;

            try {
                string name = Path.Combine(Directory, "Autoload.txt");

                if (!File.Exists(name))
                    stream = File.Create(name);
                else
                    stream = File.Open(name, FileMode.Open, FileAccess.Read);

                reader = new StreamReader(stream);

                while (!reader.EndOfStream) {
                    string line = reader.ReadLine();

                    if (!String.IsNullOrEmpty(line))
                        Plugins.Add(line);
                }
            }
            catch { }
            finally {
                if (reader != null) {
                    reader.Close();
                    reader.Dispose();
                }

                if (stream != null)
                    stream.Dispose();
            }

            Plugins.ForEach((s) => server.PluginHost.LoadPlugin(s));
        }

        internal void SaveFile() {

            Stream stream = null;
            StreamWriter writer = null;

            try {
                stream = File.Open(Path.Combine(Directory, "Autoload.txt"), FileMode.Create, FileAccess.Write);
                writer = new StreamWriter(stream);

                Plugins.ForEach((s) => writer.WriteLine(s));

                writer.Flush();
            }
            finally {
                if (writer != null) {
                    writer.Close();
                    writer.Dispose();
                }

                if (stream != null)
                    stream.Dispose();
            }
        }


        public Autoload() {
            this.settings = new Settings(this);
            this.Plugins = new ObservableCollection<string>();
        }

        public void OnPluginLoaded(IServer server) {
            this.server = server;

            LoadFile();
            this.server.PluginHost.KillPlugin("Autoload");
        }

        public void OnPluginKilled() {
        }

        public void OnCaptcha(IClient client, CaptchaEvent @event) {
            
        }

        public void OnJoinRejected(IClient client, RejectReason reason) {
            
        }

        public ServerFeatures OnSendFeatures(IClient client, ServerFeatures features) {
            return features;
        }

        public void OnSendJoin(IClient client) {
            
        }

        public bool OnJoinCheck(IClient client) {
            return true;
        }

        public void OnJoin(IClient client) {
            
        }

        public void OnPart(IClient client, object state) {
            
        }

        public bool OnVroomJoinCheck(IClient client, ushort vroom) {
            return true;
        }

        public void OnVroomJoin(IClient client) {
            
        }

        public void OnVroomPart(IClient client) {
            
        }

        public void OnHelp(IClient client) {

        }

        public void OnLogin(IClient client, IPassword password) {
        }

        public bool OnRegister(IClient client, IPassword password) {
            return true;
        }

        public bool OnFileReceived(IClient client, ISharedFile file) {
            return true;
        }

        public bool OnBeforePacket(IClient client, IPacket packet) {
            return true;
        }

        public void OnAfterPacket(IClient client, IPacket packet) {
            
        }

        public bool OnFlood(IClient client, IPacket packet) {
            return true;
        }

        public void OnError(IErrorInfo error) {
            
        }
    }
}
