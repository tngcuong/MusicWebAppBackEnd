using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.Mappers.MapingExtensions;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Models.Paging;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using MusicWebAppBackend.Services;

namespace MusicWebAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [Route(nameof(Insert))]
        [HttpPost]
        public async Task<ActionResult> Insert(InsertUserDto request)
        {
            var data = await _userService.Insert(request);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Authorize]
        [Route(nameof(GetUserById))]
        [HttpGet]
        public async Task<ActionResult> GetUserById(string id)
        {
            var data = await _userService.GetUserById(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Authorize]
        [Route(nameof(GetAllUser))]
        [HttpGet]
        public async Task<IActionResult> GetAllUser(int pageIndex, int pageSize)
        {
            var data = await _userService.GetUser(pageIndex, pageSize);
            return StatusCode((int)data.ErrorCode,data);
        }

        [Authorize]
        [Route(nameof(DeleteUserById))]
        [HttpDelete]
        public async Task<ActionResult> DeleteUserById(string id)
        {
            var data = await _userService.RemoveUserById(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Authorize]
        [Route(nameof(UpdateUserById))]
        [HttpPut]
        public async Task<ActionResult> UpdateUserById([FromQuery]string id, UpdateUserDto request)
        {
            var data = await _userService.UpdateUserById(id, request);
            return StatusCode((int)data.ErrorCode, data);
        }
    }
}
