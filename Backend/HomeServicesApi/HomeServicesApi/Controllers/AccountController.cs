using Core.Dtos;
using Core.Services;
using DataLayer.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> RegisterCustomer(CustomerRegisterDto payload)
        {
            try
            {
                await _customersService.Register(payload);
                return Ok();
            }
            catch (InvalidOperationException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);

            }
        }

        [HttpPost("register/provider")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterProvider(ProviderRegisterDto payload)
        {
            try
            {
                await _providersService.Register(payload);
                return Ok();
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

        [HttpPost("register/admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RegisterAdmin(UserRegisterDto payload)
        {
            try
            {
                await _adminsService.Register(payload);
                return Ok();
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

        [HttpPut("change-password")]
        [Authorize(Roles = "Customer,Provider")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto payload)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);

                var modifiedUser = await _accountsService.UpdatePassword(userId, payload);
                return Ok(modifiedUser.ToUserDisplayDto());
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("change-Username")]
        [Authorize(Roles = "Customer,Provider")]
        public async Task<IActionResult> ChangeUsername(ChangeUsernameDto payload)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);

                var modifiedUser = await _accountsService.UpdateUsername(userId, payload);
                return Ok(modifiedUser.ToUserDisplayDto());
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("change-email")]
        [Authorize(Roles = "Customer,Provider")]
        public async Task<IActionResult> ChangeEmail(ChangeEmailDto payload)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);

                var modifiedUser = await _accountsService.UpdateEmail(userId, payload);
                return Ok(modifiedUser.ToUserDisplayDto());
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("disable/user")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> DisableCustomerAccount(string password)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);
                await _accountsService.DisableCustomerAccount(userId, password);
                return Ok();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("admin-disable/user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDisableCustomerAccount(int userId)
        {
            try
            {
                await _accountsService.AdminDisableCustomerAccount(userId);
                return Ok();
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

        [HttpPut("disable/provider")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> DisableProviderAccount(string password)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);

                await _accountsService.DisableProviderAccount(userId, password);
                int providerId = (await _providersService.GetByUserId(userId)).Id;
                await _providersService.DisableProviderServices(providerId);
                return Ok();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("admin-disable/provider")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminDisableProviderAccount(int userId)
        {
            try
            {
                await _accountsService.AdminDisableProviderAccount(userId);
                int providerId = (await _providersService.GetByUserId(userId)).Id;

                await _providersService.DisableProviderServices(providerId);
                return Ok();
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

        [HttpPut("change-phone-number")]
        [Authorize(Roles = "Customer,Provider")]
        public async Task<IActionResult> ChangePhoneNumber(ChangePhoneNumberDto payload)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);

                var modifiedUser = await _accountsService.UpdatePhoneNumber(userId, payload);
                return Ok(modifiedUser.ToUserDisplayDto());
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
