using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.Models.Const;
using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using System.Collections.Generic;
using MusicWebAppBackend.Infrastructure.EnumTypes;
using Microsoft.AspNetCore.Mvc;

namespace MusicWebAppBackend.Services
{
    public interface ILikedSongService
    {
        Task<Payload<IList<SongProfileDto>>> GetMostLikedSongByUserId(string id);
        Task<Payload<LikedSongUserDto>> GetLikedSongByUserId(string id);
        Task<Payload<LikedSongUserDto>> AddSongToLiked(string idUser, string idSong);
        Task<Payload<LikedSongUserDto>> RemoveSongToLiked(string idUser, string idSong);
        Task<Payload<LikedSongDto>> GetLikedBySongId(string id);
        Task<Payload<IList<Object>>> GetCollectionUser(UserCollections collection, string id);
    }

    public class LikedSongService : ILikedSongService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Song> _songRepository;
        private readonly IUserService _userService;
        private readonly ISongService _songService;
        private readonly IPlayListService _playListService;
        public LikedSongService(IUserService userService,
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
            var user = await _userRepository.GetByIdAsync(id);
            var songLikesCount = new Dictionary<string, int>();

            foreach (var otherUser in  _userRepository.Table)
            {
                foreach (var likedSongId in otherUser.LikedSong)
                {
                    if (!songLikesCount.ContainsKey(likedSongId))
                    {
                        songLikesCount[likedSongId] = 1;
                    }
                    else
                    {
                        songLikesCount[likedSongId]++;
                    }
                }
            }

            // Sắp xếp danh sách theo số lượt thích giảm dần
            var topLikedSongIds = songLikesCount.OrderByDescending(pair => pair.Value)
                                                .Select(pair => pair.Key)
                                                .Take(5)
                                                .ToList();

            // Lấy thông tin chi tiết của các bài hát được like nhiều nhất
            var topLikedSongs = _songRepository.Table
                                               .Where(song => topLikedSongIds.Contains(song.Id) && song.UserId == id) // Giả sử Id của bài hát được lưu trong field Id của đối tượng Song
                                               .ToList();


            if (topLikedSongs.Count < 1)
            {
                return Payload<IList<SongProfileDto>>.NoContent(SongResource.LIKEDNOCONTENT);
            }

            IList<SongProfileDto> result = new List<SongProfileDto>();

            foreach (var item in topLikedSongs)
            {
                result.Add(_songService.GetById(item.Id.ToString()).Result.Content);
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

        public async Task<Payload<IList<Object>>> GetCollectionUser(UserCollections collection, string id)
        {
            switch (collection)
            {
                case UserCollections.Follower:

                    var data1 = await _userService.GetFollowerByUserId(id);
                    if(data1 == null)
                    {
                        return Payload<IList<Object>>.NoContent(UserResource.NOFOLLOWER);
                    }
                   

                    IList<object> objectList = data1.Content.Select(x => (object)x).ToList();
                    return Payload<IList<Object>>.Successfully(objectList);

                case UserCollections.LikedSong:

                    var data2 = await GetLikedSongByUserId(id);
                    if (data2 == null)
                    {
                        return Payload<IList<Object>>.NoContent(SongResource.LIKEDNOCONTENT); 
                    }
                    IList<object> objectList2 = data2.Content.ListSong.Select(x => (object)x).ToList();
                    return Payload<IList<Object>>.Successfully(objectList2);

                case UserCollections.LikedPlayList:

                    var data3 = await _playListService.GetLikedPlayListByUserId(id);
                    if (data3 == null)
                    {
                        return Payload<IList<Object>>.NoContent(PlayListResource.NOALBUMFOUND);
                    }
                    IList<object> objectList3 = data3.Content.Select(x => (object)x).ToList();
                    return Payload<IList<Object>>.Successfully(objectList3);

                default:
                    // Xử lý trường hợp mặc định (nếu có)
                    break;
            }

            return Payload<IList<Object>>.BadRequest();
        }
    }
}
