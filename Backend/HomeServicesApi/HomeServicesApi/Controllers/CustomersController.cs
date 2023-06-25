using Core.Dtos;
using Core.Services;
using DataLayer.Entities;
using DataLayer.Enums;
using DataLayer.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeServicesApi.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly CustomersService _customersService;
        private readonly AuthorizationService _authorizationService;

        public CustomersController(CustomersService customersService, AuthorizationService authorizationService)
        {
            _customersService = customersService;
            _authorizationService = authorizationService;
        }

        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var results = await _customersService.GetAll();

            return Ok(results);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto payload)
        {
            try { 
                var jwtToken = await _customersService.Validate(payload);
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
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var customer = await _customersService.GetByEmail(email);

            return Ok(customer);
        }

        [HttpPost("add-booking")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddBooking(BookingDto payload)
        {
            await _customersService.AddBooking(payload);
            return Ok();
        }

        [HttpGet("{customerId:int}/get-all-bookings")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBookings([FromRoute] int customerId)
        {
            var bookings = (await _customersService.GetAllBookings(customerId)).ToBookingDisplayDtos();
            return Ok(bookings);
        }

        [HttpGet("get-all-bookings")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAllBookings()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _authorizationService.GetUserIdFromToken(token);
            int customerId = (await _customersService.GetByUserId(userId)).Id;

            var bookings = (await _customersService.GetAllBookings(customerId)).ToBookingDisplayDtos();
            return Ok(bookings);
        }

        [HttpGet("{customerId:int}/get-bookings-by-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBookingsByStatus([FromRoute] int customerId, BookingStatus status)
        {
            var bookings = (await _customersService.GetBookingsByStatus(customerId, status)).ToBookingDisplayDtos();
            return Ok(bookings);
        }

        [HttpGet("get-bookings-by-status")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetBookingsByStatus(BookingStatus status)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _authorizationService.GetUserIdFromToken(token);
            int customerId = (await _customersService.GetByUserId(userId)).Id;

            var bookings = (await _customersService.GetBookingsByStatus(customerId, status)).ToBookingDisplayDtos();
            return Ok(bookings);
        }

        [HttpGet("{customerId:int}/get-bookings-by-date")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBookingsByDate([FromRoute] int customerId, DateTime date)
        {
            var bookings = (await _customersService.GetBookingsByDate(customerId, date)).ToBookingDisplayDtos();
            return Ok(bookings);
        }

        [HttpGet("get-bookings-by-date")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetBookingsByDate(DateTime date)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _authorizationService.GetUserIdFromToken(token);
            int customerId = (await _customersService.GetByUserId(userId)).Id;

            var bookings = (await _customersService.GetBookingsByDate(customerId, date)).ToBookingDisplayDtos();
            return Ok(bookings);
        }

        [HttpPut("{customerId:int}/update-customer-info")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCustomerInfo([FromRoute] int customerId, UpdateCustomerDto payload)
        {
            var updatedCustomer = (await _customersService.UpdateCustomer(customerId, payload)).ToCustomerDisplayDto();
            return Ok(updatedCustomer);
        }

        [HttpPut("update-customer-info")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCustomerInfo(UpdateCustomerDto payload)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            int userId = _authorizationService.GetUserIdFromToken(token);
            int customerId = (await _customersService.GetByUserId(userId)).Id;

            var updatedCustomer = (await _customersService.UpdateCustomer(customerId, payload)).ToCustomerDisplayDto();
            return Ok(updatedCustomer);
        }
    }
}
