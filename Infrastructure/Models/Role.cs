using MusicWebAppBackend.Infrastructure.Models.Entites;

namespace MusicWebAppBackend.Infrastructure.Models
{
    public class Role : BaseEntity
    {
        public Role()
        {
            Users = new List<string>();
        }
        public string Name { get; set; }
        public IList<string> Users { get; set; }
    }
}
