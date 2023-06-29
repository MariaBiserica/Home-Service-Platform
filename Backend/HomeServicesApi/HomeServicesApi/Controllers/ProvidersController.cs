using Core.Dtos;
using Core.Services;
using DataLayer.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        [HttpPost("add-service")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> AddService(ServiceDto payload)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);
                int providerId = (await _providersService.GetByUserId(userId)).Id;
                await _providersService.AddService(providerId, payload);
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

        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var results = (await _providersService.GetAll()).ToProviderDisplayDtos();
            return Ok(results);
        }

        [HttpGet("get-all-services")]
        [Authorize(Roles = "Customer,Provider,Admin")]
        public async Task<IActionResult> GetAllServices()
        {
            var results = (await _providersService.GetAllServices()).ToServiceDisplayDtos();
            return Ok(results);
        }

        [HttpGet("get-all-provider-services")]
        [Authorize(Roles = "Customer,Provider,Admin")]
        public async Task<IActionResult> GetAllProviderServices(int providerId)
        {
            var results = (await _providersService.GetAllProviderServices(providerId)).ToServiceDisplayDtos();
            return Ok(results);
        }

        [HttpGet("get-services-by-type")]
        [Authorize(Roles = "Customer,Provider,Admin")]
        public async Task<IActionResult> GetServicesByType(int providerId, int serviceTypeId)
        {
            try
            {
                var results = (await _providersService.GetServicesByType(providerId, serviceTypeId)).ToServiceDisplayDtos();
                return Ok(results);
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
            try
            {
                var provider = (await _providersService.GetByEmail(email)).ToProviderDisplayDto();
                return Ok(provider);
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

        [HttpGet("get-all-service-types")]
        [Authorize(Roles = "Admin,Provider,Customer")]
        public async Task<IActionResult> GetAllServiceTypes()
        {
            var serviceTypes = await _providersService.GetServiceTypes();
            return Ok(serviceTypes);
        }

        [HttpPut("update-provider")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> UpdateProvider(UpdateProviderDto payload)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);
                int providerId = (await _providersService.GetByUserId(userId)).Id;

                var updatedProvider = (await _providersService.UpdateProvider(providerId, payload)).ToProviderDisplayDto();
                return Ok(updatedProvider);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("update-service")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> UpdateService(UpdateServiceDto payload)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);
                int providerId = (await _providersService.GetByUserId(userId)).Id;

                var service = await _providersService.GetServiceById(payload.ServiceId);
                if (service.ProviderId != providerId)
                {
                    return Unauthorized();
                }
                var updatedService = (await _providersService.UpdateService(payload)).ToServiceDisplayDto();
                return Ok(updatedService);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("update-price")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> UpdatePrice(UpdatePriceDto payload)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);
                int providerId = (await _providersService.GetByUserId(userId)).Id;

                var service = await _providersService.GetServiceById(payload.ServiceId);
                if (service.ProviderId != providerId)
                {
                    return Unauthorized();
                }
                var updatedService = (await _providersService.UpdatePrice(payload)).ToServiceDisplayDto();
                return Ok(updatedService);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPut("disable/service")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> DisableService(int serviceId)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);
                int providerId = (await _providersService.GetByUserId(userId)).Id;

                var service = await _providersService.GetServiceById(serviceId);
                if (service.ProviderId != providerId)
                {
                    return Unauthorized();
                }
                var updatedService = (await _providersService.DisableService(serviceId)).ToServiceDisplayDto();
                return Ok(updatedService);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPut("{providerId:int}/update-provider")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProvider([FromRoute] int providerId, UpdateProviderDto payload)
        {
            try
            {
                var updatedProvider = (await _providersService.UpdateProvider(providerId, payload)).ToProviderDisplayDto();
                return Ok(updatedProvider);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("admin-update-service")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminUpdateService(UpdateServiceDto payload)
        {
            try
            {
                payload.ServiceId = payload.ServiceId;
                var service = await _providersService.GetServiceById(payload.ServiceId);
                var updatedService = (await _providersService.UpdateService(payload)).ToServiceDisplayDto();
                return Ok(updatedService);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPut("admin-disable/service")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDisableService(int serviceId)
        {
            try
            {
                var service = await _providersService.GetServiceById(serviceId);
                var updatedService = (await _providersService.DisableService(serviceId)).ToServiceDisplayDto();
                return Ok(updatedService);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("admin-update-price")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminUpdatePrice(UpdatePriceDto payload)
        {
            try
            {
                var service = await _providersService.GetServiceById(payload.ServiceId);
                var updatedService = (await _providersService.UpdatePrice(payload)).ToServiceDisplayDto();
                return Ok(updatedService);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }
    }
}
