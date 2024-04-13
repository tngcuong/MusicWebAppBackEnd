using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using MusicWebAppBackend.Infrastructure.ViewModels.User;

namespace MusicWebAppBackend.Infrastructure.ViewModels.PlayList
{
    public class PlayListProfileDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Thumbnail {  get; set; }
        public string CreateById { get; set; }
        public UserProfileDto CreateBy { get; set; }
        public bool IsPrivate { get; set; }
        public DateTime CreateAt { get; set; }
        public IList<SongProfileDto> SongList { get; set;}
    }
}
