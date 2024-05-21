using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.EnumTypes;
using MusicWebAppBackend.Services;
using System.Drawing.Printing;

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

        [Route(nameof(GetLikedSongByUserId))]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetLikedSongByUserId(string id)
        {
            var data = await _likedSongService.GetLikedSongByUserId(id);
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

        [Route(nameof(GetMostLikedSongByUserId))]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetMostLikedSongByUserId(string id)
        {
            var data = await _likedSongService.GetMostLikedSongByUserId(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetLikedBySongId))]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetLikedBySongId(string id)
        {
            var data = await _likedSongService.GetLikedBySongId(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetCollectionByUserId))]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetCollectionByUserId(UserCollections collections, string id)
        {
            var data = await _likedSongService.GetCollectionUser(collections, id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetRalatedSongByUserId))]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> GetRalatedSongByUserId(string id)
        {
            var data = await _likedSongService.GetRalatedSongByUserId(id);
            return StatusCode((int)data.ErrorCode, data);
        }
    }
}
