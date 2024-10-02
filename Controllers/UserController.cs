using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using MusicWebAppBackend.Services;

namespace MusicWebAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        public UserController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }


        [Route(nameof(Insert))]
        [HttpPost]
        public async Task<ActionResult> Insert(InsertUserDto request)
        {
            var data = await _userService.Insert(request);
            return StatusCode((int)data.ErrorCode, data);
        }

        [AllowAnonymous]
        [Route(nameof(GetUserById))]
        [HttpGet]
        public async Task<ActionResult> GetUserById(string id)
        {
            var data = await _userService.GetDetailUserById(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [AllowAnonymous]
        [Route(nameof(GetAllUser))]
        [HttpGet]
        public async Task<IActionResult> GetAllUser(int pageIndex, int pageSize)
        {
            var data = await _userService.GetUser(pageIndex, pageSize);
            return StatusCode((int)data.ErrorCode, data);
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
        public async Task<ActionResult> UpdateUserById([FromQuery] string id, UpdateUserDto request)
        {
            var data = await _userService.UpdateUserById(id, request);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetCurrentUser))]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> GetCurrentUser()
        {
            var id = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id").Value;
            if (id == null) return Unauthorized();
            var data = await _userService.GetUserById(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [AllowAnonymous]
        [Route(nameof(GetFollowerByUserId))]
        [HttpGet]
        public async Task<ActionResult> GetFollowerByUserId(string id)
        {
            var data = await _userService.GetFollowerByUserId(id);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(SearchPeopleByName))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SearchPeopleByName(string? name)
        {
            var data = await _userService.SearchPeopleByName(name);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(ToggleFollowUser))]
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ToggleFollowUser([FromQuery] string id, [FromBody] string userId)
        {
            var data = await _userService.ToggleFollowUser(id, userId);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(GetRandomUser))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetRandomUser(int? size)
        {
            var data = await _userService.GetRandomUser(size);
            return StatusCode((int)data.ErrorCode, data);
        }

        [Route(nameof(CountFollowerByUserId))]
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> CountFollowerByUserId(string id)
        {
            var data = await _userService.CountFollowerByUserId(id);
            return StatusCode((int)data.ErrorCode, data);
        }
    }
}
