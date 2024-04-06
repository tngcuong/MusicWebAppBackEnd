namespace MusicWebAppBackend.Infrastructure.ViewModels.Song
{
    public class SongInsertDto
    {
        public string UserId { get; set; }
        public IFormFile Img { get; set; }
        public string Name { get; set; }
        public IFormFile Source { get; set; }
        public float DurationTime { get; set; }
    }
}
