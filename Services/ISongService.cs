using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using MusicWebAppBackend.Infrastructure.Helpers;
using MusicWebAppBackend.Infrastructure.Mappers.Config;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Const;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Models.Paging;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using Org.BouncyCastle.Bcpg;
using System.Drawing.Printing;

namespace MusicWebAppBackend.Services
{
    public interface ISongService
    {
        Task<Payload<Object>> GetSong(int pageIndex, int pageSize);
        Task<Payload<Object>> GetSongDescendingById(string id, int pageIndex, int pageSize);
        Task<Payload<SongProfileDto>> GetById(string id);
        Task<Payload<IList<SongProfileDto>>> GetSongByUserId(string id);
        Task<Payload<Song>> Insert(SongInsertDto request);
        void Update(string id, Song song);
        Task<Payload<Song>> RemoveSongById(String id);

    }

    public class SongService : ISongService
    {
        private readonly IRepository<Song> _songRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        public SongService(IRepository<Song> songRepository,
            IRepository<User> userRepository,
            IFileService fileService,
            IUserService userService)
        {
            _songRepository = songRepository;
            _userRepository = userRepository;
            _fileService = fileService;
            _userService = userService;
        }
        public async Task<Payload<Object>> GetSong(int pageIndex, int pageSize)
        {
            var qure = (from s in _songRepository.Table
                        where s.IsDeleted == false
                        select new SongProfileDto
                        {
                            Id = s.Id,
                            Image = s.Img,
                            Name = s.Name,
                            Source = s.Source,
                            DurationTime = s.DurationTime,
                            CreateAt = s.CreatedAt,
                            UserId = s.UserId,
                            User = new UserProfileDto() { }
                        }).ToList();

            foreach (var item in qure)
            {
                item.User = _userService.GetUserById(item.UserId).Result.Content;
            }

            var pageList = await PageList<SongProfileDto>.Create(qure.AsQueryable(), pageIndex, pageSize);

            if (pageList.Count == 0)
            {
                return Payload<Object>.NoContent(SongResource.NOSONGFOUND);
            }

            return Payload<Object>.Successfully(new
            {
                Data = pageList,
                PageIndex = pageIndex,
                Total = qure.Count(),
                TotalPages = pageList.totalPages
            }, SongResource.GETSUCCESS);
        }

        public async Task<Payload<SongProfileDto>> GetById(string id)
        {
            var qure = (from s in _songRepository.Table
                        where s.IsDeleted == false
                        where s.Id == id
                        select new SongProfileDto
                        {
                            Id = s.Id,
                            Image = s.Img,
                            Name = s.Name,
                            Source = s.Source,
                            DurationTime = s.DurationTime,
                            CreateAt = s.CreatedAt,
                            UserId = s.UserId,
                            User = new UserProfileDto() { }
                        }).FirstOrDefault();

            if (qure == null)
            {
                return Payload<SongProfileDto>.NoContent(SongResource.NOSONGFOUND);
            }

            qure.User = _userService.GetUserById(qure.UserId).Result.Content;

            return Payload<SongProfileDto>.Successfully(qure, SongResource.GETSUCCESS);
        }

        public async Task<Payload<Song>> Insert(SongInsertDto request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Payload<Song>.NoContent(UserResource.NOUSERFOUND);
            }

            IFormFile imgFile = await _fileService.SetImage(request.Img, request.UserId);
            if (imgFile.Length == 0 || imgFile == null || imgFile is EmptyFormFile)
            {
                return Payload<Song>.BadRequest(FileResource.IMAGEFVALID);
            }

            IFormFile sourceFile = await _fileService.UploadMp3(request.Source, request.UserId);
            if (sourceFile.Length == 0 || sourceFile == null || sourceFile is EmptyFormFile)
            {
                return Payload<Song>.BadRequest(FileResource.MP3FVALID);
            }

            request.DurationTime = FunctionHelper.GetMp3Duration(request.Source);
            request.Source = sourceFile;
            request.Img = imgFile;

            Song song = request.MapTo<SongInsertDto, Song>();
            await _songRepository.InsertAsync(song);
            return Payload<Song>.Successfully(song, FileResource.SUCCESS);
        }

        public void Update(string id, Song song)
        {
            throw new NotImplementedException();
        }

        public async Task<Payload<Song>> RemoveSongById(string id)
        {
            var song = await _songRepository.GetByIdAsync(id);
            if (song == null)
            {
                return Payload<Song>.NoContent(SongResource.NOSONGFOUND);
            }

            song.IsDeleted = true;
            await _songRepository.UpdateAsync(song);
            return Payload<Song>.Successfully(song, SongResource.DELETESUCCESS);
        }

        public async Task<Payload<object>> GetSongDescendingById(string id, int pageIndex, int pageSize)
        {
            var qure = (from s in _songRepository.Table
                        where s.IsDeleted == false && s.UserId == id
                        orderby s.CreatedAt descending
                        select new SongProfileDto
                        {
                            Id = s.Id,
                            Image = s.Img,
                            Name = s.Name,
                            Source = s.Source,
                            DurationTime = s.DurationTime,
                            CreateAt = s.CreatedAt,
                            UserId = s.UserId,
                            User = new UserProfileDto() { }
                        }).ToList();

            foreach (var item in qure)
            {
                item.User = _userService.GetUserById(item.UserId).Result.Content;
            }

            var pageList = await PageList<SongProfileDto>.Create(qure.AsQueryable(), pageIndex, pageSize);

            if (pageList.Count == 0)
            {
                return Payload<Object>.NoContent(SongResource.NOSONGFOUND);
            }

            return Payload<Object>.Successfully(new
            {
                Data = pageList,
                PageIndex = pageIndex,
                Total = qure.Count(),
                TotalPages = pageList.totalPages
            }, SongResource.GETSUCCESS);
        }

        public async Task<Payload<IList<SongProfileDto>>> GetSongByUserId(string id)
        {
            var qure = (from s in _songRepository.Table
                        where s.IsDeleted == false
                        where s.UserId == id
                        select new SongProfileDto
                        {
                            Id = s.Id,
                            Image = s.Img,
                            Name = s.Name,
                            Source = s.Source,
                            DurationTime = s.DurationTime,
                            CreateAt = s.CreatedAt,
                            UserId = s.UserId,
                            User = new UserProfileDto() { }
                        }).ToList();

            if (qure == null)
            {
                return Payload<IList<SongProfileDto>>.NoContent(SongResource.NOSONGFOUND);
            }

            foreach (var item in qure)
            {
                item.User = _userService.GetUserById(item.UserId).Result.Content;
            }



            return Payload<IList<SongProfileDto>>.Successfully(qure, SongResource.GETSUCCESS);
        }
    }
}
