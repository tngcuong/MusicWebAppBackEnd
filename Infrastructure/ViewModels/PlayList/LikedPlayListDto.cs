using MusicWebAppBackend.Infrastructure.ViewModels.User;

namespace MusicWebAppBackend.Infrastructure.ViewModels.PlayList
{
    public class LikedPlayListDto
    {
        public UserProfileDto User { get; set; }=new UserProfileDto();
        public IList<PlayListProfileDto> PlayLists { get; set; } = new List<PlayListProfileDto>();
    }
}
