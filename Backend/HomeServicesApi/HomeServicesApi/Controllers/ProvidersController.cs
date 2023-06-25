using Core.Dtos;
using Core.Services;
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

        public ProvidersController(ProvidersService providersService)
        {
            _providersService = providersService;
        }

        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var results = await _providersService.GetAll();

            return Ok(results);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto payload)
        {
            var jwtToken = await _providersService.Validate(payload);

            return Ok(new { token = jwtToken });
        }

        [HttpGet("get-by-email")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var customer = await _providersService.GetByEmail(email);

            return Ok(customer);
        }

        [HttpPost("add-service")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> AddService(ServiceDto payload)
        {
            await _providersService.AddService(payload);
            return Ok();
        }
    }
}
