using MusicWebAppBackend.Infrastructure.ViewModels.Song;

namespace MusicWebAppBackend.Infrastructure.ViewModels.User
{
    public class FollowedUserDto
    {
        public UserProfileDto User { get; set; } = new UserProfileDto();
        public IList<UserProfileDto> Followed { get; set; } = new List<UserProfileDto>();
    }
}
