using MusicWebAppBackend.Infrastructure.ViewModels.Song;

namespace MusicWebAppBackend.Infrastructure.ViewModels.User
{
    public class UserLikedSongDto
    {
        string Id { get; set; }
        public string Name { get; set; }
        public IList<SongProfileDto> LikedSong { get; set; }
    }
}
