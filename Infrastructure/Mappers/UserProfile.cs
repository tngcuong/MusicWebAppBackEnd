using AutoMapper;
using MusicWebAppBackend.Infrastructure.Helpers;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.User;

namespace MusicWebAppBackend.Infrastructure.Mappers
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<InsertUserDto, User>()
                .ForMember(x => x.Email, o => o.MapFrom(u => u.Email))
                .ForMember(x => x.UserName, o => o.MapFrom(u => u.Username))
                .ForMember(x => x.Password, o => o.MapFrom(u => u.Password));

            CreateMap<User, UserProfileDto>()
                .ForMember(x => x.Id, o => o.MapFrom(u => u.Id))
                .ForMember(x => x.Name, o => o.MapFrom(u => u.Name))
                .ForMember(x => x.Email, o => o.MapFrom(u => u.Email))
                .ForMember(x => x.Avatar, o => o.MapFrom(u => u.Avatar))
                  .ForMember(x => x.ListSong, o => o.MapFrom(u => u.LikedSong))
                    .ForMember(x => x.LikedPlayList, o => o.MapFrom(u => u.LikedPlayList))
                  .ForMember(x => x.CoverAvatar, o => o.MapFrom(u => u.CoverAvatar));


            CreateMap<UserProfileDto, User>()
                 .ForMember(x => x.Id, o => o.MapFrom(u => u.Id))
               .ForMember(x => x.Name, o => o.MapFrom(u => u.Name))
               .ForMember(x => x.Email, o => o.MapFrom(u => u.Email))
               .ForMember(x => x.Avatar, o => o.MapFrom(u => u.Avatar))
                .ForMember(x => x.LikedSong, o => o.MapFrom(u => u.ListSong))
                    .ForMember(x => x.LikedPlayList, o => o.MapFrom(u => u.LikedPlayList))
                  .ForMember(x => x.CoverAvatar, o => o.MapFrom(u => u.CoverAvatar));

            CreateMap<UpdateUserDto, User>()
                .ForMember(x => x.UserName, o => o.MapFrom(u => u.Username))
                .ForMember(x => x.Email, o => o.MapFrom(u => u.Email))
                .ForMember(x => x.Password, o => o.MapFrom(u => PasswordHasher.HashPassword(u.Password)));
        }
    }
}
