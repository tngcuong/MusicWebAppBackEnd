using MusicWebAppBackend.Infrastructure.ViewModels.Song;
namespace MusicWebAppBackend.Infrastructure.ViewModels.PlayList
{
    public class PlayListProfileDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail {  get; set; }
        public string CreateBy { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime CreateAt { get; set; }
        public IList<SongProfileDto> SongList { get; set;}
    }
}
