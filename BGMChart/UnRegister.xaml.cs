using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BGMChart
{
    /// <summary>
    /// UnRegister.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class UnRegister : Window
    {
        private DataService dataService;

        public UnRegister(DataService data)
        {
            InitializeComponent();

            dataService = data;
        }

        private void IDBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Username.Text == "Username")
            {
                Username.Text = "";
            }
        }

        private void IDBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Username.Text))
            {
                Username.Text = "Username";
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (Password.Password == "Password")
            {
                Password.Password = "";
            }

        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Password.Password))
            {
                Password.Password = "Password";
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Unregister_btn_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text;
            string password = Password.Password.ToString();

            var deleteUser = new User { Username = username, Password = password };
            Boolean test = dataService.UnregisterUser(deleteUser);

            if (test) { this.Close(); }

        }
    }
}
