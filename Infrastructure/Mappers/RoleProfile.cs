using AutoMapper;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.Role;

namespace MusicWebAppBackend.Infrastructure.Mappers
{
    public class RoleProfile : Profile
    {
        public RoleProfile() 
        { 
            CreateMap<AddRoleDto,Role>()
                .ForMember(r => r.Name, e => e.MapFrom(e => e.Name));
        }
    }
}
