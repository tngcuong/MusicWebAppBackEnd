﻿namespace MusicWebAppBackend.Infrastructure.ViewModels.User
{
    public class UserProfileDto
    {
        public string Id { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Description { get; set; }
        public string CoverAvatar { get; set; }
        public List<string> Following { get; set; } = new List<string>();
        public List<string> ListSong { get; set; } = new List<string>();
        public List<string> LikedPlayList { get; set; } = new List<string>();
    }
}
