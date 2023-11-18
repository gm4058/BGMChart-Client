using BGMChart;
using HtmlAgilityPack;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace BGMChart
{
    public class MusicViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Music> Musics { get; private set; }

        public int RowNumber { get; set; }

        private static Random rng = new Random();

        public MusicViewModel()
        { // 이것은 단순히 데이터베이스의 값들으 View 즉 사용자 화면에 표시해주는 역활을 해용!

            Musics = new ObservableCollection<Music>();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LoadDataFromMongoDB(string dataBase, string collection)
        {
            Musics.Clear();
            var dataService = new DataService(dataBase);
            var dataFromMongo = dataService.GetAllFromCollection<Music>(collection);

            foreach (var music in dataFromMongo)
            {
                Musics.Add(music);
            }
        }

        public (int, int) LoadDataCrawilling(string searchWord, int currentPage, int lastPage)
        {            
            string url;

            if (currentPage == 1)
            {
                url = "https://music.bugs.co.kr/search/track?q=" + searchWord;
            }

            else if (lastPage < currentPage)
            {
                MessageBox.Show("마지막 페이지 입니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                currentPage = lastPage;
                return (currentPage, lastPage);
            }

            else if (currentPage == 0)
            {
                MessageBox.Show("첫 번째 페이지 입니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                currentPage = 1;
                return (currentPage, lastPage);
            }

            else
            {
                url = "https://music.bugs.co.kr/search/track?q=" + searchWord + "&page=" + currentPage;
            }

            Musics.Clear();
            HtmlWeb web = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc = web.Load(url);
            string indexPath = $"//*[@id='container']/section/div/fieldset/div/table/tbody/tr/td[2]/a/span";
            HtmlNode indexNode = doc.DocumentNode.SelectSingleNode(indexPath);
            string index = indexNode.InnerText;
            string convertindex = new string(index.Where(c => char.IsDigit(c)).ToArray());
            int x = Int32.Parse(convertindex);
            lastPage = x / 50;
            lastPage += 1;
            int songNum = x % 50;


            if (songNum < 50 && lastPage == 1)
            {
                songNum += 1;
                for (int i = 1; i < songNum; i++)
                {
                    string imgPath = $"//*[@id='DEFAULT0']/table/tbody/tr[{i}]/td[2]/a/img";
                    HtmlNode imgNode = doc.DocumentNode.SelectSingleNode(imgPath);
                    string imgurl = imgNode.GetAttributeValue("src", "");

                    string titlePath = $"//*[@id='DEFAULT0']/table/tbody/tr[{i}]/th/p/a";
                    HtmlNode titleNode = doc.DocumentNode.SelectSingleNode(titlePath);
                    string title = titleNode.GetAttributeValue("title", "");

                    string singerPath = $"//*[@id='DEFAULT0']/table/tbody/tr[{i}]/td[4]/p/a";
                    HtmlNode singerNode = doc.DocumentNode.SelectSingleNode(singerPath);
                    string singer;
                    if (singerNode == null)
                    {
                        string exceptionPath = $"//*[@id=\"DEFAULT0\"]/table/tbody/tr[{i}]/td[4]/p/span";
                        HtmlNode exceptionNode = doc.DocumentNode.SelectSingleNode(exceptionPath);
                        singer = exceptionNode.InnerText;
                    }
                    else singer = singerNode.InnerText;
                    Musics.Add(new Music() { title = title, singer = singer, imgurl = imgurl });

                }
            }
            else if (songNum < 50 && lastPage > currentPage)
            {
                for (int i = 1; i < 51; i++)
                {
                    string imgPath = $"//*[@id='DEFAULT0']/table/tbody/tr[{i}]/td[2]/a/img";
                    HtmlNode imgNode = doc.DocumentNode.SelectSingleNode(imgPath);
                    string imgurl = imgNode.GetAttributeValue("src", "");

                    string titlePath = $"//*[@id='DEFAULT0']/table/tbody/tr[{i}]/th/p/a";
                    HtmlNode titleNode = doc.DocumentNode.SelectSingleNode(titlePath);
                    string title = titleNode.GetAttributeValue("title", "");


                    string singerPath = $"//*[@id='DEFAULT0']/table/tbody/tr[{i}]/td[4]/p/a";
                    HtmlNode singerNode = doc.DocumentNode.SelectSingleNode(singerPath);
                    string singer;
                    if (singerNode == null)
                    {
                        string exceptionPath = $"//*[@id=\"DEFAULT0\"]/table/tbody/tr[{i}]/td[4]/p/span";
                        HtmlNode exceptionNode = doc.DocumentNode.SelectSingleNode(exceptionPath);
                        singer = exceptionNode.InnerText;
                    }
                    else singer = singerNode.InnerText;

                    Musics.Add(new Music() { title = title, singer = singer, imgurl = imgurl });

                }
            }
            else if (songNum < 50 && lastPage == currentPage)
            {
                songNum += 1;
                for (int i = 1; i < songNum; i++)
                {
                    string imgPath = $"//*[@id='DEFAULT0']/table/tbody/tr[{i}]/td[2]/a/img";
                    HtmlNode imgNode = doc.DocumentNode.SelectSingleNode(imgPath);
                    string imgurl = imgNode.GetAttributeValue("src", "");

                    string titlePath = $"//*[@id='DEFAULT0']/table/tbody/tr[{i}]/th/p/a";
                    HtmlNode titleNode = doc.DocumentNode.SelectSingleNode(titlePath);
                    string title = titleNode.GetAttributeValue("title", "");


                    string singerPath = $"//*[@id='DEFAULT0']/table/tbody/tr[{i}]/td[4]/p/a";
                    HtmlNode singerNode = doc.DocumentNode.SelectSingleNode(singerPath);
                    string singer;
                    if (singerNode == null)
                    {
                        string exceptionPath = $"//*[@id=\"DEFAULT0\"]/table/tbody/tr[{i}]/td[4]/p/span";
                        HtmlNode exceptionNode = doc.DocumentNode.SelectSingleNode(exceptionPath);
                        singer = exceptionNode.InnerText;
                    }
                    else singer = singerNode.InnerText;

                    Musics.Add(new Music() { title = title, singer = singer, imgurl = imgurl });
                }
            }
            else if (songNum == 0)
            {
                for (int i = 1; i < 51; i++)
                {
                    string imgPath = $"//*[@id='DEFAULT0']/table/tbody/tr[{i}]/td[2]/a/img";
                    HtmlNode imgNode = doc.DocumentNode.SelectSingleNode(imgPath);
                    string imgurl = imgNode.GetAttributeValue("src", "");

                    string titlePath = $"//*[@id='DEFAULT0']/table/tbody/tr[{i}]/th/p/a";
                    HtmlNode titleNode = doc.DocumentNode.SelectSingleNode(titlePath);
                    string title = titleNode.GetAttributeValue("title", "");

                    string singerPath = $"//*[@id='DEFAULT0']/table/tbody/tr[{i}]/td[4]/p/a";
                    HtmlNode singerNode = doc.DocumentNode.SelectSingleNode(singerPath);
                    string singer;
                    if (singerNode == null)
                    {
                        string exceptionPath = $"//*[@id=\"DEFAULT0\"]/table/tbody/tr[{i}]/td[4]/p/span";
                        HtmlNode exceptionNode = doc.DocumentNode.SelectSingleNode(exceptionPath);
                        singer = exceptionNode.InnerText;
                    }
                    else singer = singerNode.InnerText;
                    Musics.Add(new Music() { title = title, singer = singer, imgurl = imgurl });
                }
            }

            return (currentPage, lastPage);
        }

        public void DataIsNull()
        {
            Musics.Clear();
        }

        public void DataIsShuffle()
        {
            var tempList = Musics.ToList();
            int n = tempList.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Music value = tempList[k];
                tempList[k] = tempList[n];
                tempList[n] = value;
            }
            Musics.Clear();
            foreach (var item in tempList)
            {
                Musics.Add(item);
            }
        }
    }
}