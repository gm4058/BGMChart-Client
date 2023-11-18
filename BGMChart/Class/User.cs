using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace BGMChart
{
    public class User
    { // 단순히 데이터베이스에서 데이터를 옮기거나 가져올때 한번 거치는 클래스에용 물론 유저와 관련되어 있어용

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
