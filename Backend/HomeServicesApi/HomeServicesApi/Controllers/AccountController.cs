using Core.Dtos;
using Core.Services;
using DataLayer.Mapping;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Register a new customer (Public)
        /// </summary>
        /// <param name="payload">All fields are required, except username</param>
        /// <returns></returns>
        /// <response code="200">Successful request, customer has been added to the database</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect or another error occurred</response>
        /// <response code="500">Unsuccesful request, an error occurred when adding the customer in the database</response>
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

        /// <summary>
        /// Register a new provider (Public)
        /// </summary>
        /// <param name="payload">All fields are required, except username</param>
        /// <returns></returns>
        /// <response code="200">Successful request, provider has been added to the database</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect or another error occurred</response>
        /// <response code="500">Unsuccesful request, an error occurred when adding the provider in the database</response>
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

        /// <summary>
        /// Register a new admin (Admin only)
        /// </summary>
        /// <param name="payload">All fields are required, except username</param>
        /// <returns></returns>
        /// <response code="200">Successful request, admin has been added to the database</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect or another error occurred</response>
        /// <response code="500">Unsuccesful request, an error occurred when adding the admin in the database</response>
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

        /// <summary>
        /// Change account password (Customer and Provider only)
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>Modified user</returns>
        /// <response code="200">Successful request, password has been changed</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect or another error occurred</response>
        /// <response code="401">Unsuccesful request, the user is not authorized to change the password</response>
        /// <response code="404">Unsuccesful request, the user was not found in the database</response>
        /// <response code="500">Unsuccesful request, an error occurred when changing the password</response>
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

        /// <summary>
        /// Change account username (Customer and Provider only)
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>Modified user</returns>
        /// <response code="200">Successful request, username has been changed</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect or another error occurred</response>
        /// <response code="401">Unsuccesful request, the user is not authorized to change the username</response>
        /// <response code="404">Unsuccesful request, the user was not found in the database</response>
        /// <response code="500">Unsuccesful request, an error occurred when changing the username</response>
        [HttpPut("change-username")]
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

        /// <summary>
        /// Change account email (Customer and Provider only)
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>Modified user</returns>
        /// <response code="200">Successful request, email has been changed</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect or another error occurred</response>
        /// <response code="401">Unsuccesful request, the user is not authorized to change the email</response>
        /// <response code="404">Unsuccesful request, the user was not found in the database</response>
        /// <response code="500">Unsuccesful request, an error occurred when changing the email</response>
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

        /// <summary>
        /// Change account phone number (Customer and Provider only)
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>Modified user</returns>
        /// <response code="200">Successful request, email has been changed</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect or another error occurred</response>
        /// <response code="401">Unsuccesful request, the user is not authorized to change the email</response>
        /// <response code="404">Unsuccesful request, the user was not found in the database</response>
        /// <response code="500">Unsuccesful request, an error occurred when changing the email</response>
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

        /// <summary>
        /// Disables the account of the customer that is currently logged in (Customer only)
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <response code="200">Successful request, user has been disabled</response>
        /// <response code="400">Unsuccesful request, the request is incorrect or another error occurred</response>
        /// <response code="401">Unsuccesful request, the user is not authorized to disable the account</response>
        /// <response code="404">Unsuccesful request, the user was not found in the database</response>
        /// <response code="500">Unsuccesful request, an error occurred when disabling the account</response>
        [HttpPut("disable/customer")]
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

        /// <summary>
        /// Disables the account of the provider that is currently logged in (Provider only)
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <response code="200">Successful request, user has been disabled</response>
        /// <response code="400">Unsuccesful request, the request is incorrect or another error occurred</response>
        /// <response code="401">Unsuccesful request, the user is not authorized to disable the account</response>
        /// <response code="404">Unsuccesful request, the user was not found in the database</response>
        /// <response code="500">Unsuccesful request, an error occurred when disabling the account</response>
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

        /// <summary>
        /// Disables the account of a specific customer (Admin only)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="200">Successful request, user has been disabled</response>
        /// <response code="400">Unsuccesful request, the request is incorrect or another error occurred</response>
        /// <response code="404">Unsuccesful request, the user was not found in the database</response>
        /// <response code="500">Unsuccesful request, an error occurred when disabling the account</response>
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

        /// <summary>
        /// Disables the account of a specific provider (Admin only)
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <response code="200">Successful request, user has been disabled</response>
        /// <response code="400">Unsuccesful request, the request is incorrect or another error occurred</response>
        /// <response code="404">Unsuccesful request, the user was not found in the database</response>
        /// <response code="500">Unsuccesful request, an error occurred when disabling the account</response>
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
    }
}
