using MusicWebAppBackend.Infrastructure.ViewModels.Song;

namespace MusicWebAppBackend.Infrastructure.ViewModels.User
{
    public class UserFollowerDto
    {
        string Id { get; set; }
        public string Name { get; set; }
        public IList<UserProfileDto> Follower { get; set; }
    }
}
