using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace BGMChart
{
    public partial class Account : Window
    {
        private List<User> collection;
        private DataService dataService;
        public Account()
        {
            InitializeComponent();
            dataService = new DataService("Users");
        }

        private void IDBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (UsernameTextBox.Text == "Username")
            {
                UsernameTextBox.Text = "";
            }
        }

        private void IDBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(UsernameTextBox.Text))
            {
                UsernameTextBox.Text = "Username";
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (PasswordTextBox.Password == "Password")
            {
                PasswordTextBox.Password = "";
            }

        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(PasswordTextBox.Password))
            {
                PasswordTextBox.Password = "Password";
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
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
            Application.Current.Shutdown();
        }

        private void Login_btn_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Password.ToString();
            collection = dataService.GetAllFromCollection<User>("users");

            if (string.IsNullOrEmpty(username) || !collection.Any(p => p.Username == username && p.Password == password))
            {
                MessageBox.Show("잘못된 아이디 또는 비밀번호입니다.", "로그인 실패", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("로그인 성공!", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                MainWindow main = new MainWindow(username);
                main.Show();
                this.Hide();
            }


        }

        private void Register_btn_Click(object sender, RoutedEventArgs e)
        {
            Register register = new Register(dataService);
            register.Show();
        }

        private void Unregister_btn_Click(object sender, RoutedEventArgs e)
        {
            //TODO
        }
    }
}
