using Core.Dtos;
using Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServicesApi.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly CustomersService _customersService;
        private readonly ProvidersService _providersService;
        private readonly AdminsService _adminsService;

        public AccountController(CustomersService customersService, ProvidersService providersService, AdminsService adminsService)
        {
            _customersService = customersService;
            _providersService = providersService;
            _adminsService = adminsService;
        }

        [HttpPost("register/customer")]
        [AllowAnonymous]
        public IActionResult RegisterCustomer(CustomerRegisterDto payload)
        {
            _customersService.Register(payload);
            return Ok();
        }

        [HttpPost("register/provider")]
        [AllowAnonymous]
        public IActionResult RegisterProvider(ProviderRegisterDto payload)
        {
            _providersService.Register(payload);
            return Ok();
        }

        [HttpPost("register/admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult RegisterAdmin(UserRegisterDto payload)
        {
            _adminsService.Register(payload);
            return Ok();
        }

    }
}
