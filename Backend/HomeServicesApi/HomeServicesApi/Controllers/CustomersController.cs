﻿using Core.Dtos;
using Core.Services;
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
    }
}
