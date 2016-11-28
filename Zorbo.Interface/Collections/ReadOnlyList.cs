using System;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;


namespace System.Collections.Generic
{
    public interface IReadOnlyList<T> : 
        INotifyCollectionChanged,
        INotifyPropertyChanged,
        IEnumerable<T>,
        IEnumerable
    {
        int Count { get; }

        T this[int index] { get; }

        bool Contains(T value);
        int IndexOf(T value);
    }

    public class ReadOnlyList<T> : 
        ReadOnlyObservableCollection<T>, 
        IReadOnlyList<T>,
        IEnumerable<T>, 
        IEnumerable
    {

        protected ObservableCollection<T> Wrapped {
            get;
            private set;
        }

        public ReadOnlyList()
            : this(new ObservableCollection<T>()) { }

        public ReadOnlyList(ObservableCollection<T> towrap)
            : base(towrap) {

            Wrapped = towrap;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return Wrapped.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return Wrapped.GetEnumerator();
        }
    }
}
