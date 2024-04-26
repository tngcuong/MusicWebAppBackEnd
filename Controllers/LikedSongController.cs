using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Services;

namespace MusicWebAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikedSongController : ControllerBase
    {
        private readonly ILikedSongService _likedSongService;
        public LikedSongController(ILikedSongService likedSongService) 
        { 
            _likedSongService = likedSongService;
        }

        [Route(nameof(GetLikedSongById))]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetLikedSongById(string id)
        {
            var data = await _likedSongService.GetLikedSongById(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(AddSongToLikedSong))]
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> AddSongToLikedSong([FromQuery] string idUser, [FromBody] string idSong)
        {
            var data = await _likedSongService.AddSongToLiked(idUser, idSong);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(RemoveSongToLikedSong))]
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> RemoveSongToLikedSong([FromQuery] string idUser,[FromBody] string idSong)
        {
            var data = await _likedSongService.RemoveSongToLiked(idUser, idSong);
            return StatusCode((int)data.ErrorCode, data);
        }
    }
}
