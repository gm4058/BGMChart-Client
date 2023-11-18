using BGMChart;
using CefSharp.DevTools.Database;
using CefSharp.DevTools.IO;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Forms;

namespace BGMChart
{
    public class DataService // 데이터베이스 연결 등록 정보가져오는거 전부 여기서 관리해용!
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public DataService(string databaseName)
        {
            _client = new MongoClient("mongodb+srv://KSH:1q2w3e4r!@musicdb.zwhnas5.mongodb.net/?retryWrites=true&w=majority");
            _database = _client.GetDatabase(databaseName);
        }

        public List<T> GetAllFromCollection<T>(string collectionName)
        {
            var collection = _database.GetCollection<T>(collectionName);
            return collection.Find(_ => true).ToList();
        }

        // "users"는 사용자 정보를 저장할 컬렉션의 이름입니다.
        public void RegisterUser(User user)
        {
            var collection = _database.GetCollection<User>("users");  
            collection.InsertOne(user);
        }

        // 아이디 삭제할때 데이터 삭제
        public Boolean UnregisterUser(User user)
        {
            var collection = _database.GetCollection<User>("users");
            var filter = Builders<User>.Filter.Eq(u => u.Username, user.Username) & Builders<User>.Filter.Eq(u => u.Password, user.Password);
            var result = collection.DeleteOne(filter);

            if (result.DeletedCount == 0)
            {
                System.Windows.MessageBox.Show("잘못된 아이디 또는 비밀번호입니다.", "회원탈퇴 실패", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            else
            {
                System.Windows.MessageBox.Show("회원탈퇴가 완료되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
                _database.DropCollection(user.Username);

                var playListDatabase = _client.GetDatabase("PlayList");

                playListDatabase.DropCollection(user.Username);
                return true;
            }
        }

        public List<Tuple<string, long>> ListCollectionNames()
        {
            List<Tuple<string, long>> collectionNamesAndCounts = new List<Tuple<string, long>>();

            var collectionNames = _database.ListCollectionNames().ToList();

            foreach (var collectionName in collectionNames)
            {
                var collection = _database.GetCollection<BsonDocument>(collectionName);
                long count = collection.CountDocuments(new BsonDocument());
                collectionNamesAndCounts.Add(Tuple.Create(collectionName, count));
            }

            return collectionNamesAndCounts;
        }

        public IAsyncCursor<BsonDocument> ListCollections(ListCollectionsOptions options)
        {
            return _database.ListCollections(options);
        }

        // 플레이리스트 데이터
        public void LikePlayList(Music selectedMusic, string collectionName, Boolean chart)
        {
            var filter = Builders<Music>.Filter.Eq<string>("title", selectedMusic.title) &
                         Builders<Music>.Filter.Eq<string>("singer", selectedMusic.singer);

            var collection = _database.GetCollection<Music>(collectionName);

            var duplicateMusic = collection.Find(filter).FirstOrDefault();

            if (duplicateMusic != null)
            {
                if(chart)
                {
                    collection.DeleteOne(filter);
                    System.Windows.MessageBox.Show("노래가 삭제되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    System.Windows.MessageBox.Show("이미 추가된 노래입니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                collection.InsertOne(selectedMusic);
                System.Windows.MessageBox.Show("노래가 추가되었습니다.", "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        // 채팅방 입장시 username 데이터 넣기
        public Boolean InsertUserToCollection(string collectionName, string username)
        {
            var collection = _database.GetCollection<BsonDocument>(collectionName);
            var filter = Builders<BsonDocument>.Filter.Eq("userName", username);
            var existingUser = collection.Find(filter).FirstOrDefault();

            if (existingUser != null)
            {
                System.Windows.MessageBox.Show($"'{username}'는(은) 이미 존재하는 사용자입니다.", "경고", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            else
            {
                BsonDocument document = new BsonDocument
                {
                    { "userName", username }
                };
                collection.InsertOne(document);
                return true;
            }
        }

        // 채팅방 입장시 
        public Boolean ChatRoomIn(string username, string collectionName)
        {
            var collection = _database.GetCollection<Music>(collectionName);
            var filter = Builders<Music>.Filter.Eq("userName", username);
            long count = collection.CountDocuments(filter);

            if (count > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // 채팅방 나갈시
        public void ChatRoomOut(string username, string collectionName)
        {
            var collection = _database.GetCollection<Music>(collectionName);
            var filter = Builders<Music>.Filter.Eq("userName", username);
            collection.DeleteOne(filter);
        }

    }
}