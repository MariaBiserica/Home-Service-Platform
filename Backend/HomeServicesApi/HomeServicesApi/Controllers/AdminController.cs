using Core.Dtos;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
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
            try
            {
                var jwtToken = await _adminsService.Validate(payload);
                return Ok(new { token = jwtToken });
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("add-service-type")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddServiceType(string serviceTypeName)
        {
            try
            {
                var serviceType = await _adminsService.AddServiceType(serviceTypeName);
                return Ok(serviceType);
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{adminId:int}/delete-admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdmin([FromRoute] int adminId)
        {
            try
            {
                await _adminsService.DeleteAdmin(adminId);
                return Ok();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{serviceTypeId:int}/delete-service-type")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteServiceType([FromRoute] int serviceTypeId)
        {
            try
            {
                await _adminsService.DeleteServiceType(serviceTypeId);
                return Ok();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
