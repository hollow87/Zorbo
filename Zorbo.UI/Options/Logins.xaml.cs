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
using Zorbo.Interface;
using Zorbo.Users;

namespace Zorbo.UI
{
    /// <summary>
    /// Interaction logic for Logins.xaml
    /// </summary>
    public partial class Logins : UserControl
    {
        public Logins() {
            InitializeComponent();
        }

        private void btnNew_Click(object sender, RoutedEventArgs e) {
            LoginWin win = new LoginWin((History)DataContext);

            win.Owner = this.FindVisualAnscestor<Window>();
            win.Show();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) {

            if (lstCurrentAdmin.SelectedIndex > -1)
                ((History)DataContext).Admin.Passwords.RemoveAt(lstCurrentAdmin.SelectedIndex);
        }
    }
}
