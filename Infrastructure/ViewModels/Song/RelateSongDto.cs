namespace MusicWebAppBackend.Infrastructure.ViewModels.Song
{
    public class RelateSongDto
    {
        public string id { get; set; }
        public IList<SongProfileDto> RelatedSong { get; set; }= new List<SongProfileDto>();
    }
}
