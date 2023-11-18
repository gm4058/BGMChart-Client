using System;
using System.Drawing;
using System.IO;
using System.Net;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BGMChart

{
    public class Music
    { // 단순히 데이터베이스에서 데이터를 옮기거나 가져올때 한번 거치는 클래스에용 물론 음악과 관련되어 있어용 
        [BsonIgnoreIfDefault]
        public ObjectId _id { get; set; }
        public int Melon_rank { get; set; }
        public int Bugs_rank { get; set; }
        public int Genie_rank { get; set; }
        public double final_ranking { get; set; }
        public string title { get; set; }
        public string singer { get; set; }
        public string imgurl { get; set; }

        public Bitmap GetBitmapFromUrl(string url)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    byte[] bytes = webClient.DownloadData(url);
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        return new Bitmap(ms);
                    }
                }
            }
            catch (Exception ex)
            {
                // 오류 처리 코드
                return null;
            }
        }
    }
}
