using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

using Zorbo.Interface;

namespace Zorbo
{
    public sealed class AresClientList : 
        ReadOnlyList<AresClient>, 
        IReadOnlyList<IClient>
    {
        public bool IsReadOnly {
            get { return true; }
        }

        public object SyncRoot {
            get { return ((ICollection)this).SyncRoot; }
        }

        IClient IReadOnlyList<IClient>.this[int index] {
            get { return this[index]; }
        }

        public ObservableCollection<AresClient> List {
            get { return base.Wrapped; }
        }

        public AresClientList() { }

        public AresClientList(ObservableCollection<AresClient> towrap)
            : base(towrap) { }

        public void Sort(Comparison<AresClient> comparison) {
            List.Sort(comparison);
        }

        public void Add(IClient item) {
            List.Add((AresClient)item);
        }

        public bool Remove(IClient item) {
            return List.Remove((AresClient)item);
        }

        public void RemoveAt(int index) {
            List.RemoveAt(index);
        }

        public void Clear() {
            List.Clear();
        }

        public bool Contains(IClient value) {
            if (!(value is AresClient))
                return false;
            return this.Contains((AresClient)value);
        }

        public int IndexOf(IClient value) {
            if (!(value is AresClient))
                return -1;

            return this.IndexOf((AresClient)value);
        }

        public AresClient Find(Predicate<AresClient> predicate) {
            return List.Find(predicate);
        }

        public int FindIndex(Predicate<AresClient> predicate) {
            return List.FindIndex(predicate);
        }

        IEnumerator<IClient> IEnumerable<IClient>.GetEnumerator() {
            return this.GetEnumerator();
        }
    }
}
