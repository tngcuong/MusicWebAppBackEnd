using MusicWebAppBackend.Infrastructure.Models.Entites;

namespace MusicWebAppBackend.Infrastructure.Models
{
    public class Comment : BaseEntity
    {
        public string Content { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false;
        public string UserId { get; set; } = string.Empty;
        public string SongId { get; set; } = string.Empty;
    }
}
