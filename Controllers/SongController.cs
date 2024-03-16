using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using MusicWebAppBackend.Services;

namespace MusicWebAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SongController : ControllerBase
    {
        private readonly ISongService _songService;

        public SongController(ISongService songService)
        {
            _songService = songService;
        }


        [Authorize]
        [Route(nameof(Insert))]
        [HttpPost]
        public async Task<ActionResult> Insert([FromQuery] SongInsertDto request)
        {
            var data = await _songService.Insert(request);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetSong))]
        [HttpGet]
        public async Task<ActionResult> GetSong(int pageIndex, int pageSize)
        {
            var data = await _songService.GetSong(pageIndex, pageSize);
            return StatusCode((int)data.ErrorCode, data);
        }
    }
}
