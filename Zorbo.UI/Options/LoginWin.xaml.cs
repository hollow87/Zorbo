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
using System.Windows.Shapes;

using Zorbo.Interface;
using Zorbo.Users;

namespace Zorbo.UI
{
    /// <summary>
    /// Interaction logic for LoginWin.xaml
    /// </summary>
    public partial class LoginWin : Window
    {
        CollectionView view;

        public LoginWin(History history) {

            DataContext = history;
            InitializeComponent();

            view = (CollectionView)CollectionViewSource.GetDefaultView(listBox1.ItemsSource);
            view.Filter = (obj) => 
                ((IRecord)obj).Name.Contains(txtSearch.Text) ||
                ((IRecord)obj).ClientId.ExternalIp.ToString().Contains(txtSearch.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            view.Refresh();
        }

        private void button1_Click(object sender, RoutedEventArgs e) {

            if (listBox1.SelectedIndex > -1) {

                var record = (IRecord)listBox1.SelectedItem;
                ((History)DataContext).Admin.Passwords.Add(new Password(record, (AdminLevel)(cbLevel.SelectedIndex + 1), passBox.SecurePassword));
            }

            this.Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
