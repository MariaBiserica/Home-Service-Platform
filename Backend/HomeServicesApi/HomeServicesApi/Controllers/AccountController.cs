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
        private readonly AccountsService _accountsService;

        public AccountController(CustomersService customersService, ProvidersService providersService, AdminsService adminsService, AccountsService accountsService)
        {
            _customersService = customersService;
            _providersService = providersService;
            _adminsService = adminsService;
            _accountsService = accountsService;
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

        [HttpPut("change-password")]
        [Authorize(Roles = "Customer,Provider")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto payload)
        {
            var modifiedUser = await _accountsService.UpdatePassword(payload);
            if (modifiedUser == null)
            {
                return BadRequest("Invalid password"!);
            }
            return Ok(modifiedUser);
        }

        [HttpPut("change-Username")]
        [Authorize(Roles = "Customer,Provider")]
        public async Task<IActionResult> ChangeUsername(ChangeUsernameDto payload)
        {
            var modifiedUser = await _accountsService.UpdateUsername(payload);
            if (modifiedUser == null)
            {
                return BadRequest("Invalid password!");
            }
            return Ok(modifiedUser);
        }

        [HttpPut("change-email")]
        [Authorize(Roles = "Customer,Provider")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailDto payload)
        {
            var modifiedUser = await _accountsService.UpdateEmail(payload);
            if (modifiedUser == null)
            {
                return BadRequest("Invalid password!");
            }
            return Ok(modifiedUser);
        }

        [HttpPut("change-phone-number")]
        [Authorize(Roles = "Customer,Provider")]
        public async Task<IActionResult> ChangePhoneNumber(ChangePhoneNumberDto payload)
        {
            var modifiedUser = await _accountsService.UpdatePhoneNumber(payload);
            if (modifiedUser == null)
            {
                return BadRequest("Invalid password!");
            }
            return Ok(modifiedUser);
        }


    }
}
