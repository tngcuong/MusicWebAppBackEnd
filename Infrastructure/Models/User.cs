﻿using MusicWebAppBackend.Infrastructure.Models.Entites;

namespace MusicWebAppBackend.Infrastructure.Models
{
    public class User : BaseEntity
    {
        public string? Avatar { get; set; }
        public string? Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? CoverAvatar { get; set; } = string.Empty;
        public IList<string>? LikedSong { get; set; } = new List<string>();
        public IList<string>? Following { get; set; } = new List<string>();
        public IList<string>? LikedPlayList { get; set; } = new List<string>();
        public string Password { get; set; } = string.Empty;
        public string? RefreshToken { get; set; } = string.Empty;
        public DateTime? TokenCreated { get; set; }
        public DateTime? TokenExpires { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
