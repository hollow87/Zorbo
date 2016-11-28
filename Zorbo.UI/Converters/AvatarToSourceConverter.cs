using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Data;
using System.Windows.Media.Imaging;

using Zorbo.Interface;

namespace Zorbo.UI
{
    public class AvatarToSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var avatar = value as IAvatar;
            if (avatar != null) {

                if (avatar.LargeBytes.Length > 0) {

                    BitmapImage img = new BitmapImage();
                    MemoryStream stream = new MemoryStream(avatar.LargeBytes);

                    img.BeginInit();
                    img.StreamSource = stream;
                    img.EndInit();
                    img.Freeze();

                    return img;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
