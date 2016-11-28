using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Zorbo.Interface
{
    public abstract class NotifyObject : INotifyPropertyChanged
    {
        #region " INotifyPropertyChanged "

        protected void RaisePropertyChanged(string propName) {
            OnPropertyChanged(new PropertyChangedEventArgs(propName));
        }

        protected void RaisePropertyChanged(PropertyChangedEventArgs e) {
            OnPropertyChanged(e);
        }

        protected void RaisePropertyChanged<T>(Expression<Func<T>> property) {
            var prop = ((MemberExpression)property.Body).Member as PropertyInfo;

            if (prop == null) return;
            RaisePropertyChanged(prop.Name);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) {
            var x = PropertyChanged;
            if (x != null) x(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
