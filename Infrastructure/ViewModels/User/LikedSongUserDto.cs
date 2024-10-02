using MusicWebAppBackend.Infrastructure.ViewModels.Song;

namespace MusicWebAppBackend.Infrastructure.ViewModels.User
{
    public class LikedSongUserDto
    {
        public IList<SongProfileDto> ListSong { get; set; } = new List<SongProfileDto>();
    }
}
