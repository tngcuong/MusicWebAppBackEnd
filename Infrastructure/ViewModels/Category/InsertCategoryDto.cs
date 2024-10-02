namespace MusicWebAppBackend.Infrastructure.ViewModels.Category
{
    public class InsertCategoryDto
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public IFormFile Thumbnail { get; set; }
    }
}
