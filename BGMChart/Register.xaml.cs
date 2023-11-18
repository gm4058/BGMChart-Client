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
    /// Register.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Register : Window
    {
        private DataService dataService;
        private bool isUsernameAvailable;
        public Register(DataService data)
        {
            InitializeComponent();
            dataService = data;
            isUsernameAvailable = false;
            Register_btn.IsEnabled = isUsernameAvailable;
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

        private void Register_btn_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text;
            string password = Password.Password.ToString();

            if (isUsernameAvailable)
            {
                var newUser = new User { Username = username, Password = password };
                dataService.RegisterUser(newUser);
                MessageBox.Show("회원가입이 완료되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
        }

        private void dupCheck_btn_Click(object sender, RoutedEventArgs e)
        {
            string username = Username.Text;
            string password = Password.Password.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("아이디와 비밀번호를 입력하세요.", "중복 확인 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                isUsernameAvailable = false;
                Register_btn.IsEnabled = isUsernameAvailable;
                reset();
                return;
            }

            if (username.Contains(" ") || password.Contains(" "))
            {
                MessageBox.Show("아이디와 비밀번호에는 스페이스바를 포함할 수 없습니다.", "중복 확인 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                isUsernameAvailable = false;
                Register_btn.IsEnabled = isUsernameAvailable;
                reset();
                return;
            }

            if (HasSpecialCharacters(username) || ContainsKoreanCharacters(username))
            {
                MessageBox.Show("아이디에는 특수문자와 한글을 포함할 수 없습니다.", "중복 확인 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                isUsernameAvailable = false;
                Register_btn.IsEnabled = isUsernameAvailable;
                reset();
                return;
            }

            List<User> collection = dataService.GetAllFromCollection<User>("users");
            collection.Any(u => u.Username == username);

            if (collection.Any(u => u.Username == username))
            {
                MessageBox.Show("이미 존재하는 아이디입니다.", "중복 확인", MessageBoxButton.OK, MessageBoxImage.Error);
                isUsernameAvailable = false;
                reset();
            }
            else
            {
                MessageBox.Show("사용할 수 있는 아이디입니다.", "중복 확인", MessageBoxButton.OK, MessageBoxImage.Information);
                isUsernameAvailable = true;
            }

            Register_btn.IsEnabled = isUsernameAvailable;

        }

        private void reset()
        {
            Username.Text = null;
            Password.Password = null;
            // 다른 컨트롤 초기화 로직 (예: someCheckBox.Checked = false;)
        }

        private bool HasSpecialCharacters(string input)
        {
            return input.Any(c => !Char.IsLetterOrDigit(c));
        }

        // 추가된 부분: 한글 문자 검사
        // 추가된 부분: 한글 문자 검사
        private bool ContainsKoreanCharacters(string input)
        {
            return input.Any(c => c >= 0xAC00 && c <= 0xD7A3 || c >= 0x3131 && c <= 0x318E);
        }

    }
}
