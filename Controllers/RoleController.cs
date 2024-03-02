﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicWebAppBackend.Infrastructure.ViewModels.Role;
using MusicWebAppBackend.Infrastructure.ViewModels.User;
using MusicWebAppBackend.Services;

namespace MusicWebAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

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
    }
}