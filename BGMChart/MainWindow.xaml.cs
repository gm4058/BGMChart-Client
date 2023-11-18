using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using CefSharp;
using CefSharp.Wpf;
using MongoDB.Driver;

namespace BGMChart
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out System.Drawing.Point lpPoint);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;

        private object _lastSelectedItem = null;

        public List<Music> Musics = new List<Music>();

        DataService dataService;

        public Boolean chart = false;
        public int currentPage = 1;
        public int lastPage;
        public int indexInList = -1;

        public MainWindow(string username)
        {
            InitializeComponent();

            // 이것은 버튼이 안보이는 것입니다용!
            join_btn.Visibility = Visibility.Hidden;
            Search_btn.Visibility = Visibility.Hidden;
            nextPage_btn.Visibility = Visibility.Hidden;
            previous_btn.Visibility = Visibility.Hidden;
            // 이것은 버튼이 안보이는 것입니다용!

            userName_tb.Text = username;
            var viewModel = (MusicViewModel)DataContext;
            viewModel.LoadDataFromMongoDB("Music", "Merge");

            SetupJavascriptMessageHandler();
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
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Account account = new Account();
            account.Show();
            this.Close();
        }

        private void musicDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        // 여기에 데이터 그리드 선택시 수행될 로직을 구현해보아용!!
        private async void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Musics.Clear();

            if (sender is DataGrid dataGrid && dataGrid.SelectedItem != null)
            {
                switch (dataGrid.SelectedItem)
                {
                    case Music selectedRow:
                        song_N.Text = selectedRow.title;
                        singer_N.Text = selectedRow.singer;
                        break;

                    case Tuple<string, long> selectedGenre:
                        song_N.Text = selectedGenre.Item1;
                        break;
                }

                for (int i = 0; i < dataGrid.Items.Count; i++)
                {
                    Music item = dataGrid.Items[i] as Music;

                    Musics.Add(item);
                }
            }

            if (browser.Visibility == Visibility.Visible)
            {
                videoID video = new videoID();

                string[] videoIDs = await video.GetVideoIdFromGoogleSearchAsync(song_N.Text, singer_N.Text);

                //browser.ShowDevTools();
                video.LoadVideoAsync(videoIDs, browser);
            }
        }

        //여기에 기본 송리스트를 추가해 보아용!
        private void Sing_btn_Click(object sender, RoutedEventArgs e)
        {
            musicDataGrid.SelectionChanged -= DataGrid_SelectionChanged;

            // 이것은 버튼이 안보이는 것입니다용!
            join_btn.Visibility = Visibility.Hidden;
            Search_btn.Visibility = Visibility.Hidden;
            nextPage_btn.Visibility = Visibility.Hidden;
            previous_btn.Visibility = Visibility.Hidden;
            search_word_tb.Visibility = Visibility.Hidden;
            // 이것은 버튼이 안보이는 것입니다용!

            // 이것은 버튼이 보이는 겁니다용!!
            Shuffle_btn.Visibility = Visibility.Visible;
            Pre_btn.Visibility = Visibility.Visible;
            Play_btn.Visibility = Visibility.Visible;
            Next_btn.Visibility = Visibility.Visible;
            Like_btn.Visibility = Visibility.Visible;
            singer_N.Visibility = Visibility.Visible;
            browser.Visibility = Visibility.Visible;
            // 이것은 버튼이 보이는 겁니다용!!

            chart = false;

            RestoreOriginalData_Click();


            var viewModel = (MusicViewModel)DataContext;
            viewModel.LoadDataFromMongoDB("Music", "Merge");

            musicDataGrid.SelectionChanged += DataGrid_SelectionChanged;
        }

        //여기에 마이플레이리스트 클릭시 수행할 로직 추가해용!
        private void Myplaylist_btn_Click(object sender, RoutedEventArgs e)
        {
            musicDataGrid.SelectionChanged -= DataGrid_SelectionChanged;

            // 이것은 버튼이 안보이는 것입니다용!
            join_btn.Visibility = Visibility.Hidden;
            Search_btn.Visibility = Visibility.Hidden;
            nextPage_btn.Visibility = Visibility.Hidden;
            previous_btn.Visibility = Visibility.Hidden;
            search_word_tb.Visibility = Visibility.Hidden;
            // 이것은 버튼이 안보이는 것입니다용!

            // 이것은 버튼이 보이는 겁니다용!!
            Shuffle_btn.Visibility = Visibility.Visible;
            Pre_btn.Visibility = Visibility.Visible;
            Play_btn.Visibility = Visibility.Visible;
            Next_btn.Visibility = Visibility.Visible;
            Like_btn.Visibility = Visibility.Visible;
            browser.Visibility = Visibility.Visible;
            // 이것은 버튼이 보이는 겁니다용!!

            chart = true;

            RestoreOriginalData_Click();
            var rankColumn = musicDataGrid.Columns.FirstOrDefault(col => col.Header.ToString() == "순위");
            if (rankColumn != null)
            {
                musicDataGrid.Columns.Remove(rankColumn);
            }

            var viewModel = (MusicViewModel)DataContext;
            viewModel.LoadDataFromMongoDB("PlayList", userName_tb.Text);

            musicDataGrid.SelectionChanged += DataGrid_SelectionChanged;
        }

        // TODO : 여기에 쳇룸 클릭시 수행될 로직을 추가하세용!!
        private void ButtonChatRoom_Click(object sender, RoutedEventArgs e)
        {

            // 이것은 버튼이 안보이는 것입니다용!
            Shuffle_btn.Visibility = Visibility.Hidden;
            Pre_btn.Visibility = Visibility.Hidden;
            Play_btn.Visibility = Visibility.Hidden;
            Next_btn.Visibility = Visibility.Hidden;
            Like_btn.Visibility = Visibility.Hidden;
            singer_N.Visibility = Visibility.Hidden;
            Search_btn.Visibility = Visibility.Hidden;
            browser.Visibility = Visibility.Hidden;
            nextPage_btn.Visibility = Visibility.Hidden;
            previous_btn.Visibility = Visibility.Hidden;
            search_word_tb.Visibility = Visibility.Hidden;
            // 이것은 버튼이 안보이는 것입니다용!

            // 이것은 버튼이 보이는 겁니다용!!
            join_btn.Visibility = Visibility.Visible;
            // 이것은 버튼이 보이는 겁니다용!!

            chart = false;

            musicDataGrid.Columns.Clear();
            dataService = new DataService("ChatRoom");
            List<Tuple<string, long>> genres = dataService.ListCollectionNames();

            // "장르 이름" 칼럼 추가
            var genreColumn = new DataGridTextColumn
            {
                Header = "장르 이름",
                Binding = new Binding("Item1"), // 'Item1'은 Tuple의 첫 번째 항목을 의미한다.
                CanUserSort = false
            };
            musicDataGrid.Columns.Add(genreColumn);

            // "참여자 수" 칼럼 추가
            var participantCountColumn = new DataGridTextColumn
            {
                Header = "참여자 수",
                Binding = new Binding("Item2"),  // 'Item2'은 Tuple의 두 번째 항목을 의미한다.
                CanUserSort = false
            };
            musicDataGrid.Columns.Add(participantCountColumn);

            musicDataGrid.RowHeight = 60;
            musicDataGrid.Columns[0].Width = new DataGridLength(8, DataGridLengthUnitType.Star); // 80% of the DataGrid's total width
            musicDataGrid.Columns[1].Width = new DataGridLength(2, DataGridLengthUnitType.Star); // 20% of the DataGrid's total width
            musicDataGrid.ItemsSource = genres;
        }

        // TODO: 여기에 검색 버튼을 클릭시 할 로직을 추가하세용!!
        private void Searching_btn_Click(object sender, RoutedEventArgs e)
        {
            join_btn.Visibility = Visibility.Hidden;

            Search_btn.Visibility = Visibility.Visible;
            Shuffle_btn.Visibility = Visibility.Visible;
            Pre_btn.Visibility = Visibility.Visible;
            Play_btn.Visibility = Visibility.Visible;
            Next_btn.Visibility = Visibility.Visible;
            Like_btn.Visibility = Visibility.Visible;
            singer_N.Visibility = Visibility.Visible;
            browser.Visibility = Visibility.Visible;
            nextPage_btn.Visibility = Visibility.Visible;
            previous_btn.Visibility = Visibility.Visible;
            search_word_tb.Visibility = Visibility.Visible;

            chart = false;

            RestoreOriginalData_Click();
            var rankColumn = musicDataGrid.Columns.FirstOrDefault(col => col.Header.ToString() == "순위");
            if (rankColumn != null)
            {
                musicDataGrid.Columns.Remove(rankColumn);
            }

            var viewModel = (MusicViewModel)DataContext;
            viewModel.DataIsNull();
        }

        // TODO : 여기에 join 버튼을 클릭 수행될 로직을 추가하세용!!
        private void join_btn_Click(object sender, RoutedEventArgs e)
        {
            dataService = new DataService("ChatRoom");
            if (dataService.InsertUserToCollection(song_N.Text, userName_tb.Text))
            {
                Chatroom chatroom = new Chatroom(userName_tb.Text, song_N.Text);
                chatroom.Show();
                this.Close();
            }
        }

        // TODO : 여기에 텍스트를입력한우 검색버튼을 클릭하면 수행될 로직을 추가헤숑ㅇ!!!
        private void Search_btn_Click(object sender, RoutedEventArgs e)
        {
            currentPage = 1;
            var viewModel = (MusicViewModel)DataContext;
            var (result1, result2) = viewModel.LoadDataCrawilling(search_word_tb.Text, currentPage, lastPage);

            currentPage = result1;  // 문자열을 정수로 변환
            lastPage = result2;     // 문자열을 정수로 변환
        }

        // TODO : 여기에 넥스트 페이지 버튼을 클릭하면 수행될 로직을 추가하세요ㅕㅇ!!!
        private void nextPage_btn_Click(object sender, RoutedEventArgs e)
        {
            currentPage++;
            var viewModel = (MusicViewModel)DataContext;
            (currentPage, lastPage) = viewModel.LoadDataCrawilling(search_word_tb.Text, currentPage, lastPage);
        }

        // TODO : 여기에 이전 페이지 버튼을 클릭하면 수행될 로직을 추가하세용!!!
        private void previous_btn_Click(object sender, RoutedEventArgs e)
        {
            currentPage--;
            var viewModel = (MusicViewModel)DataContext;
            (currentPage, lastPage) = viewModel.LoadDataCrawilling(search_word_tb.Text, currentPage, lastPage);

        }

        // 셔플 버튼 클릭시 수행할 로직
        private void Shuffle_btn_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (MusicViewModel)DataContext;
            viewModel.DataIsShuffle();
        }

        // 이전 버튼 클릭시 수행항 로직
        private async void Pre_btn_Click(object sender, RoutedEventArgs e)
        {
            string selectedTitle = song_N.Text;

            for (int i = 0; i < Musics.Count; i++)
            {
                if (Musics[i].title.Equals(selectedTitle, StringComparison.OrdinalIgnoreCase))
                {
                    indexInList = i;
                    break;
                }
            }

            if (indexInList > 0)  // 첫 번째 행이 아닌 경우
            {
                song_N.Text = Musics[indexInList - 1].title;
                singer_N.Text = Musics[indexInList - 1].singer;

                if (browser.Visibility == Visibility.Visible)
                {
                    videoID video = new videoID();

                    string[] videoIDs = await video.GetVideoIdFromGoogleSearchAsync(song_N.Text, singer_N.Text);

                    //browser.ShowDevTools();
                    video.LoadVideoAsync(videoIDs, browser);
                }
                indexInList--;
            }
            else if (indexInList == -1)
            {
                MessageBox.Show("노래를 선택해주세요", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            else
            {
                MessageBox.Show("첫 번째 행입니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // 플레이 버튼 클릭시 수행할 로직
        private void Play_btn_Click(object sender, RoutedEventArgs e)
        {
            // 현재 마우스 커서 위치를 저장
            GetCursorPos(out System.Drawing.Point originalCursorPosition);

            // WPF 윈도우 내의 상대적 좌표
            Point relativePoint = new Point(750, 222);
            // 상대적 좌표를 화면의 절대 좌표로 변환
            Point absolutePoint = this.PointToScreen(relativePoint);

            // 절대 좌표로 마우스를 이동
            SetCursorPos((int)absolutePoint.X, (int)absolutePoint.Y);

            // 좌표를 65535 기준으로 정규화
            uint normalizedX = (uint)(absolutePoint.X * (65535.0 / SystemParameters.PrimaryScreenWidth));
            uint normalizedY = (uint)(absolutePoint.Y * (65535.0 / SystemParameters.PrimaryScreenHeight));

            // 마우스 이벤트를 특정 좌표에서 발생
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTDOWN, normalizedX, normalizedY, 0, 0);
            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_LEFTUP, normalizedX, normalizedY, 0, 0);

            // 마우스 커서를 원래 위치로 되돌림
            SetCursorPos(originalCursorPosition.X, originalCursorPosition.Y);
        }

        // 다음 버튼 클릭시 수행할 로직
        private async void Next_btn_Click(object sender, RoutedEventArgs e)
        {
            string selectedTitle = song_N.Text;
            for (int i = 0; i < Musics.Count; i++)
            {
                if (Musics[i].title.Equals(selectedTitle, StringComparison.OrdinalIgnoreCase))
                {
                    indexInList = i;
                    break;
                }
            }

            if (indexInList == -1)
            {
                MessageBox.Show("노래를 선택해주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (indexInList < Musics.Count - 1)  // 마지막 행이 아닌 경우
            {
                song_N.Text = Musics[indexInList + 1].title;
                singer_N.Text = Musics[indexInList + 1].singer;

                if (browser.Visibility == Visibility.Visible)
                {
                    videoID video = new videoID();

                    string[] videoIDs = await video.GetVideoIdFromGoogleSearchAsync(song_N.Text, singer_N.Text);

                    //browser.ShowDevTools();
                    video.LoadVideoAsync(videoIDs, browser);
                }
                indexInList++;
            }
            else
            {
                MessageBox.Show("마지막 행입니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // 좋아요 버튼 클릭시 수행할 로직 --> 플레이 리스트 데이터베이스에 저장 해야함..
        private void Like_btn_Click(object sender, RoutedEventArgs e)
        {
            if (musicDataGrid.SelectedItem != null)
            {
                Music selectedMusic = (Music)musicDataGrid.SelectedItem;

                dataService = new DataService("PlayList");
                dataService.LikePlayList(selectedMusic, userName_tb.Text, chart);
            }
            else
            {
                // 선택된 항목이 없는 경우에 대한 처리
                MessageBox.Show("음악을 선택해주세요.", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // 이것은 칼럼을 초기화하는 함수에용!!!
        private void RestoreOriginalData_Click()
        {
            // 기존 DataGrid의 모든 칼럼을 지운다.
            musicDataGrid.Columns.Clear();

            // 순위 칼럼 추가
            if (!musicDataGrid.Columns.Any(col => col.Header.ToString() == "순위"))
            {
                // "순위" 칼럼 추가
                var rankColumn = new DataGridTextColumn
                {
                    Header = "순위",
                    CanUserSort = false
                };

                var style = new Style(typeof(TextBlock));
                style.Setters.Add(new Setter(TextBlock.TextProperty,
                    new Binding { RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1), Path = new PropertyPath("Header") }));

                rankColumn.ElementStyle = style;
                musicDataGrid.Columns.Insert(0, rankColumn); // 첫 번째 위치에 추가

            }

            // 앨범 칼럼 추가
            if (!musicDataGrid.Columns.Any(col => col.Header.ToString() == "앨범"))
            {
                var imgColumn = new DataGridTemplateColumn
                {
                    Header = "앨범",
                    Width = new DataGridLength(60), // 여기에 적절한 너비를 지정하십시오.
                    CanUserSort = false
                };

                var imgTemplate = new DataTemplate();
                var imageFactory = new FrameworkElementFactory(typeof(Image));
                imageFactory.SetValue(Image.SourceProperty, new Binding("imgurl"));
                imageFactory.SetValue(Image.HeightProperty, 50.0);
                imageFactory.SetValue(Image.StretchProperty, Stretch.UniformToFill);
                imgTemplate.VisualTree = imageFactory;

                imgColumn.CellTemplate = imgTemplate;

                musicDataGrid.Columns.Add(imgColumn);
            }

            // 제목 칼럼 추가
            if (!musicDataGrid.Columns.Any(col => col.Header.ToString() == "제목"))
            {
                var titleColumn = new DataGridTextColumn
                {
                    Header = "제목",
                    Width = new DataGridLength(260, DataGridLengthUnitType.Pixel),
                    Binding = new Binding("title"),
                    CanUserSort = false
                };
                musicDataGrid.Columns.Add(titleColumn);
            }

            // 가수 칼럼 추가
            if (!musicDataGrid.Columns.Any(col => col.Header.ToString() == "가수"))
            {
                var singerColumn = new DataGridTextColumn
                {
                    Header = "가수",
                    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                    Binding = new Binding("singer"),
                    CanUserSort = false
                };
                musicDataGrid.Columns.Add(singerColumn);
            }

            var viewModel = (MusicViewModel)DataContext;
            musicDataGrid.ItemsSource = viewModel.Musics;
        }

        // 비디오연동할때 자바스크립트 처리 
        private void SetupJavascriptMessageHandler()
        {
            browser.JavascriptMessageReceived += async (sender, args) =>
            {
                var message = args.Message.ToString();

                if (message == "playNextSong")
                {
                    this.Dispatcher.Invoke(async () =>
                    {
                        Next_btn_Click(sender, new RoutedEventArgs());
                    });
                }
            };
        }
    }
}
