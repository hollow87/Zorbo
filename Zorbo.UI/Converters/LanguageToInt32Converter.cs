using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Zorbo.Interface;

namespace Zorbo.UI
{
    //Convert from Language to the SelectedIndex

    public sealed class LanguageToInt32Converter : IValueConverter
    {
        Language[] values;

        public LanguageToInt32Converter() {
            values = Enum.GetValues(typeof(Language)).Cast<Language>().ToArray();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value is Language) {
                return values.FindIndex((s) => s == (Language)value);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            
            if (value is int) {
                return values[(int)value];
            }

            return null;
        }
    }
}
