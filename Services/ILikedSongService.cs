﻿using MusicWebAppBackend.Infrastructure.EnumTypes;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Const;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using NuGet.Packaging;

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
        Task<Payload<RelateSongDto>> GetRalatedSongByUserId(string id);
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
                return Payload<LikedSongUserDto>.NoContent(UserResource.NOUSERFOUND);
            }

            if (user.LikedSong.Count() < 1)
            {
                return Payload<LikedSongUserDto>.NoContent(SongResource.LIKEDNOCONTENT);
            }

            var result = new LikedSongUserDto();

            foreach (var item in user.LikedSong)
            {
                var likedsong = _songService.GetById(item).Result.Content;
                if (likedsong != null)
                {
                    result.ListSong.Add(likedsong);
                }
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
                return Payload<LikedSongUserDto>.NoContent(UserResource.NOUSERFOUND);
            }

            if (user.LikedSong.Contains(idSong))
            {
                return Payload<LikedSongUserDto>.NoContent(SongResource.LIKED);
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
                return Payload<LikedSongUserDto>.NoContent(UserResource.NOUSERFOUND);
            }

            if (!user.LikedSong.Contains(idSong))
            {
                return Payload<LikedSongUserDto>.NoContent();
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

            foreach (var otherUser in _userRepository.Table)
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

            var topLikedSongIds = songLikesCount.OrderByDescending(pair => pair.Value)
                                                .Select(pair => pair.Key)
                                                .Take(5)
                                                .ToList();

            var topLikedSongs = _songRepository.Table
                                               .Where(song => topLikedSongIds.Contains(song.Id) && song.UserId == id)
                                               .ToList();


            if (topLikedSongs.Count < 1)
            {
                return Payload<IList<SongProfileDto>>.NoContent(SongResource.LIKEDNOCONTENT);
            }

            IList<SongProfileDto> result = new List<SongProfileDto>();

            foreach (var item in topLikedSongs)
            {
                var likedsong = _songService.GetById(item.Id.ToString()).Result.Content;
                if (likedsong != null)
                {
                    result.Add(likedsong);
                }

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
                    if (data1 == null)
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
                    return Payload<IList<Object>>.NotFound();

            }
        }

        public async Task<Payload<RelateSongDto>> GetRalatedSongByUserId(string id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                {
                    return Payload<RelateSongDto>.BadRequest(AccountResource.NOTFOUND);
                }
                RelateSongDto result = new RelateSongDto();
                result.id = id;
                result.RelatedSong.AddRange(GetLikedSongByUserId(id).Result.Content.ListSong);

                return Payload<RelateSongDto>.Successfully(result, SongResource.GETSUCCESS);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
