using MusicWebAppBackend.Infrastructure.Models.Entites;

namespace MusicWebAppBackend.Infrastructure.Models
{
    public class Follower : BaseEntity
    {
        public string UserId { get; set; }
        public IList<string> Followers { get; set; }
    }
}
