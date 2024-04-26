using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.Models.Const;

namespace MusicWebAppBackend.Services
{
    public interface ILikedSongService
    {
        Task<Payload<LikedSongUserDto>> GetLikedSongById(string id);
        Task<Payload<LikedSongUserDto>> AddSongToLiked(string idUser, string idSong);
        Task<Payload<LikedSongUserDto>> RemoveSongToLiked(string idUser, string idSong);
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

        public async Task<Payload<LikedSongUserDto>> GetLikedSongById(string id)
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
    }
}
