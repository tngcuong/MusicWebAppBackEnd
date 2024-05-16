using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.PlayList;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.Models.Const;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using System.Runtime.CompilerServices;
using MusicWebAppBackend.Infrastructure.ViewModels.Song;

namespace MusicWebAppBackend.Services
{
    public interface ILikedPlayListService
    {
        Task<Payload<LikedPlayListDto>> ToggleLikePlayList (string userId, string playListId);
        Task<Payload<CountLikedPLayListDto>> GetLikedByPlayListId(string id);
    }

    public class LikedPlayList : ILikedPlayListService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Song> _songRepository;
        private readonly IUserService _userService;
        private readonly ISongService _songService;
        private readonly IPlayListService _playListService;
        public LikedPlayList(IUserService userService,
            IPlayListService playListService,
            ISongService songService,
            IRepository<User> userRepository,
            IRepository<Song> songRepository
            )
        {
            _userRepository = userRepository;
            _songRepository = songRepository;
            _userService = userService;
            _songService = songService;
            _playListService = playListService;
        }

        public async Task<Payload<CountLikedPLayListDto>> GetLikedByPlayListId(string id)
        {
            try
            {
                int likeCount = _userRepository.Table.SelectMany(u => u.LikedPlayList)
                            .Count(playListId => playListId == id);

                CountLikedPLayListDto likedPLaylist = new CountLikedPLayListDto
                {
                    Id = id,
                    Liked = likeCount
                };
                return Payload<CountLikedPLayListDto>.Successfully(likedPLaylist);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Payload<LikedPlayListDto>> ToggleLikePlayList(string userId, string playListId)
        {
            var user = await _userRepository.GetByIdAsync(userId );
            if (user == null)
            {
                return Payload<LikedPlayListDto>.NotFound(UserResource.LOGIN);
            }
            var result = new LikedPlayListDto();
            result.User = _userService.GetUserById(userId).Result.Content;

            if (user.LikedPlayList.Contains(playListId))
            {
                user.LikedPlayList.Remove(playListId);
                await _userRepository.UpdateAsync(user);
            }
            else
            {
                user.LikedPlayList.Add(playListId);
                await _userRepository.UpdateAsync(user);
            }

           

            foreach (var item in user.LikedPlayList)
            {
                result.PlayLists.Add(_playListService.GetPlayListById(item).Result.Content);
            }

            return Payload<LikedPlayListDto>.Successfully(result);
        }
    }
}
