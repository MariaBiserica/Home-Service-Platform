﻿using Core.Dtos;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServicesApi.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly AdminsService _adminsService;

        public AdminsController(AdminsService adminsService)
        {
            _adminsService = adminsService;
        }

        [HttpPost("admin-login")]
        public async Task<IActionResult> Login(LoginDto payload)
        {
            var jwtToken = await _adminsService.Validate(payload);

            return Ok(new { token = jwtToken });
        }

        [HttpPost("add-service-type")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddServiceType(string serviceTypeName)
        {
            var serviceType = await _adminsService.AddServiceType(serviceTypeName);
            return Ok(serviceType);
        }

        [HttpDelete("{adminId:int}/delete-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdmin([FromRoute]int adminId)
        {
            await _adminsService.DeleteAdmin(adminId);
            return Ok();
        }

        [HttpDelete("{serviceTypeId:int}/delete-service-type")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteServiceType([FromRoute] int serviceTypeId)
        {
            await _adminsService.DeleteServiceType(serviceTypeId);
            return Ok();
        }
    }
}
