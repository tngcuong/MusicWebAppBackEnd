using Microsoft.AspNetCore.Http.HttpResults;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Data;

namespace MusicWebAppBackend.Services
{
    public interface ISongService
    {
        Task<List<Song>> GetAll();
        Song GetById(string id);
        Task<Song> Insert(Song song);
        void Update(string id, Song song);
        void Remove(String id);
    }

    public class SongService : ISongService
    {
        private readonly IRepository<Song> _songRepository;
        public SongService(IRepository<Song> songRepository) 
        {
            _songRepository = songRepository;
        }
        public async Task<List<Song>> GetAll()
        {
           return await _songRepository.GetAllAsync();
        }

        public Song GetById(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<Song> Insert(Song song)
        {
            return await _songRepository.InsertAsync(song);
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
