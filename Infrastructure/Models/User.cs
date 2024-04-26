using MusicWebAppBackend.Infrastructure.Models.Entites;
using System.Collections.Generic;

namespace MusicWebAppBackend.Infrastructure.Models
{
    public class User : BaseEntity
    {
        public string? Avatar {  get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public IList<string>? LikedSong { get; set; } = new List<string>();
        public IList<string>? Follower { get; set; } = new List<string>();
        public string Password { get; set; }
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? TokenCreated { get; set; }
        public DateTime? TokenExpires { get; set; }
        public bool? IsDeleted {  get; set; } = false;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
