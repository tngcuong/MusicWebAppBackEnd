using MusicWebAppBackend.Infrastructure.Models.Entites;

namespace MusicWebAppBackend.Infrastructure.Models
{
    public class PLaylist : BaseEntity
    {
        public string UserId { get; set; }
        public String Name { get; set; }
        public IList<string> Songs { get; set; }
    }
}
