namespace MusicWebAppBackend.Infrastructure.ViewModels.Account
{
    public class UpdateCoverAvatarDto
    {
        public string Id { get; set; }
        public IFormFile CoverAvatar { get; set; }
    }
}
