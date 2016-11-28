using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Zorbo.Interface;
using Zorbo.Users;

namespace Zorbo.UI
{
    public class ClientIdToUsernameConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) {

            if (values.Length == 2) {

                var id = values[0] as IClientId;
                var history = values[1] as IHistory;

                if (id == null || history == null)
                    return string.Empty;

                var record = history.Find((s) => s.ClientId.Equals(id));
                if (record != null) return record.Name;
            }

            return string.Empty;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
