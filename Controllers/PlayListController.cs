using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.ViewModels.PlayList;
using MusicWebAppBackend.Infrastructure.ViewModels.Role;
using MusicWebAppBackend.Services;

namespace MusicWebAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayListController : ControllerBase
    {
        private readonly IPlayListService _playListService;
        public PlayListController(IPlayListService playListService) 
        { 
            _playListService = playListService;
        }

        [Route(nameof(CreateAPlayList))]
        [HttpPost]
        public async Task<ActionResult> CreateAPlayList([FromForm]InsertPlayListDto request)
        {
            var data = await _playListService.InsertAPlayList(request);
            return StatusCode((int)data.ErrorCode, data);

        }

        [Route(nameof(GetPlayListById))]
        [HttpGet]
        public async Task<ActionResult> GetPlayListById(string id)
        {
            var data = await _playListService.GetById(id);
            return StatusCode((int)data.ErrorCode, data);

        }

        [Route(nameof(InsertSongToList))]
        [HttpPost]
        public async Task<ActionResult> InsertSongToList(UpdateSongToPlayListDto request)
        {
            var data = await _playListService.InsertSongToPlayList(request);
            return StatusCode((int)data.ErrorCode, data);

        }

        [Route(nameof(GetAllAlbum))]
        [HttpGet]
        public async Task<ActionResult> GetAllAlbum(int pageIndex, int pageSize)
        {
            var data = await _playListService.GetPlayList(pageIndex, pageSize);
            return StatusCode((int)data.ErrorCode, data);

        }
    }
}
