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
        /// <summary>
        /// Log in as provider (Public)
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>JWT token</returns>
        /// <response code="200">Successful request, returns JWT token</response>
        /// <response code="400">Unsuccessful request, the request body is incorrect or another error occurred</response>
        /// <response code="401">Unsuccessful request, password is incorrect</response>
        /// <response code="404">Unsuccessful request, the user was not found in the database</response>
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

        /// <summary>
        /// Add a service (Provider only)
        /// </summary>
        /// <param name="payload">All fields are required, except description</param>
        /// <returns></returns>
        /// <response code="200">Successful request, service service has been added to the database </response>
        /// <response code="400">Bad request, the request body is incorrect or another error occurred</response>\
        /// <response code="404">Not found, the provider was not found in the database</response>
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

        /// <summary>
        /// Update provider information (Provider only)
        /// </summary>
        /// <param name="payload">All fields are optional</param>
        /// <returns></returns>
        /// <response code="200">Successful request, provider updated successfully </response>
        /// <response code="400">Bad request, the request body is incorrect or another error occurred</response>\
        /// <response code="404">Not found, the provider was not found in the database</response>
        /// <response code="500">Internal server error, an error occurred while updating the provider</response>
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

        /// <summary>
        /// Update service information (Provider only)
        /// </summary>
        /// <param name="payload">Service id is required</param>
        /// <returns></returns>
        /// <response code="200">Successful request, provider updated successfully </response>
        /// <response code="400">Bad request, the request body is incorrect or another error occurred</response>\
        /// <response code="404">Not found, the service was not found in the database</response>
        /// <response code="500">Internal server error, an error occurred while updating the service</response>
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

        /// <summary>
        /// Update price (Provider only)
        /// </summary>
        /// <param name="payload">All fields are required</param>
        /// <returns></returns>
        /// <response code="200">Successful request, provider updated successfully </response>
        /// <response code="400">Bad request, the request body is incorrect or another error occurred</response>\
        /// <response code="404">Not found, either the service or the prices dictionary key was not found in the database</response>
        /// <response code="500">Internal server error, an error occurred while updating the price</response>
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

        /// <summary>
        /// Disable service (Provider only)
        /// </summary>
        /// <param name="payload">Service id is required</param>
        /// <returns></returns>
        /// <response code="200">Successful request,service disabled successfully </response>
        /// <response code="400">Bad request, the request body is incorrect or another error occurred</response>\
        /// <response code="401">Unsuccessful request, access denied</response>
        /// <response code="404">Not found, the service was not found in the database</response>
        /// <response code="500">Internal server error, an error occurred while disabling the service</response>
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

        /// <summary>
        /// Update provider information (Admin only)
        /// </summary>
        /// <param name="providerId">Provider id is required</param>
        /// <param name="payload">All fields are optional</param>
        /// <returns></returns>
        /// <response code="200">Successful request,provider updated successfully </response>
        /// <response code="400">Bad request, the request body is incorrect or another error occurred</response>\
        /// <response code="401">Unsuccessful request, access denied</response>
        /// <response code="404">Not found, the provider was not found in the database</response>
        /// <response code="500">Internal server error, an error occurred while disabling the service</response>
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

        /// <summary>
        /// Update service information (Admin only)
        /// </summary>
        /// <param name="payload">Service id is required</param>
        /// <returns></returns>
        /// <response code="200">Successful request, provider updated successfully </response>
        /// <response code="400">Bad request, the request body is incorrect or another error occurred</response>\
        /// <response code="404">Not found, the service was not found in the database</response>
        /// <response code="500">Internal server error, an error occurred while updating the service</response>
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

        /// <summary>
        /// Disable service (Admin only)
        /// </summary>
        /// <param name="payload">Service id is required</param>
        /// <returns></returns>
        /// <response code="200">Successful request,service disabled successfully </response>
        /// <response code="400">Bad request, the request body is incorrect or another error occurred</response>\
        /// <response code="401">Unsuccessful request, access denied</response>
        /// <response code="404">Not found, the service was not found in the database</response>
        /// <response code="500">Internal server error, an error occurred while disabling the service</response>
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

        /// <summary>
        /// Update price (Provider only)
        /// </summary>
        /// <param name="payload">All fields are required</param>
        /// <returns></returns>
        /// <response code="200">Successful request, provider updated successfully </response>
        /// <response code="400">Bad request, the request body is incorrect or another error occurred</response>\
        /// <response code="404">Not found, either the service or the prices dictionary key was not found in the database</response>
        /// <response code="500">Internal server error, an error occurred while updating the price</response>
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

        /// <summary>
        /// Get all providers (Admin only)
        /// </summary>
        /// <returns>List of providers or empty list</returns>
        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var results = (await _providersService.GetAll()).ToProviderDisplayDtos();
            return Ok(results);
        }

        /// <summary>
        /// Get all services (Any logged in user)
        /// </summary>
        /// <returns>List of services or empty list</returns>
        [HttpGet("get-all-services")]
        [Authorize(Roles = "Customer,Provider,Admin")]
        public async Task<IActionResult> GetAllServices()
        {
            var results = (await _providersService.GetAllServices()).ToServiceDisplayDtos();
            return Ok(results);
        }

        /// <summary>
        /// Get all services of a provider (Any logged in user)
        /// </summary>
        /// <returns>List of services or empty list</returns>
        [HttpGet("get-all-provider-services")]
        [Authorize(Roles = "Customer,Provider,Admin")]
        public async Task<IActionResult> GetAllProviderServices(int providerId)
        {
            var results = (await _providersService.GetAllProviderServices(providerId)).ToServiceDisplayDtos();
            return Ok(results);
        }

        /// <summary>
        /// Get service bt type (Any logged in user)
        /// </summary>
        /// <param name="providerId"></param>
        /// <param name="serviceTypeId"></param>
        /// <returns>List of services</returns>
        /// <response code="200">Successful request</response>
        /// <response code="400">Unsuccessful request, the request body is incorrect or another error occurred</response>
        /// <response code="404">Unsuccessful request, the service was not found in the database or there are no matching services</response>
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

        /// <summary>
        /// Get any provider's info (Provider and admin only)
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Provider information</returns>
        /// <response code="200">Successful request</response>
        /// <response code="400">Unsuccessful request, the request body is incorrect or another error occurred</response>
        /// <response code="404">Unsuccessful request, the user was not found in the database</response>
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

        /// <summary>
        /// Get all services types (Any logged in user)
        /// </summary>
        /// <returns>List of services types or empty list</returns>
        [HttpGet("get-all-service-types")]
        [Authorize(Roles = "Admin,Provider,Customer")]
        public async Task<IActionResult> GetAllServiceTypes()
        {
            var serviceTypes = await _providersService.GetServiceTypes();
            return Ok(serviceTypes);
        }
    }
}
