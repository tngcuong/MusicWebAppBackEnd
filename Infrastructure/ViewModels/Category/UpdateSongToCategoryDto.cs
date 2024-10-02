namespace MusicWebAppBackend.Infrastructure.ViewModels.Category
{
    public class UpdateSongToCategoryDto
    {
        public string IdCate { get; set; }
        public IList<string> IdSong { get; set; }
    }
}
