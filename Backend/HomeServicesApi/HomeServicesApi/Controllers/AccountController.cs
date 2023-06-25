using Core.Dtos;
using Core.Services;
using DataLayer.Mapping;
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
        private readonly AuthorizationService _authorizationService;

        public AccountController(CustomersService customersService, ProvidersService providersService, AdminsService adminsService, AccountsService accountsService, AuthorizationService authorizationService)
        {
            _customersService = customersService;
            _providersService = providersService;
            _adminsService = adminsService;
            _accountsService = accountsService;
            _authorizationService = authorizationService;
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
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _authorizationService.GetUserIdFromToken(token);

            var modifiedUser = await _accountsService.UpdatePassword(userId, payload);
            if (modifiedUser == null)
            {
                return BadRequest("Invalid password"!);
            }
            return Ok(modifiedUser.ToUserDisplayDto());
        }

        [HttpPut("change-Username")]
        [Authorize(Roles = "Customer,Provider")]
        public async Task<IActionResult> ChangeUsername(ChangeUsernameDto payload)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _authorizationService.GetUserIdFromToken(token);

            var modifiedUser = await _accountsService.UpdateUsername(userId, payload);
            if (modifiedUser == null)
            {
                return BadRequest("Invalid password!");
            }
            return Ok(modifiedUser.ToUserDisplayDto());
        }

        [HttpPut("change-email")]
        [Authorize(Roles = "Customer,Provider")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailDto payload)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _authorizationService.GetUserIdFromToken(token);

            var modifiedUser = await _accountsService.UpdateEmail(userId, payload);
            if (modifiedUser == null)
            {
                return BadRequest("Invalid password!");
            }
            return Ok(modifiedUser.ToUserDisplayDto());
        }

        [HttpPut("disable/user")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DisableCustomerAccount(string password)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _authorizationService.GetUserIdFromToken(token);
            await _accountsService.DisableCustomerAccount(userId, password);
            return Ok();
        }

        [HttpPut("admin-disable/user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDisableCustomerAccount(int userId)
        {
            await _accountsService.AdminDisableCustomerAccount(userId);
            return Ok();
        }


        [HttpPut("change-phone-number")]
        [Authorize(Roles = "Customer,Provider")]
        public async Task<IActionResult> ChangePhoneNumber(ChangePhoneNumberDto payload)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _authorizationService.GetUserIdFromToken(token);

            var modifiedUser = await _accountsService.UpdatePhoneNumber(userId, payload);
            if (modifiedUser == null)
            {
                return BadRequest("Invalid password!");
            }
            return Ok(modifiedUser.ToUserDisplayDto());
        }
    }
}
