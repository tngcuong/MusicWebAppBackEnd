using MongoDB.Driver;

namespace MusicWebAppBackend.Infrastructure.Models
{
    public class DbContext
    {
        private readonly IMongoDatabase _database;

        public DbContext()
        {

            var client = new MongoClient(/*"mongodb+srv://cuong:cuong2516252@main.jualbpw.mongodb.net/"*/"mongodb://localhost:27017/");
            _database = client.GetDatabase("MusicWebApp");
        }

        //public IMongoCollection<Users> Users => _database.GetCollection<Users>("Users");
    }
}
