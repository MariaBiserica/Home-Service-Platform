using Core.Dtos;
using Core.Services;
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
    }
}
