namespace MusicWebAppBackend.Infrastructure.ViewModels.PlayList
{
    public class InsertPlayListDto
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public IFormFile Thumbnail { get; set; }
        public bool IsPrivate { get; set; }
    }
}
