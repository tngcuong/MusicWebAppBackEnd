using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.ViewModels.Account;
using MusicWebAppBackend.Services;
using System.Net;

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
        public async Task<Payload<AccountRegisterDto>> Register(AccountRegisterDto account)
        {
           return await _accountService.Register(account);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route(nameof(VerifyEmail))]
        public async Task<Payload<User>> VerifyEmail(string username, string password, string email, string otp)
        {
            return await _accountService.VerifyEmail(username, password, email, otp);
        }


        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Login))]
        public async Task<IActionResult> Login(AccountLoginDto request)
        {
            var data = await _accountService.Login(request);
            return StatusCode((int)data.ErrorCode,data);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route(nameof(Logout))]
        public async Task<IActionResult> Logout(string token)
        {
            return  Ok(await _accountService.Logout(token));
        }

        [HttpPost]
        [Route(nameof(RefreshToken))]
        public async Task<Payload<Object>> RefreshToken(string id)
        {
            return await _tokenService.RefreshToken(id);
        }

        [HttpPut]
        [Route(nameof(ChangePassword))]
        public async Task<Payload<User>> ChangePassword(string id, ChangePasswordDto request)
        {
            return await _accountService.ChangePasssord(id, request);
        }

        [HttpPut]
        [Route(nameof(UpdateInfo))]     
        public async Task<Payload<User>> UpdateInfo(UpdateAccountDto request)
        {
            return await _accountService.UpdateInfo(request);
        }
    }
}
