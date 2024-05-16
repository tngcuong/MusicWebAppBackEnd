namespace MusicWebAppBackend.Infrastructure.ViewModels.Account
{
    public class UpdateAvatarDto
    {
        public string Id { get; set; }
        public IFormFile Avatar { get; set; }
    }
}
