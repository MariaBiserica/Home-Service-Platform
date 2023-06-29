using Core.Dtos;
using Core.Services;
using DataLayer.Enums;
using DataLayer.Mapping;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        /// <summary>
        /// Customer login (Public)
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>JWT token</returns>
        /// <response code="200">Succesful request, returns JWT token</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect or another error occurred</response>
        /// <response code="401">Unsuccesful request, password is incorrect</response>
        /// <response code="404">Unsuccesful request, the user was not found in the database</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto payload)
        {
            try
            {
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

        /// <summary>
        /// Add a new booking for logged in customer (Customer only)
        /// </summary>
        /// <param name="payload">ServiceId is required, PaymentId is optional - if not given a new payment will be created with IsProcessed=false</param>
        /// <returns>Booking details</returns>
        /// <response code="200">Succesful request</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect, there is a token error or another error occurred</response>
        /// <response code="404">Unsuccesful request, the user or the service were not found in the database</response>
        /// <response code="500">Unsuccesful request, an error occurred when adding the booking or payment in the database</response>
        [HttpPost("add-booking")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddBooking(BookingDto payload)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);
                int customerId = (await _customersService.GetByUserId(userId)).Id;

                await _customersService.AddBooking(customerId, payload);
                return Ok();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
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
        /// Update customer info (Customer only)
        /// </summary>
        /// <param name="payload">All fields are optional</param>
        /// <returns>The customer with the updated information</returns>
        /// <response code="200">Succesful request</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect, there is a token error or another error occurred</response>
        /// <response code="404">Unsuccesful request, the user was not found in the database</response>
        /// <responde code="500">Unsuccesful request, an error occurred when updating the customer in the database</responde>
        [HttpPut("update-customer-info")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCustomerInfo(UpdateCustomerDto payload)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);
                int customerId = (await _customersService.GetByUserId(userId)).Id;

                var updatedCustomer =
                    (await _customersService.UpdateCustomer(customerId, payload)).ToCustomerDisplayDto();
                return Ok(updatedCustomer);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
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
        /// Update any customer's info (Admin only)
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="payload">All fields are optional</param>
        /// <returns>The customer with the updated information</returns>
        /// <response code="200">Succesful request</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect, there is a token error or another error occurred</response>
        /// <response code="404">Unsuccesful request, the user was not found in the database</response>
        /// <responde code="500">Unsuccesful request, an error occurred when updating the customer in the database</responde>
        [HttpPut("{customerId:int}/update-customer-info")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCustomerInfo([FromRoute] int customerId, UpdateCustomerDto payload)
        {
            try
            {
                var updatedCustomer =
                    (await _customersService.UpdateCustomer(customerId, payload)).ToCustomerDisplayDto();
                return Ok(updatedCustomer);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (DbUpdateException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
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
        /// Get any customer's info (Customer and Admin only)
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Customer information</returns>
        /// <response code="200">Succesful request</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect or another error occurred</response>
        /// <response code="404">Unsuccesful request, the user was not found in the database</response>
        [HttpGet("get-by-email")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            try
            {
                var customer = await _customersService.GetByEmail(email);
                return Ok(customer.ToCustomerDisplayDto());
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
        /// Get all bookings for logged in customer (Customer only)
        /// </summary>
        /// <returns>List of bookings</returns>
        /// <response code="200">Succesful request</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect, there is a token error or another error occurred</response>
        [HttpGet("get-all-bookings")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);
                int customerId = (await _customersService.GetByUserId(userId)).Id;

                var bookings = (await _customersService.GetAllBookings(customerId)).ToBookingDisplayDtos();
                return Ok(bookings);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get all bookings that have the given status for logged in customer (Customer only)
        /// </summary>
        /// <param name="status"></param>
        /// <returns>List of bookings</returns>
        /// <response code="200">Succesful request</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect, there is a token error or another error occurred</response>
        [HttpGet("get-bookings-by-status")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetBookingsByStatus(BookingStatus status)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);
                int customerId = (await _customersService.GetByUserId(userId)).Id;

                var bookings = (await _customersService.GetBookingsByStatus(customerId, status)).ToBookingDisplayDtos();
                return Ok(bookings);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get all bookings from the selected date for logged in customer (Customer only)
        /// </summary>
        /// <param name="date"></param>
        /// <returns>List of bookings</returns>
        /// <response code="200">Succesful request</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect, there is a token error or another error occurred</response>
        [HttpGet("get-bookings-by-date")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetBookingsByDate(DateTime date)
        {
            try
            {
                string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                int userId = _authorizationService.GetUserIdFromToken(token);
                int customerId = (await _customersService.GetByUserId(userId)).Id;

                var bookings = (await _customersService.GetBookingsByDate(customerId, date)).ToBookingDisplayDtos();
                return Ok(bookings);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        /// <summary>
        /// Get all bookings for any customer (Admin only)
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns>List of bookings</returns>
        /// <response code="200">Succesful request</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect, there is a token error or another error occurred</response>
        [HttpGet("{customerId:int}/get-all-bookings")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBookings([FromRoute] int customerId)
        {
            var bookings = (await _customersService.GetAllBookings(customerId)).ToBookingDisplayDtos();
            return Ok(bookings);
        }

        /// <summary>
        /// Get all bookings that have the given status for any customer (Admin only)
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="status"></param>
        /// <returns>List of bookings</returns>
        /// <response code="200">Succesful request</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect, there is a token error or another error occurred</response>
        [HttpGet("{customerId:int}/get-bookings-by-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBookingsByStatus([FromRoute] int customerId, BookingStatus status)
        {
            var bookings = (await _customersService.GetBookingsByStatus(customerId, status)).ToBookingDisplayDtos();
            return Ok(bookings);
        }

        /// <summary>
        /// Get all bookings from the selected date for any customer (Admin only)
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="date"></param>
        /// <returns>List of bookings</returns>
        /// <response code="200">Succesful request</response>
        /// <response code="400">Unsuccesful request, the request body is incorrect, there is a token error or another error occurred</response>
        [HttpGet("{customerId:int}/get-bookings-by-date")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetBookingsByDate([FromRoute] int customerId, DateTime date)
        {
            var bookings = (await _customersService.GetBookingsByDate(customerId, date)).ToBookingDisplayDtos();
            return Ok(bookings);
        }

        /// <summary>
        /// Get all customers (Admin only)
        /// </summary>
        /// <returns>List of customers</returns>
        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var results = await _customersService.GetAll();

            return Ok(results.ToCustomerDisplayDtos());
        }
    }
}