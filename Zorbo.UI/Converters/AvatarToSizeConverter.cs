using System;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Data;

using Zorbo.Interface;

namespace Zorbo.UI
{
    public class AvatarToSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is IAvatar)
                return ((IAvatar)value).SmallBytes.Length;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;   
        }
    }
}
