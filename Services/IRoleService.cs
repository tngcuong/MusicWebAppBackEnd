using MusicWebAppBackend.Infrastructure.ViewModels;
using MusicWebAppBackend.Infrastructure.Models;
using MusicWebAppBackend.Infrastructure.ViewModels.Role;
using MusicWebAppBackend.Infrastructure.Models.Data;
using MusicWebAppBackend.Infrastructure.Mappers.Config;
using MusicWebAppBackend.Infrastructure.Models.Const;

namespace MusicWebAppBackend.Services
{
    public interface IRoleService
    {
        Task<Payload<Role>> Add(AddRoleDto model);
        Task<Payload<Role>> GetRoleByIdUser(string request);
        Task<string> GetRoleNameByIdUser(string request);
        Task<Role> GetRoleForUser(string request="User");
        Task<Payload<Role>> GetRoleByName(string reques );
    }

    public class RoleService : IRoleService
    {
        private readonly IRepository<Role> _roleRepository;
        public RoleService(IRepository<Role> roleRepository) 
        { 
            _roleRepository = roleRepository;
        }

        public async Task<Payload<Role>> Add(AddRoleDto model)
        {
            Role role = model.MapTo<AddRoleDto,Role>();
            await _roleRepository.InsertAsync(role);
            return Payload<Role>.Successfully(role,RoleResource.ADDSUCCESS);
        }

        public async Task<Payload<Role>> GetRoleByIdUser(string request)
        {
            var role = _roleRepository.Table.FirstOrDefault(e => e.Users.Contains(request));
            if (role == null)
            {
                return Payload<Role>.NotFound(RoleResource.NOTFOUND);
            }
            return Payload<Role>.Successfully(role);
        }

        public async Task<string> GetRoleNameByIdUser(string request)
        {
            var role = _roleRepository.Table.FirstOrDefault(e => e.Users.Contains(request));
            if (role == null)
            {
                return "";
            }
            return role.Name;
        }

        public async Task<Payload<Role>> GetRoleByName(string request)
        {
            var role = _roleRepository.Table.FirstOrDefault(e => e.Name.Equals(request));
            if(role == null)
            {
                return Payload<Role>.ErrorInProcessing();
            }
            return Payload<Role>.Successfully(role);
        }

        public async Task<Role> GetRoleForUser(string? request = "User")
        {
            var role = _roleRepository.Table.FirstOrDefault(e => e.Name.Equals(request));
            if (role == null)
            {
                return null;
            }
            return role; 
        }
    }
}
