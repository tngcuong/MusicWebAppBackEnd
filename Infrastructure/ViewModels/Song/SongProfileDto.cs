namespace MusicWebAppBackend.Infrastructure.ViewModels.Song
{
    public class SongProfileDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public float DurationTime { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
