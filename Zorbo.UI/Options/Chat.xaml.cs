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

using Microsoft.Win32;

namespace Zorbo.UI
{
    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : UserControl
    {
        public Chat() {
            InitializeComponent();
        }


        private void btnLoad_Click(object sender, RoutedEventArgs e) {

            StringBuilder filter = new StringBuilder();

            filter.Append("Image Files (*.bmp, *.jpg, *.jpeg, *.gif, *.tif)|*.bmp;*.jpg;*.jpeg;*.gif;*.tif|");
            filter.Append("Bitmap Image (*.bmp)|*.bmp|");
            filter.Append("JPEG Image (*.jpg, *.jpeg)|*.jpg;*.jpeg|");
            filter.Append("GIF Image (*.gif)|*.gif|");
            filter.Append("TIFF Image (*.tif)|*.tif|");
            filter.Append("All Files (*.*)|*.*");

            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            ofd.Filter = filter.ToString();

            if ((bool)ofd.ShowDialog()) {
                string file = ofd.FileName;

                AresAvatar avatar = AresAvatar.Load(file);
                ((Config)DataContext).Avatar = avatar;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e) {
            ((Config)DataContext).Avatar = null;
        }

        private void Help_MouseUp(object sender, MouseButtonEventArgs e) {

            Help help = this.FindVisualAnscestor<Settings>().Help;

            help.Control = sender as UIElement;
            help.Text = Help.GetHelpText(help.Control);

            help.IsOpen = true;
        }
    }
}
