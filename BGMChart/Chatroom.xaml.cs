using BGMChart.Themes;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace BGMChart  
{
    public partial class Chatroom : Window
    {
        string currentUsername;
        string genre;

        SocketIOClient.SocketIO client;
        ChatViewModel chatViewModel = new ChatViewModel();


        public Chatroom(string username, string genre)
        {
            InitializeComponent();

            this.currentUsername = username;
            this.genre = genre;

            this.DataContext = chatViewModel;

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
            if (System.Windows.Application.Current.MainWindow.WindowState != WindowState.Maximized)
                System.Windows.Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                System.Windows.Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            DataService dataService = new DataService("ChatRoom");
            dataService.ChatRoomOut(currentUsername, genre);

            client.EmitAsync("leave", new { userName = currentUsername, room = genre });

            MainWindow main = new MainWindow(currentUsername);
            main.Show();
            this.Close();
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (messageBox.Text != string.Empty)
            {
                var messageData = new
                {
                    room = genre,
                    msg = $"{currentUsername}: {messageBox.Text}"
                };
                
                if (client == null)
                {
                    System.Windows.MessageBox.Show("서버에 연결되지 않았습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    await client.EmitAsync("message", messageData);
                }
                messageBox.Clear();
            }
        }

        private async void Conversation_Loaded(object sender, RoutedEventArgs e)
        {
            // 이벤트 처리 로직


            DataService dataService = new DataService("ChatRoom");

            if (genre == "Ballade") { browser.Load("http://13.124.230.155:8000/Ballade.mp3"); }
            else if (genre == "Dance") { browser.Load("http://13.124.230.155:8000/Dance.mp3"); }
            else if (genre == "Folk") { browser.Load("http://13.124.230.155:8000/Folk.mp3"); }
            else if (genre == "HipHop") { browser.Load("http://13.124.230.155:8000/HipHop.mp3"); }
            else if (genre == "Indie") { browser.Load("http://13.124.230.155:8000/Indie.mp3"); }
            else if (genre == "RB") { browser.Load("http://13.124.230.155:8000/RB.mp3"); }
            else if (genre == "Rock") { browser.Load("http://13.124.230.155:8000/Rock.mp3"); }
            else if (genre == "Trot") { browser.Load("http://13.124.230.155:8000/Trot.mp3"); }


            // 채팅방에 연결하는 코드 추가
            client = new SocketIOClient.SocketIO("http://13.124.230.155:5000");

            client.OnConnected += Client_OnConnected;

            client.On("message", response =>
            {
                var messageData = response.GetValue<System.Collections.Generic.Dictionary<string, string>>();
                var message = messageData["msg"];

                var splitMessage = message.Split(':');
                var username = splitMessage[0].Trim();
                var content = splitMessage.Length > 1 ? splitMessage[1].Trim() : message;

                // "has entered the room" 또는 "has left the room"이 포함된 메시지를 검사
                if (content.Contains("has entered the room") || content.Contains("has left the room"))
                {
                    username = "Narration";
                }

                Dispatcher.Invoke(() =>
                {
                    var status = username == currentUsername ? "sent" : "Received";
                    chatViewModel.Messages.Add(new ConversationMessages { Username = username, Message = content, MessageStatus = status });
                    chatConversation.ScrollToLatestMessage();
                });
            });

            try
            {
                await client.ConnectAsync();
            }
            catch (Exception ex)
            {

            }

        }

        private void Client_OnConnected(object sender, EventArgs e)
        {
            client.EmitAsync("join", new { userName = currentUsername, room = genre });
        }

        private void MessageTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Keyboard.IsKeyDown(Key.LeftShift) && !Keyboard.IsKeyDown(Key.RightShift)) // Shift+Enter를 허용하려면 이 조건을 추가합니다.
            {
                SendButton_Click(this, new RoutedEventArgs());
                e.Handled = true; // 이벤트가 처리되었음을 표시합니다.
            }
        }
    } 
}