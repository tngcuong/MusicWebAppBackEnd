using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.ViewModels.Account;
using MusicWebAppBackend.Services;

namespace MusicWebAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ITokenService _tokenService;
        public AccountController(IAccountService accountService,
            ITokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;

        }

        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Register))]
        public async Task<ActionResult> Register(AccountRegisterDto account)
        {
            var data = await _accountService.Register(account);
            return StatusCode((int)data.ErrorCode, data);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(VerifyEmail))]
        public async Task<ActionResult> VerifyEmail(VerifyDto request)
        {
            var data = await _accountService.VerifyEmail(request);
            return StatusCode((int)data.ErrorCode, data);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(VerifyChange))]
        public async Task<ActionResult> VerifyChange(VerifyChangeDto request)
        {
            var data = await _accountService.VerifyChange(request);
            return StatusCode((int)data.ErrorCode, data);
        }


        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Login))]
        public async Task<IActionResult> Login(AccountLoginDto request)
        {
            var data = await _accountService.Login(request);
            return StatusCode((int)data.ErrorCode, data);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Logout))]
        public async Task<IActionResult> Logout(string token)
        {
            return Ok(await _accountService.Logout(token));
        }

        [HttpGet]
        [Route(nameof(RefreshToken))]
        public async Task<ActionResult> RefreshToken()
        {
            var idUser = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id").Value;
            var data = await _tokenService.RefreshToken(idUser);
            return StatusCode((int)data.ErrorCode, data);
        }

        [HttpPut]
        [Route(nameof(ChangePassword))]
        public async Task<Payload<User>> ChangePassword(string id, ChangePasswordDto request)
        {
            return await _accountService.ChangePasssord(id, request);
        }

        [HttpPut]
        [Route(nameof(UpdateInfo))]
        public async Task<ActionResult> UpdateInfo([FromForm] UpdateAccountDto request)
        {
            var data = await _accountService.UpdateInfo(request);
            return StatusCode((int)data.ErrorCode, data);
        }

        [HttpPut]
        [Route(nameof(UpdateCoverAvatar))]
        public async Task<ActionResult> UpdateCoverAvatar([FromForm] UpdateCoverAvatarDto request)
        {
            var data = await _accountService.UpdateCoverAvartar(request);
            return StatusCode((int)data.ErrorCode, data);
        }

        [HttpPut]
        [Route(nameof(UpdateAvatar))]
        public async Task<ActionResult> UpdateAvatar([FromForm] UpdateAvatarDto request)
        {
            var data = await _accountService.UpdateAvartar(request);
            return StatusCode((int)data.ErrorCode, data);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(ChangePassword))]
        public async Task<ActionResult> ChangePassword(ResetPasswordDto account)
        {
            var data = await _accountService.ResetPasswordSendMail(account);
            return StatusCode((int)data.ErrorCode, data);
        }
    }
}
