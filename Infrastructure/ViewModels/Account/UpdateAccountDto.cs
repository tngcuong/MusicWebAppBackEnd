namespace MusicWebAppBackend.Infrastructure.ViewModels.Account
{
    public class UpdateAccountDto
    {
        public string Id { get; set; }
        public IFormFile? Avatar { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
