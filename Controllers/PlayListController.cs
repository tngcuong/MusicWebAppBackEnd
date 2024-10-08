﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.ViewModels.PlayList;
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
        public async Task<ActionResult> CreateAPlayList([FromForm] InsertPlayListDto request)
        {
            var data = await _playListService.InsertAPlayList(request);
            return StatusCode((int)data.ErrorCode, data);

        }

        [Route(nameof(GetPlayListById))]
        [HttpGet]
        public async Task<ActionResult> GetPlayListById(string id)
        {
            var data = await _playListService.GetPlayListById(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetPlayListByUserId))]
        [HttpGet]
        public async Task<ActionResult> GetPlayListByUserId(string id)
        {
            var data = await _playListService.GetByUserId(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(InsertSongToList))]
        [HttpPost]
        public async Task<ActionResult> InsertSongToList(UpdateSongToPlayListDto request)
        {
            var data = await _playListService.InsertSongToPlayList(request);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetAllPlayList))]
        [HttpGet]
        public async Task<ActionResult> GetAllPlayList(int pageIndex, int pageSize)
        {
            var data = await _playListService.GetPlayList(pageIndex, pageSize);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(RemovePlayListById))]
        [HttpDelete]
        public async Task<ActionResult> RemovePlayListById(string id)
        {
            var data = await _playListService.RemovePlayListById(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(SearchPlaylistByName))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SearchPlaylistByName(string? name)
        {
            var data = await _playListService.SearchPlaylistByName(name);
            return StatusCode((int)data.ErrorCode, data);
        }
    }
}
