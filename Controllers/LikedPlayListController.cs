using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Services;

namespace MusicWebAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikedPlayListController : ControllerBase
    {
        private readonly ILikedPlayListService _likedPlaylistService;
        public LikedPlayListController(ILikedPlayListService likedPlaylistService)
        {
            _likedPlaylistService = likedPlaylistService;
        }

        [Route(nameof(ToggleLikePLayList))]
        [HttpPost]
        public async Task<ActionResult> ToggleLikePLayList([FromQuery] string idUser, [FromBody] string idSong)
        {
            var data = await _likedPlaylistService.ToggleLikePlayList(idUser, idSong);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(CountLikedPlaylistByPlayListId))]
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult> CountLikedPlaylistByPlayListId(string id)
        {
            var data = await _likedPlaylistService.GetLikedByPlayListId(id);
            return StatusCode((int)data.ErrorCode, data);
        }
    }
}
