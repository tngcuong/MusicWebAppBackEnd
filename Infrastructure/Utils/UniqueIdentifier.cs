using MongoDB.Bson;

namespace MusicWebAppBackend.Infrastructure.Helpers
{
    public class UniqueIdentifier
    {
        public static string New => ObjectId.GenerateNewId().ToString();
    }
}
