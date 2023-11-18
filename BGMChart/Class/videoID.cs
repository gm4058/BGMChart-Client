using CefSharp;
using CefSharp.Wpf;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BGMChart
{
    public class videoID // 비디오 관련된 모든것들을 관장합니다잉
    {

        // 요것은 비디오 아이디를 가져오는것 입니다잉!
        public async Task<string[]> GetVideoIdFromGoogleSearchAsync(string title, string singer)
        {
            List<string> videoIds = new List<string>();
            for (int start = 0; start <= 20; start += 10)
            {
                string url = $"https://www.google.com/search?q=" + WebUtility.UrlEncode(title) + " " + WebUtility.UrlEncode(singer) + "&tbm=vid&start=" + start;
                HtmlWeb web = new HtmlWeb();
                web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36";
                HtmlAgilityPack.HtmlDocument doc = web.Load(url);
                string indexPath = "//div[@class='nhaZ2c']//a[contains(@href, 'https://www.youtube.com/watch?v=')]";
                HtmlNodeCollection indexNodes = doc.DocumentNode.SelectNodes(indexPath);
                if (indexNodes != null && indexNodes.Count >= 9)
                { // 이 줄을 추가하여 indexNode가 null이 아님을 확인합니다.
                    for (int i = 0; i < indexNodes.Count; i++)
                    {
                        HtmlNode indexNode = indexNodes[i];  // 첫 번째 a 요소

                        string href = indexNodes[i].GetAttributeValue("href", string.Empty);

                        if (!string.IsNullOrEmpty(href) && href.Contains("https://www.youtube.com/watch?v="))
                        {
                            int idStartIndex = href.IndexOf("watch?v=") + "watch?v=".Length;
                            int idEndIndex = href.IndexOf('&');
                            string VideoId = null;

                            if (idEndIndex == -1)
                            { // &가 없는 경우 
                                VideoId = href.Substring(idStartIndex);
                            }
                            else
                            { // &가 있는 경우 
                                VideoId = href.Substring(idStartIndex, idEndIndex - idStartIndex);
                            }
                            videoIds.Add(VideoId);
                        }
                    }
                }
            }
            return videoIds.ToArray();
        }

        // 이것은 비디오 아이디를 html 코드에 적용시키는것 입니다잉 --> main에 넣으면 코드가 너무 길어져서 가독성이 떨어져서 여기 넣었습니다잉!!
        public async void LoadVideoAsync(string[] videoIDs,
                                         ChromiumWebBrowser browser)
        {

            if (videoIDs.Length > 0 && !string.IsNullOrEmpty(videoIDs[0]))
            {

                string videoIDsInJSArray = $"[{string.Join(",", videoIDs.Select(id => $"'{id}'"))}]";

                string html = $@"
                                <html>
                                    <head>
                                        <style>
                                            html, body {{
                                                margin: 0;
                                                padding: 0;
                                            }}
                                            #player {{
                                                position: absolute;
                                                top: 0;
                                                left: 0;
                                                width: 100%;
                                                height: 100%;
                                            }}
                                        </style>
                                        <script>
                                            var player;
                                            var videoIds = {videoIDsInJSArray};
                                            var currentIndex = 0;

                                            function onYouTubeIframeAPIReady() {{
                                                player = new YT.Player('player', {{
                                                    height: '200',
                                                    width: '250',
                                                    videoId: videoIds[currentIndex],
                                                    playerVars: {{
                                                        fs: 0,
                                                        autoplay: 1,
                                                    }},
                                                    events: {{
                                                        'onReady': onPlayerReady,
                                                        'onError': onPlayerError,
                                                        'onStateChange': onPlayerStateChange
                                                    }}
                                                }});
                                            }}

                                            function onPlayerReady(event) {{
                                                event.target.playVideo();

                                            }}

                                            function onPlayerError(event) {{
                                                currentIndex++;
                                                if (currentIndex < videoIds.length) {{
                                                    player.loadVideoById(videoIds[currentIndex]);
                                                }}
                                            }}
                                            function onPlayerStateChange(event) {{
                                                console.log(""Player state changed to: "", event.data);
                                                if (event.data == YT.PlayerState.ENDED) {{
                                                    console.log(""Video ended. Sending playNextSong message to CefSharp."");
                                                    CefSharp.PostMessage('playNextSong');
                                                }}
                                            }}
                                        </script>
                                        <script src='https://www.youtube.com/iframe_api'></script>
                                    </head>
                                    <body>
                                        <div id='player'></div>
                                    </body>
                                </html>";

                if (!string.IsNullOrEmpty(html))
                {
                    browser.LoadHtml(html);
                }
            }
        }


    }
}

