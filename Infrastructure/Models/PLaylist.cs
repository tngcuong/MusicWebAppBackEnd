using MusicWebAppBackend.Infrastructure.Models.Entites;

namespace MusicWebAppBackend.Infrastructure.Models
{
    public class PLaylist : BaseEntity
    {
        public string UserId { get; set; }
        public String Name { get; set; }
        public string Thumbnail {  get; set; }
        public bool IsPrivate { get; set; }
        public IList<string> Songs { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
    }
}
