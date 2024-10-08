﻿using FuzzySharp;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using MusicWebAppBackend.Infrastructure.Helpers;
using MusicWebAppBackend.Infrastructure.Mappers.Config;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.Models.Const;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Models.Paging;
using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using System.Data;


namespace MusicWebAppBackend.Services
{
    public interface IUserService
    {
        Task<Payload<DetailUserDto>> GetDetailUserById(string id);
        Task<Payload<Object>> GetUser(int pageIndex, int pageSize);
        Task<Payload<UserProfileDto>> GetUserById(string id);
        Task<Payload<User>> Insert(InsertUserDto request);
        Task<Payload<UpdateUserDto>> UpdateUserById(string id, UpdateUserDto user);
        Task<Payload<User>> RemoveUserById(String id);
        Task<Payload<IList<UserProfileDto>>> GetFollowerByUserId(string id);
        Task<Payload<IList<DetailUserDto>>> SearchPeopleByName(string? name);
        Task<Payload<UserProfileDto>> ToggleFollowUser(string id, string idUser);
        Task<Payload<IList<UserProfileDto>>> GetRandomUser(int? size);
        Task<Payload<int>> CountFollowerByUserId(string id);
    }

    public class UserService : IUserService
    {
        //private readonly ISongService _songService;
        private readonly IFileService _fileService;
        private readonly IRoleService _roleService;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<Song> _songRepository;
        public UserService(IRepository<User> userRepository,
            IFileService fileService,
            IRoleService roleService,
            IRepository<Role> roleRepository,
            IRepository<Song> songRepository
            //ISongService songService
            )
        {
            //_songService = songService;
            _roleRepository = roleRepository;
            _songRepository = songRepository;
            _fileService = fileService;
            _userRepository = userRepository;
            _roleService = roleService;

        }

        public async Task<Payload<UserProfileDto>> GetUserById(string id)
        {
            var data = from r in _roleRepository.Table
                       from u in _userRepository.Table
                       where u.Id == id
                       where r.Users.Contains(id)
                       where u.IsDeleted == false
                       select new UserProfileDto
                       {
                           Id = u.Id,
                           Avatar = u.Avatar,
                           Email = u.Email,
                           Name = u.Name,
                           Role = r.Name,
                           Description = u.Description,
                           CoverAvatar = u.CoverAvatar,
                           Following = new List<string>(u.Following),
                           ListSong = new List<string>(u.LikedSong),
                           LikedPlayList = new List<string>(u.LikedPlayList)
                       };

            if (!data.Any() || data == null)
            {
                return Payload<UserProfileDto>.NotFound(UserResource.NOUSERFOUND);
            }

            return Payload<UserProfileDto>.Successfully(data.FirstOrDefault(), UserResource.GETUSERSUCCESSFUL);
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
            if (user == null)
            {
                return Payload<User>.ErrorInProcessing(UserResource.NOUSERFOUND);
            }
            user.IsDeleted = true;
            await _userRepository.UpdateAsync(user);
            return Payload<User>.Successfully(user, UserResource.DELETESUCCESS);
        }

        public async Task<Payload<UpdateUserDto>> UpdateUserById(string id, UpdateUserDto user)
        {
            if (string.IsNullOrEmpty(user.Username) || string.IsNullOrEmpty(user.Password)
                || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Role))
            {
                return Payload<UpdateUserDto>.BadRequest(AccountResource.NOTENTER);
            }
            if (!Validator.IsValidEmail(user.Email))
            {
                return Payload<UpdateUserDto>.BadRequest(AccountResource.VALIDEMAIL);
            }
            if (!Validator.IsValidPassword(user.Password) || !Validator.IsValidPasswordAdvanced(user.Password))
            {
                return Payload<UpdateUserDto>.BadRequest(AccountResource.FVALIDPASSWORD);
            }
            var entity = await _userRepository.GetByIdAsync(id);
            if (entity == null)
            {
                return Payload<UpdateUserDto>.NotFound(UserResource.NOUSERFOUND);
            }
            var existUsername = from u in _userRepository.Table
                                where u.UserName.Equals(user.Username)
                                && u.IsDeleted == false && !u.Id.Equals(id)
                                select u.UserName;
            if (existUsername.Any())
            {
                return Payload<UpdateUserDto>.Dublicated();
            }

            var roleData = await _roleService.GetRoleByName(user.Role);

            if ((int)roleData.ErrorCode != 200)
            {
                return Payload<UpdateUserDto>.ErrorInProcessing(RoleResource.NOTFOUND);
            }
            var userUpdate = user.MapTo<UpdateUserDto, User>(entity);

            var roleEntity = await _roleService.GetRoleByIdUser(entity.Id);
            if (roleData.Content.Name != roleEntity.Content.Name)
            {
                roleEntity.Content.Users.Remove(id);
                await _roleRepository.UpdateAsync(roleEntity.Content);
                Role role = roleData.Content;

                role.Users.Add(userUpdate.Id);
                await _roleRepository.UpdateAsync(role);
            }

            await _userRepository.UpdateAsync(userUpdate);
            return Payload<UpdateUserDto>.Successfully(user, AccountResource.SUCCESSREGIS);

        }

        public async Task<Payload<Object>> GetUser(int pageIndex, int pageSize)
        {
            var qure = (from r in _roleRepository.Table
                        from u in _userRepository.Table
                        where r.Users.Contains(u.Id)
                        where u.IsDeleted == false
                        select new UserProfileDto
                        {
                            Id = u.Id,
                            Avatar = u.Avatar,
                            Email = u.Email,
                            Username = u.UserName,
                            Name = u.Name,
                            Role = r.Name
                        });

            var pageList = await PageList<UserProfileDto>.Create(qure, pageIndex, pageSize);

            if (pageList.Count == 0)
            {
                return Payload<Object>.NotFound(UserResource.NOUSERFOUND);
            }

            return Payload<Object>.Successfully(new
            {
                Data = pageList,
                PageIndex = pageIndex,
                Total = qure.Count(),
                TotalPages = pageList.totalPages
            }, UserResource.GETUSERSUCCESSFUL);

        }

        public async Task<Payload<IList<UserProfileDto>>> GetFollowerByUserId(string id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Payload<IList<UserProfileDto>>.NotFound(UserResource.NOUSERFOUND);
            }

            if (user.Following.Count < 1)
            {
                return Payload<IList<UserProfileDto>>.NoContent(UserResource.NOFOLLOWER);
            }

            IList<UserProfileDto> result = new List<UserProfileDto>();
            foreach (var item in user.Following)
            {
                var userItem = await GetUserById(item);
                if (userItem.Content != null)
                {
                    result.Add(userItem.Content);
                }

            }

            return Payload<IList<UserProfileDto>>.Successfully(result);
        }

        public async Task<Payload<DetailUserDto>> GetDetailUserById(string id)
        {

            if (_userRepository.GetByIdAsync(id).Result == null)
            {
                return Payload<DetailUserDto>.NotFound(UserResource.NOUSERFOUND);
            }
            var data = (from r in _roleRepository.Table
                        from u in _userRepository.Table
                        where u.Id == id
                        where r.Users.Contains(id)
                        where u.IsDeleted == false
                        select new DetailUserDto
                        {
                            Id = u.Id,
                            Avatar = u.Avatar,
                            Email = u.Email,
                            Name = u.Name,
                            Description = u.Description,
                            Role = r.Name,
                            CoverAvatar = u.CoverAvatar,
                            ListSong = new List<string>(u.LikedSong),
                            Following = new List<string>(u.Following)
                        }).FirstOrDefault();

            data.Followers = _userRepository.Table.Where(u => u.Id != id).SelectMany(u => u.Following)
                        .Count(user => user == id);

            data.Tracks = (
                from s in _songRepository.Table
                where s.UserId == id
                select s.Id
                ).ToList();

            if (data == null)
            {
                return Payload<DetailUserDto>.NotFound(UserResource.NOUSERFOUND);
            }

            return Payload<DetailUserDto>.Successfully(data, UserResource.GETUSERSUCCESSFUL);
        }

        public async Task<Payload<IList<DetailUserDto>>> SearchPeopleByName(string? name)
        {

            IList<DetailUserDto> userMatching = new List<DetailUserDto>();


            if (!name.IsNullOrEmpty() && !name.Equals("undefined"))
            {
                var userList = await Task.FromResult(_userRepository.Table.Where(s => s.IsDeleted == false).ToList());
                userMatching = await Task.FromResult(userList
                .Select(p => new
                {
                    User = p,
                    Similarity = Fuzz.Ratio(p.Name, name)
                })
                .OrderByDescending(p => p.Similarity)
                .Select(p => new DetailUserDto
                {
                    Id = p.User.Id,
                    Avatar = p.User.Avatar,
                    CoverAvatar = p.User.CoverAvatar,
                    Description = p.User.Description,
                    Name = p.User.Name,
                    Following = new List<string>(p.User.Following),
                }).ToList());

            }
            else
            {
                userMatching = await Task.FromResult((from p in _userRepository.Table
                                                      where p.IsDeleted == false
                                                      select new DetailUserDto
                                                      {
                                                          Id = p.Id,
                                                          Avatar = p.Avatar,
                                                          CoverAvatar = p.CoverAvatar,
                                                          Description = p.Description,
                                                          Name = p.Name,
                                                          Following = new List<string>(p.Following),
                                                      }).ToList());

            }

            if (userMatching == null || userMatching.Count == 0)
            {
                return Payload<IList<DetailUserDto>>.NoContent(SongResource.NOSONGFOUND);
            }

            foreach (var user in userMatching)
            {
                user.Followers = _userRepository.Table.Where(u => u.Id != user.Id).SelectMany(u => u.Following)
                        .Count(us => us == user.Id);
            }

            return Payload<IList<DetailUserDto>>.Successfully(userMatching, SongResource.GETSUCCESS);
        }

        public async Task<Payload<UserProfileDto>> ToggleFollowUser(string id, string idUser)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return Payload<UserProfileDto>.NoContent(UserResource.NOUSERFOUND);
            }

            if (user.Following.Contains(idUser))
            {
                user.Following.Remove(idUser);
                await _userRepository.UpdateAsync(user);
            }
            else
            {
                user.Following.Add(idUser);
                await _userRepository.UpdateAsync(user);
            }

            UserProfileDto result = user.MapTo<User, UserProfileDto>();
            return Payload<UserProfileDto>.Successfully(result);
        }

        public async Task<Payload<IList<UserProfileDto>>> GetRandomUser(int? size)
        {
            Random random = new Random();
            var qure = (from r in _roleRepository.Table
                        from u in _userRepository.Table
                        where r.Users.Contains(u.Id)
                        where u.IsDeleted == false
                        select new UserProfileDto
                        {
                            Id = u.Id,
                            Avatar = u.Avatar,
                            Email = u.Email,
                            Username = u.UserName,
                            Name = u.Name,
                            Role = r.Name,
                            ListSong = new List<string>(u.LikedSong),
                        }).AsEnumerable()
                          .OrderBy(x => random.Next())
                          .Take(size ?? 3).ToList();
            if (!qure.Any())
            {
                return Payload<IList<UserProfileDto>>.NoContent(UserResource.NOUSERFOUND);
            }

            return Payload<IList<UserProfileDto>>.Successfully(qure, UserResource.GETUSERSUCCESSFUL);
        }

        public async Task<Payload<int>> CountFollowerByUserId(string id)
        {
            var query = _userRepository.Table.Where(u => u.Id != id).SelectMany(u => u.Following)
                        .Count(us => us == id);

            return Payload<int>.Successfully(query, UserResource.NOUSERFOUND);
        }
    }
}
