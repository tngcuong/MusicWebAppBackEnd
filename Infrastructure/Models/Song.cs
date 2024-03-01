using MusicWebAppBackend.Infrastructure.Models.Entites;

namespace MusicWebAppBackend.Infrastructure.Models
{
    public class Song : BaseEntity
    {
        public string Name { get; set; }
        public string Img { get; set;}
        public string Source { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public User? UploadBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}
