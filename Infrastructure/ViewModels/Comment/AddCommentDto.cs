namespace MusicWebAppBackend.Infrastructure.ViewModels.Comment
{
    public class AddCommentDto
    {
        public string UserId { get; set; } = string.Empty;
        public string SongId { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
