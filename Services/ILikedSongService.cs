using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.Models.Const;
using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using System.Collections.Generic;

namespace MusicWebAppBackend.Services
{
    public interface ILikedSongService
    {
        Task<Payload<IList<SongProfileDto>>> GetMostLikedSongByUserId(string id);
        Task<Payload<LikedSongUserDto>> GetLikedSongByUserId(string id);
        Task<Payload<LikedSongUserDto>> AddSongToLiked(string idUser, string idSong);
        Task<Payload<LikedSongUserDto>> RemoveSongToLiked(string idUser, string idSong);
        Task<Payload<LikedSongDto>> GetLikedBySongId (string id);
    }

    public class LikedSongService : ILikedSongService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Song> _songRepository;
        private readonly IUserService _userService;
        private readonly ISongService _songService;
        public LikedSongService(IUserService userService,
            ISongService songService,
            IRepository<User> userRepository,
            IRepository<Song> songRepository
            ) 
        {
            _userRepository = userRepository;
            _songRepository = songRepository;
            _userService = userService;
            _songService = songService;
        }

        public async Task<Payload<LikedSongUserDto>> GetLikedSongByUserId(string id)
        {
            var user = (from u in _userRepository.Table
                        where u.Id == id
                        select u).FirstOrDefault();

            if (user == null)
            {
                return Payload<LikedSongUserDto>.NotFound(UserResource.NOUSERFOUND);
            }

            if (user.LikedSong.Count() < 1)
            {
                return Payload<LikedSongUserDto>.NoContent(SongResource.LIKEDNOCONTENT);
            }

            var result = new LikedSongUserDto();

            foreach (var item in user.LikedSong)
            {
                result.ListSong.Add(_songService.GetById(item).Result.Content);
            }

            return Payload<LikedSongUserDto>.Successfully(result);
        }

        public async Task<Payload<LikedSongUserDto>> AddSongToLiked(string idUser, string idSong)
        {
            var user = (from u in _userRepository.Table
                        where u.Id == idUser
                        select u).FirstOrDefault();
            if (user == null)
            {
                return Payload<LikedSongUserDto>.NotFound(UserResource.LOGIN);
            }

            if (user.LikedSong.Contains(idSong))
            {
                return Payload<LikedSongUserDto>.NotFound(SongResource.LIKED);
            }

            user.LikedSong.Add(idSong);
            await _userRepository.UpdateAsync(user);

            var result = new LikedSongUserDto();

            foreach (var item in user.LikedSong)
            {
                result.ListSong.Add(_songService.GetById(item).Result.Content);
            }

            return Payload<LikedSongUserDto>.Successfully(result);
        }

        public async Task<Payload<LikedSongUserDto>> RemoveSongToLiked(string idUser, string idSong)
        {
            var user = (from u in _userRepository.Table
                        where u.Id == idUser
                        select u).FirstOrDefault();
            if (user == null)
            {
                return Payload<LikedSongUserDto>.NotFound(UserResource.NOUSERFOUND);
            }

            if (!user.LikedSong.Contains(idSong))
            {
                return Payload<LikedSongUserDto>.NotFound();
            }
            user.LikedSong.Remove(idSong);
            await _userRepository.UpdateAsync(user);

            var result = new LikedSongUserDto();

            foreach (var item in user.LikedSong)
            {
                result.ListSong.Add(_songService.GetById(item).Result.Content);
            }

            return Payload<LikedSongUserDto>.Successfully(result, SongResource.UNLIKE);
        }

        public async Task<Payload<IList<SongProfileDto>>> GetMostLikedSongByUserId(string id)
        {
            var topSong = (from u in _userRepository.Table
                           where u.Id == id
                           join s in _songRepository.Table on u.Id equals s.UserId
                           where !s.IsDeleted
                           group s by s.Id into g
                           orderby g.Count() descending, g.Max(s => s.CreatedAt) ascending
                           where g.Count() > 0
                           select g)
               .Take(5)
               .ToList();

            if (topSong.Count < 1 )
            {
                return Payload<IList<SongProfileDto>>.NoContent(SongResource.LIKEDNOCONTENT);
            }

            IList<SongProfileDto> result = new List<SongProfileDto>();

            foreach (var item in topSong)
            {
                result.Add(_songService.GetById(item.Key.ToString()).Result.Content);
            }

            return Payload<IList<SongProfileDto>>.Successfully(result, SongResource.GETSUCCESS);
        }

        public async Task<Payload<LikedSongDto>> GetLikedBySongId(string id)
        {
            try
            {
                //var topSong = (from u in _userRepository.Table 
                //               join s in _songRepository.Table on u.Id equals s.UserId
                //               where !s.IsDeleted && s.Id == id
                //               group s by s.Id into g
                //               orderby g.Count() ascending
                //               select g).FirstOrDefault();

                //var songLikeCounts = from user in _userRepository.Table
                //                     from likedSong in user.LikedSong
                //                     group likedSong by likedSong into songGroup
                //                     select new { SongId = songGroup.Key, LikeCount = songGroup.Count() };

                int likeCount = _userRepository.Table.SelectMany(u => u.LikedSong)
                            .Count(songId => songId == id);

                LikedSongDto likeSong = new LikedSongDto
                {
                     Id = id,
                     Liked = likeCount
                };
                return Payload<LikedSongDto>.Successfully(likeSong);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

            
        }
    }
}
