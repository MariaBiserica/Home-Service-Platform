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

        /// <summary>
        /// Log in as admin (Public)
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>JWT token</returns>
        /// <response code="200">Successful request, returns JWT token</response>
        /// <response code="400">Unsuccessful request, the request body is incorrect or another error occurred</response>
        /// <response code="401">Unsuccessful request, password is incorrect</response>
        /// <response code="404">Unsuccessful request, the user was not found in the database</response>
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

        /// <summary>
        /// Add a service type (Admin only)
        /// </summary>
        /// <param name="serviceTypeName">Service type name is required</param>
        /// <returns></returns>
        /// <response code="200">Successful request, service type added successfully</response>
        /// <response code="400">Bad request, the request body is incorrect or another error occurred</response>
        /// <responese code="500">Internal server error, service type already exists</responese>
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

        /// <summary>
        /// Delete admin (Admin only)
        /// </summary>
        /// <param name="adminId">Admin id is required</param>
        /// <returns></returns>
        /// <response code="200">Successful request, service type deleted successfully</response>
        /// <response code="400">Bad request, the request body is incorrect or another error occurred</response>
        /// <response code="404">Not found, admin not found in the database</response>
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


        /// <summary>
        /// Delete service type (Admin only)
        /// </summary>
        /// <param name="serviceTypeId"></param>
        /// <returns></returns>
        /// <response code="200">Successful request, service type deleted successfully</response>
        /// <response code="400">Bad request, the request body is incorrect or another error occurred</response>
        /// <response code="404">Not found, admin not found in the database</response>
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
