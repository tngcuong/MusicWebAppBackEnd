using AutoMapper;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.Role;
using MusicWebAppBackend.Infrastructure.ViewModels.User;

namespace MusicWebAppBackend.Infrastructure.Mappers
{
    public class RoleProfile : Profile
    {
        public RoleProfile()
        {
            CreateMap<AddRoleDto, Role>()
                .ForMember(r => r.Name, e => e.MapFrom(e => e.Name));

            CreateMap<Role, RoleProfileDto>()
             .ForMember(x => x.Name, o => o.MapFrom(u => u.Name))
             .ForMember(x => x.Id, o => o.MapFrom(u => u.Id));
        }
    }
}
