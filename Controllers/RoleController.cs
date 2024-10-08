﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.ViewModels.Role;
using MusicWebAppBackend.Services;

namespace MusicWebAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }


        [Route(nameof(Insert))]
        [HttpPost]
        public async Task<ActionResult> Insert(AddRoleDto request)
        {
            var data = await _roleService.Add(request);
            return StatusCode((int)data.ErrorCode, data);

        }

        [Route(nameof(GetAll))]
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var data = await _roleService.GetRole();
            return StatusCode((int)data.ErrorCode, data);

        }

        [Route(nameof(GetRoleNameByCurrentUser))]
        [HttpGet]
        public async Task<ActionResult> GetRoleNameByCurrentUser()
        {
            var id = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "id").Value;
            var data = await _roleService.GetRoleNameByIdUser(id);
            return StatusCode((int)data.ErrorCode, data);

        }
    }
}
