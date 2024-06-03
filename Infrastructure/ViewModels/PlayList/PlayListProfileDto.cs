using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using MusicWebAppBackend.Infrastructure.ViewModels.User;

namespace MusicWebAppBackend.Infrastructure.ViewModels.PlayList
{
    public class PlayListProfileDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Thumbnail { get; set; }
        public string CreateById { get; set; }
        public UserProfileDto CreateBy { get; set; } = new UserProfileDto();
        public bool IsPrivate { get; set; } = false;
        public DateTime CreateAt { get; set; }
        public IList<SongProfileDto> SongList { get; set; } = new List<SongProfileDto>();
    }
}
