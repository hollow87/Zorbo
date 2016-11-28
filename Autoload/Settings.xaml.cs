using System;
using System.IO;
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
using System.Collections.ObjectModel;

using Microsoft.Win32;

namespace Autoload
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings(Autoload plugin) {
            InitializeComponent();
            DataContext = plugin;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {

            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Multiselect = false;
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            ofd.InitialDirectory = ((Autoload)DataContext).Directory;
            ofd.Filter = "Dynamic Link Library (*.dll)|*.dll";

            if ((bool)ofd.ShowDialog()) {
                Autoload plugin = (Autoload)DataContext;
                FileInfo file = new FileInfo(ofd.FileName);

                plugin.Plugins.Add(file.Name);
                plugin.SaveFile();
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e) {

            if (lbPlugins.SelectedIndex > -1) {

                Autoload plugin = (Autoload)DataContext;

                plugin.Plugins.RemoveAt(lbPlugins.SelectedIndex);
                plugin.SaveFile();
            }
        }
    }
}
