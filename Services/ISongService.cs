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
        Song GetById(string id);
        Task<Payload<Song>> Insert(SongInsertDto request);
        void Update(string id, Song song);
        void Remove(String id);
    }

    public class SongService : ISongService
    {
        private readonly IRepository<Song> _songRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IFileService _fileService;
        public SongService(IRepository<Song> songRepository,
            IRepository<User> userRepository,
            IFileService fileService)
        {
            _songRepository = songRepository;
            _userRepository = userRepository;
            _fileService = fileService;
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
                            CreateAt = s.CreatedAt,
                        });

            var pageList = await PageList<SongProfileDto>.Create(qure, pageIndex, pageSize);

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

        public Song GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Payload<Song>> Insert(SongInsertDto request)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return Payload<Song>.NotFound(UserResource.NOUSERFOUND);
            }

            if (!await _fileService.SetImage(request.Img, request.UserId))
            {
                return Payload<Song>.BadRequest(FileResource.IMAGEFVALID);
            }

            if (!await _fileService.UploadMp3(request.Source, request.UserId))
            {
                return Payload<Song>.BadRequest(FileResource.MP3FVALID);
            }

            Song song = request.MapTo<SongInsertDto, Song>();
            await _songRepository.InsertAsync(song);
            return Payload<Song>.Successfully(song, FileResource.SUCCESS);
        }

        public void Remove(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(string id, Song song)
        {
            throw new NotImplementedException();
        }
    }
}
