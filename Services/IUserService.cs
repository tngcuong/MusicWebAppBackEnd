using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MusicWebAppBackend.Infrastructure.Helpers;
using MusicWebAppBackend.Infrastructure.Mappers.Config;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Const;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Models.Paging;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.ViewModels.Account;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using System.Data;
using System.Diagnostics;


namespace MusicWebAppBackend.Services
{
    public interface IUserService
    {
        Task<Payload<Object>> GetUser(int pageIndex, int pageSize);
        Task<Payload<UserProfileDto>> GetUserById(string id);
        Task<Payload<User>> Insert(InsertUserDto request);
        Task<IActionResult> UpdateUserById(string id, User user);
        Task<Payload<User>> RemoveUserById(String id);
    }

    public class UserService : IUserService
    {
        private readonly IFileService _fileService;
        private readonly IRoleService _roleService;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        public UserService( IRepository<User> userRepository,
            IFileService fileService,
            IRoleService roleService,
            IRepository<Role> roleRepository) 
        {
            _roleRepository = roleRepository;
            _fileService = fileService;
            _userRepository = userRepository;
            _roleService = roleService;
        }

        public async Task<Payload<UserProfileDto>> GetUserById(string id)
        {
            var data =  from r in _roleRepository.Table
                                  from u in _userRepository.Table
                                  where r.Users.Contains(id)
                                  where u.IsDeleted == false
                                  select new UserProfileDto
                                  {
                                      Id = u.Id,
                                      Avatar = u.Avatar,
                                      Email = u.Email,
                                      Name = u.Name,
                                      Role = r.Name
                                  };

            if(!data.Any() || data == null) 
            {
                return Payload<UserProfileDto>.NotFound(UserResource.NOUSERFOUND);
            }

            return Payload<UserProfileDto>.Successfully(data.FirstOrDefault(),UserResource.GETUSERSUCCESSFUL);
        }

        public async Task<Payload<User>> Insert(InsertUserDto request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password)
                || string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Role))
            {
                return Payload<User>.BadRequest(AccountResource.NOTENTER);
            }
            if (!Validator.IsValidEmail(request.Email))
            {
                return Payload<User>.BadRequest(AccountResource.VALIDEMAIL);
            }
            if (!Validator.IsValidPassword(request.Password) || !Validator.IsValidPasswordAdvanced(request.Password))
            {
                return Payload<User>.BadRequest(AccountResource.FVALIDPASSWORD);
            }
            var qurey = from a in _userRepository.Table
                        where a.UserName.Equals(request.Username) && a.IsDeleted == false
                        select a;
            if (qurey.Any())
            {
                return Payload<User>.Dublicated();
            }

            var roleData = await _roleService.GetRoleByName(request.Role);
            
            if ((int)roleData.ErrorCode != 200)
            {
                return Payload<User>.ErrorInProcessing(RoleResource.NOTFOUND);
            }

            Role role = roleData.Content;
            User userInfo = new User
            {
                Avatar = await _fileService.SetAvatarDefault(),
                Name = RenderRandomCode.GenerateRandomString(14),
                UserName = request.Username,
                Email = request.Email,
                Password = PasswordHasher.HashPassword(request.Password),
            };

            role.Users.Add(userInfo.Id);
            await _roleRepository.UpdateAsync(role);

            await _userRepository.InsertAsync(userInfo);
            return Payload<User>.Successfully(userInfo, AccountResource.SUCCESSREGIS);
        }

        public async Task<Payload<User>> RemoveUserById(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if(user == null)
            {
                return Payload<User>.ErrorInProcessing(UserResource.NOUSERFOUND);
            }
            user.IsDeleted = true;
            await _userRepository.UpdateAsync(user);
            return Payload<User>.Successfully(user, UserResource.DELETESUCCESS);
        }

        public async Task<IActionResult> UpdateUserById(string id, User user)
        {
            throw new NotImplementedException();
        }

        public async Task<Payload<Object>> GetUser(int pageIndex, int pageSize)
        {
            var qure = from r in _roleRepository.Table
                       from u in _userRepository.Table
                       where r.Users.Contains(u.Id)
                       where u.IsDeleted == false
                       select new UserProfileDto
                       {
                           Id = u.Id,
                           Avatar = u.Avatar,
                           Email = u.Email,
                           Name = u.Name,
                           Role = r.Name
                       };

            var pageList = await PageList<UserProfileDto>.Create(qure, pageIndex, pageSize);

            if (pageList.Count == 0)
            {
                return Payload<Object>.NotFound(UserResource.NOUSERFOUND);
            }

            return Payload<Object>.Successfully(new
            {
                Data = pageList,
                PageIndex = pageIndex,
                Total = pageList.Count,
                TotalPages = pageList.totalPages
            }, UserResource.GETUSERSUCCESSFUL);

        }
    }
}
