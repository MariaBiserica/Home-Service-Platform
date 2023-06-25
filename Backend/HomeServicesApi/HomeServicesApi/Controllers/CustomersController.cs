using Core.Dtos;
using Core.Services;
using DataLayer.Entities;
using DataLayer.Enums;
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

        public CustomersController(CustomersService customersService)
        {
            _customersService = customersService;
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
            var jwtToken = await _customersService.Validate(payload);

            return Ok(new { token = jwtToken });
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
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAllBookings([FromRoute] int customerId)
        {
            var bookings = await _customersService.GetAllBookings(customerId);
            return Ok(bookings);
        }

        [HttpGet("{customerId:int}/get-bookings-by-status")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetBookingsByStatus([FromRoute] int customerId, BookingStatus status)
        {
            var bookings = await _customersService.GetBookingsByStatus(customerId, status);
            return Ok(bookings);
        }

        [HttpGet("{customerId:int}/get-bookings-by-date")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetBookingsByDate([FromRoute] int customerId, DateTime date)
        {
            var bookings = await _customersService.GetBookingsByDate(customerId, date);
            return Ok(bookings);
        }


    }
}
