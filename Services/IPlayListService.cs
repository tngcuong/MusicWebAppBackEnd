using FuzzySharp;
using Microsoft.IdentityModel.Tokens;
using MusicWebAppBackend.Infrastructure.Mappers.Config;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Const;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Models.Paging;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.ViewModels.PlayList;
using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using NuGet.Packaging;

namespace MusicWebAppBackend.Services
{
    public interface IPlayListService
    {
        Task<Payload<Object>> GetPlayList(int pageIndex, int pageSize);
        Task<Payload<PlayListProfileDto>> GetPlayListById(string id);
        Task<Payload<PLaylist>> InsertAPlayList(InsertPlayListDto request);
        Task<Payload<PlayListProfileDto>> InsertSongToPlayList(UpdateSongToPlayListDto request);
        void Update(string id, PLaylist PlayList);
        Task<Payload<IList<PlayListProfileDto>>> GetByUserId(string id);
        Task<Payload<IList<PlayListProfileDto>>> GetLikedPlayListByUserId(String id);
        Task<Payload<PLaylist>> RemovePlayListById(string id);
        Task<Payload<IList<PlayListProfileDto>>> SearchPlaylistByName(string? name);
    }

    public class PlayListService : IPlayListService
    {
        public IRepository<PLaylist> _playListRepository;
        public IRepository<User> _userRepository;
        public IRepository<Song> _songRepository;
        public ISongService _songService;
        public IFileService _fileService;
        public IUserService _userService;
        public PlayListService(IRepository<PLaylist> playListRepository,
            IRepository<User> userRepository,
            IRepository<Song> songRepository,
            IFileService fileService,
            ISongService songService,
            IUserService userService)
        {
            _songRepository = songRepository;
            _fileService = fileService;
            _songService = songService;
            _userService = userService;
            _playListRepository = playListRepository;
            _userRepository = userRepository;
        }
        public async Task<Payload<PlayListProfileDto>> GetPlayListById(string id)
        {
            var qure = (from s in _playListRepository.Table
                        where s.IsDeleted == false && s.Id == id
                        select new PlayListProfileDto
                        {
                            Id = s.Id,
                            Thumbnail = s.Thumbnail,
                            Name = s.Name,
                            IsPrivate = s.IsPrivate,
                            CreateAt = s.CreatedAt,
                            CreateById = s.UserId,
                            CreateBy = new UserProfileDto(),
                            SongList = new List<SongProfileDto>()
                        }).FirstOrDefault();

            if (qure == null)
            {
                return Payload<PlayListProfileDto>.NoContent(PlayListResource.NOALBUMFOUND);
            }

            qure.CreateBy = _userService.GetUserById(qure.CreateById).Result.Content;

            foreach (var item in _playListRepository.GetByIdAsync(id).Result.Songs)
            {
                var song = await _songService.GetById(item);
                if (song.Content != null)
                {
                    qure.SongList.Add(song.Content);
                }

            }

            return Payload<PlayListProfileDto>.Successfully(qure, PlayListResource.GETSUCCESS);

        }

        public async Task<Payload<IList<PlayListProfileDto>>> GetByUserId(string id)
        {
            var qure = (from s in _playListRepository.Table
                        where s.IsDeleted == false && s.UserId == id
                        select new PlayListProfileDto
                        {
                            Id = s.Id,
                            Thumbnail = s.Thumbnail,
                            Name = s.Name,
                            IsPrivate = s.IsPrivate,
                            CreateAt = s.CreatedAt,
                            CreateById = s.UserId,
                        }).ToList();

            foreach (var listItem in qure)
            {
                var creator = await _userService.GetUserById(listItem.CreateById);
                if (creator.Content != null)
                {
                    listItem.CreateBy = creator.Content;
                }


                foreach (var item in _playListRepository.GetByIdAsync(listItem.Id).Result.Songs)
                {
                    if (item != null)
                    {
                        var song = await _songService.GetById(item);
                        if (song.Content != null)
                        {
                            listItem.SongList.Add(song.Content);
                        }
                    }

                }
            }

            return Payload<IList<PlayListProfileDto>>.Successfully(qure, PlayListResource.GETSUCCESS);
        }

        public async Task<Payload<object>> GetPlayList(int pageIndex, int pageSize)
        {
            var qure = (from s in _playListRepository.Table
                        join u in _userRepository.Table on s.UserId equals u.Id
                        where s.IsDeleted == false && u.IsDeleted == false
                        select new PlayListProfileDto
                        {
                            Id = s.Id,
                            Thumbnail = s.Thumbnail,
                            Name = s.Name,
                            CreateById = s.UserId,
                            CreateAt = s.CreatedAt,
                            IsPrivate = s.IsPrivate,

                        }).ToList();

            foreach (var listItem in qure)
            {
                var creator = await _userService.GetUserById(listItem.CreateById);
                if (creator.Content != null)
                {
                    listItem.CreateBy = creator.Content;
                }

                var playlist = await _playListRepository.GetByIdAsync(listItem.Id);
                if (playlist != null)
                {
                    foreach (var item in playlist.Songs)
                    {
                        var song = await _songService.GetById(item);
                        if (song.Content != null)
                        {
                            listItem.SongList.Add(song.Content);
                        }
                    }
                }

            }

            var pageList = await PageList<PlayListProfileDto>.Create(qure.AsQueryable(), pageIndex, pageSize);

            if (pageList.Count == 0)
            {
                return Payload<Object>.NotFound(PlayListResource.NOALBUMFOUND);
            }

            return Payload<Object>.Successfully(new
            {
                Data = pageList,
                PageIndex = pageIndex,
                Total = qure.Count(),
                TotalPages = pageList.totalPages
            }, PlayListResource.GETSUCCESS);
        }

        public async Task<Payload<PLaylist>> InsertAPlayList(InsertPlayListDto request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Payload<PLaylist>.NotFound(UserResource.NOUSERFOUND);
            }

            var imgFile = await _fileService.SetImage(request.Thumbnail, request.UserId);
            if (imgFile.Length < 1 || imgFile == null || imgFile is EmptyFormFile)
            {
                return Payload<PLaylist>.BadRequest(FileResource.IMAGEFVALID);
            }

            request.Thumbnail = imgFile;

            PLaylist playList = request.MapTo<InsertPlayListDto, PLaylist>();
            await _playListRepository.InsertAsync(playList);
            return Payload<PLaylist>.Successfully(playList, PlayListResource.CREATESUCCESS);

        }

        public async Task<Payload<PlayListProfileDto>> InsertSongToPlayList(UpdateSongToPlayListDto request)
        {
            try
            {
                var playlist = await _playListRepository.GetByIdAsync(request.IdPlayList);

                playlist.Songs.Clear();
                playlist.Songs.AddRange(request.IdSong);
                await _playListRepository.UpdateAsync(playlist);

                PlayListProfileDto playListDto = playlist.MapTo<PLaylist, PlayListProfileDto>();
                foreach (var item in _playListRepository.GetByIdAsync(request.IdPlayList).Result.Songs)
                {
                    playListDto.SongList.Add(_songService.GetById(item).Result.Content);
                }
                return Payload<PlayListProfileDto>.Successfully(playListDto);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(string id, PLaylist PlayList)
        {
            throw new NotImplementedException();
        }

        public async Task<Payload<IList<PlayListProfileDto>>> GetLikedPlayListByUserId(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Payload<IList<PlayListProfileDto>>.NotFound(UserResource.NOUSERFOUND);
            }

            if (user.LikedPlayList.Count < 1)
            {
                return Payload<IList<PlayListProfileDto>>.NoContent(UserResource.NOLIKEDPLAYLIST);
            }

            IList<PlayListProfileDto> result = new List<PlayListProfileDto>();
            foreach (var item in user.LikedPlayList)
            {
                var playlist = await GetPlayListById(item);
                if (playlist.Content != null)
                {
                    result.Add(playlist.Content);
                }
            }

            return Payload<IList<PlayListProfileDto>>.Successfully(result);
        }

        public async Task<Payload<PLaylist>> RemovePlayListById(string id)
        {
            var playList = await _playListRepository.GetByIdAsync(id);
            if (playList == null)
            {
                return Payload<PLaylist>.NotFound(SongResource.NOSONGFOUND);
            }

            playList.IsDeleted = true;
            await _playListRepository.UpdateAsync(playList);
            return Payload<PLaylist>.Successfully(playList, SongResource.DELETESUCCESS);
        }

        public async Task<Payload<IList<PlayListProfileDto>>> SearchPlaylistByName(string? name)
        {

            IList<PlayListProfileDto> playlistMatching = new List<PlayListProfileDto>();

            if (!name.IsNullOrEmpty() && !name.Equals("undefined"))
            {
                var playlistList = await Task.FromResult(_playListRepository.Table.Where(s => !s.IsDeleted).ToList());
                playlistMatching = await Task.FromResult(
                    (from p in playlistList
                     join u in _userRepository.Table on p.UserId equals u.Id
                     where u.IsDeleted == false
                     select new
                     {
                         Playlist = p,
                         User = u,
                         Similarity = Fuzz.Ratio(p.Name, name)
                     })
                .Where(p => p.Playlist.Name.ToLower().Contains(name.ToLower()))
               .OrderByDescending(p => p.Similarity)
               .Select(p => new PlayListProfileDto
               {
                   Id = p.Playlist.Id,
                   CreateAt = p.Playlist.CreatedAt,
                   Thumbnail = p.Playlist.Thumbnail,
                   CreateById = p.Playlist.UserId,
                   IsPrivate = p.Playlist.IsPrivate,
                   Name = p.Playlist.Name,
               }).ToList());
            }
            else
            {
                playlistMatching = await Task.FromResult((from s in _playListRepository.Table
                                                          join u in _userRepository.Table on s.UserId equals u.Id
                                                          where u.IsDeleted == false && s.IsDeleted == false
                                                          select new PlayListProfileDto
                                                          {
                                                              Id = s.Id,
                                                              Thumbnail = s.Thumbnail,
                                                              Name = s.Name,
                                                              CreateById = s.UserId,
                                                              CreateAt = s.CreatedAt,
                                                              IsPrivate = s.IsPrivate,
                                                          }).ToList());
            }



            if (playlistMatching == null || playlistMatching.Count == 0)
            {
                return Payload<IList<PlayListProfileDto>>.NoContent(PlayListResource.NOALBUMFOUND);
            }


            foreach (var playlist in playlistMatching)
            {
                var user = await _userService.GetUserById(playlist.CreateById);
                if (user.Content == null)
                {
                    playlist.CreateBy = user.Content;
                }

                foreach (var item in _playListRepository.GetByIdAsync(playlist.Id).Result.Songs)
                {
                    if (item != null)
                    {
                        var song = _songService.GetById(item).Result.Content;
                        if (song != null)
                        {
                            playlist.SongList.Add(song);
                        }
                    }

                }
            }

            return Payload<IList<PlayListProfileDto>>.Successfully(playlistMatching, PlayListResource.GETSUCCESS);
        }
    }
}
