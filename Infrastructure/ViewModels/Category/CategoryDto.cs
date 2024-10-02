using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using MusicWebAppBackend.Infrastructure.ViewModels.User;

namespace MusicWebAppBackend.Infrastructure.ViewModels.Category
{
    public class CategoryDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Image { get; set; }
        public string CreateById { get; set; }
        public UserProfileDto CreateBy { get; set; } = new UserProfileDto();
        public DateTime CreateAt { get; set; }
        public IList<SongProfileDto> SongList { get; set; } = new List<SongProfileDto>();
    }
}
