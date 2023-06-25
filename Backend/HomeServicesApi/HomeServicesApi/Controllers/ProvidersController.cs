using Core.Dtos;
using Core.Services;
using DataLayer.Entities;
using DataLayer.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServicesApi.Controllers
{
    [Route("api/providers")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly ProvidersService _providersService;
        private readonly AuthorizationService _authorizationService;

        public ProvidersController(ProvidersService providersService, AuthorizationService authorizationService)
        {
            _providersService = providersService;
            _authorizationService = authorizationService;
        }

        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var results = (await _providersService.GetAll()).ToProviderDisplayDtos();

            return Ok(results);
        }

        [HttpGet("get-all-services")]
        [Authorize(Roles = "Customer,Provider,Admin")]
        public async Task<IActionResult> GetAllServices(int providerId)
        {
            var results = (await _providersService.GetAllServices(providerId)).ToServiceDisplayDtos();
            return Ok(results);
        }

        [HttpGet("get-services-by-type")]
        [Authorize(Roles = "Customer,Provider,Admin")]
        public async Task<IActionResult> GetServicesByType(int providerId, int serviceTypeId)
        {
            var results = (await _providersService.GetServicesByType(providerId, serviceTypeId)).ToServiceDisplayDtos();
            return Ok(results);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto payload)
        {
            try
            {
                var jwtToken = await _providersService.Validate(payload);
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

        [HttpGet("get-by-email")]
        [Authorize(Roles = "Provider,Admin")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var provider = (await _providersService.GetByEmail(email)).ToProviderDisplayDto();

            return Ok(provider);
        }

        [HttpPost("add-service")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> AddService(ServiceDto payload)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _authorizationService.GetUserIdFromToken(token);
            int providerId = (await _providersService.GetByUserId(userId)).Id;

            await _providersService.AddService(providerId, payload);
            return Ok();
        }

        [HttpPut("update-provider")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> UpdateProvider(UpdateProviderDto payload)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _authorizationService.GetUserIdFromToken(token);
            int providerId = (await _providersService.GetByUserId(userId)).Id;

            var updatedProvider = (await _providersService.UpdateProvider(providerId, payload)).ToProviderDisplayDto();
            return Ok(updatedProvider);
        }

        [HttpPut("update-service")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> UpdateService(UpdateServiceDto payload)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _authorizationService.GetUserIdFromToken(token);
            int providerId = (await _providersService.GetByUserId(userId)).Id;

            Service service;
            try
            {
                service = await _providersService.GetServiceById(payload.ServiceId);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            if (service.ProviderId != providerId)
            {
                return Unauthorized();
            }

            var updatedService = (await _providersService.UpdateService(payload)).ToServiceDisplayDto();
            return Ok(updatedService);
        }
    }
}
