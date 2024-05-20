using Azure;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.Commands;
using MusicWebAppBackend.Infrastructure.Helpers;
using MusicWebAppBackend.Infrastructure.Mappers.Config;
using MusicWebAppBackend.Infrastructure.Mappers.MapingExtensions;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Const;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Utils;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.ViewModels.Account;
using MusicWebAppBackend.Infrastructure.ViewModels.Song;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using NuGet.Common;
using NuGet.Protocol;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;


namespace MusicWebAppBackend.Services
{
    public interface IAccountService
    {
        Task<Payload<AccountRegisterDto>> Register(AccountRegisterDto model);
        Task<Payload<Object>> Login(AccountLoginDto request);
        Task<Payload<User>> Logout(string token);
        Task<Payload<User>> VerifyEmail(VerifyDto request);
        Task<Payload<User>> ChangePasssord(string id, ChangePasswordDto request);
        Task<Payload<User>> UpdateInfo(UpdateAccountDto request);
        Task<Payload<UserProfileDto>> UpdateCoverAvartar (UpdateCoverAvatarDto request);
        Task<Payload<UserProfileDto>> UpdateAvartar(UpdateAvatarDto request);
    }

    public class AccountService : IAccountService
    {
        private readonly ITokenService _tokenService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ConfigEmail _configMail;
        private readonly IMediator _mediator;
        private readonly IRepository<User> _accountRepositoty;
        private readonly IRepository<Role> _roleRepositoty;
        private readonly IFileService _fileService;
        private readonly IRoleService _roleService;

        public AccountService(IRepository<User> accountRepositoty,
            IRepository<Role> roleRepositoty,
            IMediator mediator,
            IRoleService roleService,
            ConfigEmail configMail,
            IHttpContextAccessor httpContextAccessor,
            ITokenService tokenService,
            IFileService fileService)
        {
            _fileService = fileService;
            _tokenService = tokenService;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
            _accountRepositoty = accountRepositoty;
            _configMail = configMail;
            _roleService = roleService;
            _roleRepositoty = roleRepositoty;
        }

        public async Task<Payload<UserProfileDto>> UpdateAvartar(UpdateAvatarDto request)
        {
            User user = await _accountRepositoty.GetByIdAsync(request.Id);
            if (user == null)
            {
                return Payload<UserProfileDto>.BadRequest(AccountResource.NOTFOUND);
            }
            IFormFile Avatar = await _fileService.SetImage(request.Avatar, request.Id);
            if (Avatar.Length < 1 || Avatar == null || Avatar is EmptyFormFile)
            {
                return Payload<UserProfileDto>.BadRequest(FileResource.IMAGEFVALID);
            }

            request.Avatar = Avatar;
            User userUpdated = request.MapTo<UpdateAvatarDto, User>(user);
            await _accountRepositoty.UpdateAsync(userUpdated);

            UserProfileDto userDto = user.ToEntity();
            return Payload<UserProfileDto>.Successfully(userDto, AccountResource.UPDATEAVATARSUCCESS);
        }

        public async Task<Payload<User>> ChangePasssord(string id, ChangePasswordDto request)
        {
            var user = await _accountRepositoty.GetByIdAsync(id);
            if (user == null)
            {
                return Payload<User>.NotFound(AccountResource.NOTFOUND);
            }
            if (!PasswordHasher.VerifyPassword(request.OldPassword, user.Password))
            {
                return Payload<User>.BadRequest(AccountResource.PASSWORDFAIL);
            }
            if (!Validator.IsValidPassword(request.NewPassword) || !Validator.IsValidPasswordAdvanced(request.NewPassword))
            {
                return Payload<User>.BadRequest(AccountResource.FVALIDPASSWORD);
            }

            User newUser = request.MapTo<ChangePasswordDto, User>(user);
            await _accountRepositoty.UpdateAsync(newUser);
            return Payload<User>.Successfully(newUser, AccountResource.CHANGEPW);
        }

        public async Task<Payload<Object>> Login([FromBody] AccountLoginDto request)
        {
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {

                return Payload<Object>.BadRequest(AccountResource.NOTENTER);
            }

            var user = _accountRepositoty.Table.FirstOrDefault(a => a.UserName.Equals(request.UserName) && a.IsDeleted == false);

            if (user == null)
            {
                return Payload<Object>.ErrorInProcessing(AccountResource.NOTFOUND);
            }

            if (!PasswordHasher.VerifyPassword(request.Password, user.Password))
            {
                return Payload<Object>.BadRequest(AccountResource.LOGINFAIL);
            }

            string token = await _tokenService.CreateToken(user);

            var refreshToken = await _tokenService.GenerateRefreshToken();

            await _tokenService.SetRefreshToken(refreshToken, user);
            var data = new
            {
                Token = token,
            };

            return Payload<Object>.Successfully(data, AccountResource.LOGINSUCCESS);
        }

        public async Task<Payload<User>> Logout(string token)
        {
            var userName = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name).ToString();
            var user = _accountRepositoty.Table.FirstOrDefault(a => a.UserName.Equals(userName));
            _httpContextAccessor.HttpContext.Session.Clear();

            return Payload<User>.Successfully(user);
        }

        public async Task<Payload<AccountRegisterDto>> Register(AccountRegisterDto model)
        {
            if (!Validator.IsValidEmail(model.Email))
            {
                return Payload<AccountRegisterDto>.BadRequest(AccountResource.VALIDEMAIL);
            }
            if (!Validator.IsValidPassword(model.Password) || !Validator.IsValidPasswordAdvanced(model.Password))
            {
                return Payload<AccountRegisterDto>.BadRequest(AccountResource.FVALIDPASSWORD);
            }
            var qurey = from a in _accountRepositoty.Table
                        where a.UserName.Equals(model.UserName) && a.IsDeleted == false
                        select a;
            if (qurey.Any())
            {
                return Payload<AccountRegisterDto>.Dublicated(AccountResource.DUPLICATEUSERNAME);
            }

            await SendMailRegister(model.Email);
            return Payload<AccountRegisterDto>.Successfully(model, AccountResource.MAILSUCCESSFUL);
        }

        public async Task<Payload<EmailRegisterDto>> SendMailRegister(string mail)
        {
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("OTP");
            var emailContent = new EmailContent()
            {
                Code = RenderRandomCode.GenerateRandomSixDigitNumber(),
                Email = mail,

            };

            var view = "ResgisterForm";

            var emailHTML = await _mediator.Send(new SendEmailCommand
            {
                ViewName = view,
                Model = emailContent
            });

            await _configMail.SetContent(mail, AccountResource.REGISTITLE, emailHTML);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Domain = "localhost",
                Path = "/",
                SameSite = SameSiteMode.None,
                Secure = true,
                Expires = DateTimeOffset.Now.AddMinutes(3),

            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append("OTP", emailContent.Code.ToString(), cookieOptions);

            return Payload<EmailRegisterDto>.Successfully(new EmailRegisterDto { Email = mail });
        }

        public async Task<Payload<UserProfileDto>> UpdateCoverAvartar(UpdateCoverAvatarDto request)
        {
            User user = await _accountRepositoty.GetByIdAsync(request.Id);
            if (user == null)
            {
                return Payload<UserProfileDto>.BadRequest(AccountResource.NOTFOUND);
            }
            IFormFile CoverAvatar = await _fileService.SetImage(request.CoverAvatar, request.Id);
            if (CoverAvatar.Length < 1 || CoverAvatar == null || CoverAvatar is EmptyFormFile)
            {
                return Payload<UserProfileDto>.BadRequest(FileResource.IMAGEFVALID);
            }

            request.CoverAvatar = CoverAvatar;
            User userUpdated = request.MapTo<UpdateCoverAvatarDto, User>(user);
            await _accountRepositoty.UpdateAsync(userUpdated);

            UserProfileDto userDto = user.ToEntity();
            return Payload<UserProfileDto>.Successfully(userDto, AccountResource.UPDATECOVERAVATARSUCCESS);
        }

        public async Task<Payload<User>> UpdateInfo(UpdateAccountDto request)
        {
            var user = _accountRepositoty.Table.FirstOrDefault(a => a.Id == request.Id);
            if (user == null)
            {
                return Payload<User>.BadRequest(AccountResource.NOTFOUND);
            }

            var data = await _fileService.SetImage(request.Avatar, request.Id);
            if(data.Length == 0 || data == null || data is EmptyFormFile)
            {
                user.Description = request.Description;
                user.Name = request.Name;
                await _accountRepositoty.UpdateAsync(user);
                return Payload<User>.Successfully(user, AccountResource.UPDATESUCCESS);
            }
            request.Avatar = data;
            var userUpdate = request.MapTo<UpdateAccountDto, User>(user);
            await _accountRepositoty.UpdateAsync(userUpdate);
            return Payload<User>.Successfully(user, AccountResource.UPDATESUCCESS);
        }

        public async Task<Payload<User>> VerifyEmail(VerifyDto request)
        {
            var OTP = _httpContextAccessor.HttpContext.Request.Cookies["OTP"];
            if(OTP == null)
            {
                return Payload<User>.BadRequest(AccountResource.EXPRIREOTP);
            }

            if ( OTP != request.OTP)
                return Payload<User>.BadRequest(AccountResource.WRONGOTP);

            User user = new User
            {
                Avatar = await _fileService.SetAvatarDefault(),
                Name = RenderRandomCode.GenerateRandomString(14),
                UserName = request.UserName,
                Email = request.Email,
                Password = PasswordHasher.HashPassword(request.Password),
                CoverAvatar = GenerateRandomColor.GetRandomGradient()
            };

            Role role = await _roleService.GetRoleForUser();
            role.Users.Add(user.Id);
            await _roleRepositoty.UpdateAsync(role);

            await _accountRepositoty.InsertAsync(user);
            _httpContextAccessor.HttpContext.Response.Cookies.Delete("OTP");
            return Payload<User>.Successfully(user, AccountResource.SUCCESSREGIS);
        }
    }
}
