using MusicWebAppBackend.Infrastructure.ViewModels.PlayList;

namespace MusicWebAppBackend.Infrastructure.ViewModels.User
{
    public class UserLikedPlayListDto
    {
        string Id { get; set; }
        public string Name { get; set; }
        public IList<PlayListProfileDto> LikedSong { get; set; }
    }
}
