using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using MusicWebAppBackend.Services;

namespace MusicWebAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SongController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongController(ISongService songService)
        {
            _songService = songService;
        }

        [Route(nameof(Insert))]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Insert([FromForm] SongInsertDto request)
        {
            var data = await _songService.Insert(request);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetSong))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetSong(int pageIndex, int pageSize)
        {
            var data = await _songService.GetSong(pageIndex, pageSize);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetSongAdmin))]
        [HttpGet]
        public async Task<ActionResult> GetSongAdmin(int pageIndex, int pageSize)
        {
            var data = await _songService.GetSongAdmin(pageIndex, pageSize);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(DeleteASongById))]
        [HttpDelete]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteASongById(string id)
        {
            var data = await _songService.RemoveSongById(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetSongById))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetSongById(string id)
        {
            var data = await _songService.GetById(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetSongDescendingByIdUser))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetSongDescendingByIdUser(string id, int pageIndex, int pageSize)
        {
            var data = await _songService.GetSongDescendingById(id, pageIndex, pageSize);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(SearchSongByName))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SearchSongByName(string? name)
        {
            var data = await _songService.SearchSongByName(name);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetRandomSong))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetRandomSong(int? size)
        {
            var data = await _songService.GetRandomSong(size);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(ToggleApproveSongById))]
        [HttpPut]
        public async Task<ActionResult> ToggleApproveSongById(string id)
        {
            var data = await _songService.ToggleApproveSongById(id);
            return StatusCode((int)data.ErrorCode, data);
        }
    }
}
