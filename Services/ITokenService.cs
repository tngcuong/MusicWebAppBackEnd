using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.ViewModels;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MusicWebAppBackend.Services
{
    public interface ITokenService
    {
        Task<RefreshToken>  GenerateRefreshToken();
        Task<User> SetRefreshToken(RefreshToken newRefreshToken, User user);
        Task<string> CreateToken(User user);
        Task<Payload<Object>> RefreshToken(string id);

    }

    public class TokenService : ITokenService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IRoleService _roleService;

        public TokenService(IConfiguration configuration,
              IHttpContextAccessor httpContextAccessor,
              IRepository<User> userRepository,
              IRoleService roleService)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _roleService = roleService;

        }
        public async Task<string> CreateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentails = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role,_roleService.GetRoleByIdUser(user.Id).Result.Content.Name),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = credentails,
            };

            var token = new JwtSecurityTokenHandler().CreateToken(tokenDescription);
            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);


            return accessToken;
        }

        public async Task<RefreshToken> GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        public async Task<User> SetRefreshToken(RefreshToken newRefreshToken, User user)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            _httpContextAccessor.HttpContext.Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
            await _userRepository.UpdateAsync(user);
            return user;
        }

        public async Task<Payload<Object>> RefreshToken(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            var refreshToken = _httpContextAccessor.HttpContext.Request.Cookies["refreshToken"];

            if (!user.RefreshToken.Equals(refreshToken))
            {
                return Payload<Object>.BadRequest();
            }
            else if (user.TokenExpires < DateTime.Now)
            {
                return Payload<Object>.BadRequest();
            }

            string token = await CreateToken(user);
            var newRefreshToken = await GenerateRefreshToken();
            await SetRefreshToken(newRefreshToken, user);
            var data = new
            {
                Token = token,
                User = user
            };
            return Payload<Object>.Successfully(data);
        }
    }
}
