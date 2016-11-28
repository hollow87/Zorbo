using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Zorbo.Interface;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Zorbo.Users
{
    public class Admin : IReadOnlyList<IClient>, IAdmins
    {
        IServer server;
        List<IClient> admins;

        Passwords passwords;


        public IPasswords Passwords { get { return passwords; } }


        public Admin(IServer server) {

            this.server = server;
            this.server.Users.CollectionChanged += Users_CollectionChanged;

            this.admins = new List<IClient>();
            this.admins.AddRange(server.Users.FindAll((s) => s.Admin > 0));

            this.passwords = new Passwords(server);
            this.passwords.Load(Directories.Cache);
        }


        public void Load() {
            this.passwords.Load(Directories.Cache);
        }

        public void Save() {
            this.passwords.Save(Directories.Cache);
        }


        public int Count {
            get { return admins.Count; }
        }


        public IClient this[int index] {
            get { return admins[index]; }
        }


        public bool Contains(IClient value) {
            return value.Admin > AdminLevel.User;
        }

        public int IndexOf(IClient value) {
            return admins.IndexOf(value);
        }

        public IAdmins Clone() {
            return new Admin(this.server);
        }

        object ICloneable.Clone() {
            return this.Clone();
        }

        public IEnumerator<IClient> GetEnumerator() {
            return admins.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return admins.GetEnumerator();
        }

        private void AddAdmin(IClient client) {

            admins.Add(client);

            RaisePropertyChanged("Count");
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, client, admins.Count - 1));
        }

        private void RemoveAdmin(IClient client) {

            int index = admins.IndexOf(client);
            if (index < 0) return;

            admins.RemoveAt(index);

            RaisePropertyChanged("Count");
            RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, client, index));
        }

        void Users_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {

            if (e.Action == NotifyCollectionChangedAction.Add) {
                IClient client = (IClient)e.NewItems[0];

                client.PropertyChanged += User_PropertyChanged;
                if (client.Admin > AdminLevel.User) AddAdmin(client);

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove) {
                IClient client = (IClient)e.OldItems[0];

                client.PropertyChanged -= User_PropertyChanged;
                if (client.Admin > AdminLevel.User) RemoveAdmin(client);
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset) {

                admins.ForEach((s) => s.PropertyChanged -= User_PropertyChanged);
                admins.Clear();

                RaisePropertyChanged("Count");
                RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        void User_PropertyChanged(object sender, PropertyChangedEventArgs e) {

            if (e.PropertyName == "Admin") {
                IClient admin = (IClient)sender;

                if (admin.Admin > AdminLevel.User)
                    AddAdmin(admin);

                else RemoveAdmin(admin);
            }
        }

        private void RaisePropertyChanged(string propertyName) {
            var x = PropertyChanged;
            if (x != null) x(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs e) {
            var x = CollectionChanged;
            if (x != null) x(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
