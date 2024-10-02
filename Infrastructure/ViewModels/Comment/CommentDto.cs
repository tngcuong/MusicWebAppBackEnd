using MusicWebAppBackend.Infrastructure.ViewModels.User;

namespace MusicWebAppBackend.Infrastructure.ViewModels.Comment
{
    public class CommentDto
    {
        public string Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public string UserId { get; set; } = string.Empty;
        public string SongId { get; set; } = string.Empty;
        public UserProfileDto User { get; set; } = new UserProfileDto();
    }
}
