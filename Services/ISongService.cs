using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
        Task<Payload<SongProfileDto>> GetById(string id);
        Task<Payload<Song>> Insert(SongInsertDto request);
        void Update(string id, Song song);
        Task<Payload<Song>> RemoveUserById(String id);
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

        public async Task<Payload<SongProfileDto>> GetById(string id)
        {
            var song = await _songRepository.GetByIdAsync(id);
            var songDto = song.MapTo<Song, SongProfileDto>();
            return Payload<SongProfileDto>.Successfully(songDto);
        }

        public async Task<Payload<Song>> Insert(SongInsertDto request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Payload<Song>.NotFound(UserResource.NOUSERFOUND);
            }

            IFormFile imgFile = await _fileService.SetImage(request.Img, request.UserId);
            if (imgFile.Length == 0 || imgFile == null || imgFile is EmptyFormFile)
            {
                return Payload<Song>.BadRequest(FileResource.IMAGEFVALID);
            }

            IFormFile sourceFile = await _fileService.UploadMp3(request.Source, request.UserId);
            if (sourceFile.Length == 0 || sourceFile == null || imgFile is EmptyFormFile)
            {
                return Payload<Song>.BadRequest(FileResource.MP3FVALID);
            }

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

        public async Task<Payload<Song>> RemoveUserById(string id)
        {
            var song = await _songRepository.GetByIdAsync(id);
            if (song == null)
            {
                Payload<Song>.NotFound(SongResource.NOSONGFOUND);
            }

            song.IsDeleted = true;
            await _songRepository.UpdateAsync(song);
            return Payload<Song>.Successfully(song, SongResource.DELETESUCCESS);
        }
    }
}
