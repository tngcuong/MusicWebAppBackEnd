using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.ViewModels.PlayList;
using MusicWebAppBackend.Infrastructure.Models.Const;
using MusicWebAppBackend.Infrastructure.Mappers.Config;
using MusicWebAppBackend.Infrastructure.Models.Paging;

namespace MusicWebAppBackend.Services
{
    public interface IPlayListService
    {
        Task<Payload<Object>> GetPlayList(int pageIndex, int pageSize);
        Task<Payload<PlayListProfileDto>> GetById(string id);
        Task<Payload<PLaylist>> InsertAPlayList(InsertPlayListDto request);
        Task<Payload<PlayListProfileDto>> InsertSongToPlayList(UpdateSongToPlayListDto request);
        void Update(string id, PLaylist PlayList); 
        Task<Payload<PLaylist>> RemoveUserById(String id);
    }

    public class PlayListService : IPlayListService
    {
        public IRepository<PLaylist> _playListRepository;
        public IRepository<User> _userRepository;
        public IRepository<Song> _songRepository;
        public ISongService _songService;
        public IFileService _fileService;
        public PlayListService(IRepository<PLaylist> playListRepository,
            IRepository<User> userRepository,
            IRepository<Song> songRepository,
            IFileService fileService,
            ISongService songService) 
        {
            _songRepository = songRepository;
            _fileService = fileService;
            _songService = songService;
           _playListRepository = playListRepository;
            _userRepository = userRepository;
        }
        public async Task<Payload<PlayListProfileDto>> GetById(string id)
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
                            CreateBy = s.UserId,
                            SongList = new List<SongProfileDto>()
                        }).FirstOrDefault();

            foreach (var item in _playListRepository.GetByIdAsync(id).Result.Songs)
            {
                qure.SongList.Add(_songService.GetById(item).Result.Content);
            }

            return Payload<PlayListProfileDto>.Successfully(qure,PlayListResource.GETSUCCESS);

        }

        public async Task<Payload<object>> GetPlayList(int pageIndex, int pageSize)
        {
            var qure = (from s in _playListRepository.Table
                        where s.IsDeleted == false
                        select new PlayListProfileDto
                        {
                            Id = s.Id,
                            Thumbnail = s.Thumbnail,
                            Name = s.Name,
                            CreateBy= s.UserId,
                            CreateAt = s.CreatedAt,
                            IsPrivate= s.IsPrivate,
                            SongList = new List<SongProfileDto>()
                        }).ToList();
            foreach (var listItem in qure)
            {
                foreach (var item in _playListRepository.GetByIdAsync(listItem.Id).Result.Songs)
                {
                    listItem.SongList.Add(_songService.GetById(item).Result.Content);
                }
            }
           
            var pageList = await PageList<PlayListProfileDto>.Create(qure.AsQueryable(), pageIndex, pageSize);

            if (pageList.Count == 0)
            {
                return Payload<Object>.NotFound(SongResource.NOSONGFOUND);
            }

            return Payload<Object>.Successfully(new
            {
                Data = pageList,
                PageIndex = pageIndex,
                Total = qure.Count(),
                TotalPages = pageList.totalPages
            }, SongResource.GETSUCCESS);
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
            var playlist = await _playListRepository.GetByIdAsync(request.IdPlayList);

            foreach (var item in request.IdSong)
            {
                playlist.Songs.Add(item);
            }
            await _playListRepository.UpdateAsync(playlist);

            PlayListProfileDto playListDto = playlist.MapTo<PLaylist, PlayListProfileDto>();
            foreach (var item in _playListRepository.GetByIdAsync(request.IdPlayList).Result.Songs)
            {
                playListDto.SongList.Add(_songService.GetById(item).Result.Content);
            }
            return Payload<PlayListProfileDto>.Successfully(playListDto);
        }

        public Task<Payload<PLaylist>> RemoveUserById(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(string id, PLaylist PlayList)
        {
            throw new NotImplementedException();
        }
    }
}
