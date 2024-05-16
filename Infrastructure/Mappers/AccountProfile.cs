using AutoMapper;
using MusicWebAppBackend.Infrastructure.Helpers;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.Account;
using MusicWebAppBackend.Infrastructure.ViewModels.User;

namespace MusicWebAppBackend.Infrastructure.Mappers
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<AccountRegisterDto, User>()
                .ForMember(x => x.Email, o => o.MapFrom(u => u.Email))
                .ForMember(x => x.UserName, o => o.MapFrom(u => u.UserName))
                .ForMember(x => x.Password, o => o.MapFrom(u => u.Password));

            CreateMap<ChangePasswordDto, User>()
                .ForMember(x => x.Password, o => o.MapFrom(u => PasswordHasher.HashPassword(u.NewPassword)));

            CreateMap<UpdateAccountDto, User>()
               .ForMember(x => x.Avatar, o => o.MapFrom(u => "https://musicwebapp.blob.core.windows.net/" + u.Id + "/" + u.Avatar.FileName))
               .ForMember(x => x.Name, o => o.MapFrom(u => u.Name))
               .ForMember(x => x.Description, o => o.MapFrom(u => u.Description))
               .ForMember(x => x.Id, o => o.Ignore());

            CreateMap<UpdateAvatarDto, User>()
             .ForMember(x => x.Avatar, o => o.MapFrom(u => "https://musicwebapp.blob.core.windows.net/" + u.Id + "/" + u.Avatar.FileName))
             .ForMember(x => x.Id, o => o.Ignore());

            CreateMap<UpdateCoverAvatarDto, User>()
            .ForMember(x => x.CoverAvatar, o => o.MapFrom(u => "https://musicwebapp.blob.core.windows.net/" + u.Id + "/" + u.CoverAvatar.FileName))
            .ForMember(x => x.Id, o => o.Ignore());
        }
    }
}
