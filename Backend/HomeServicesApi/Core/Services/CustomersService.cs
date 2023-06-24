﻿using Core.Dtos;
using DataLayer.Entities;
using DataLayer.Repositories;
using DataLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class CustomersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AuthenticationService _authService;
        private readonly AuthorizationService _authorizationService;

        public CustomersService(IUnitOfWork unitOfWork, AuthenticationService authService, AuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _authorizationService = authorizationService;
        }

        public async Task<List<Customer>> GetAll()
        {
            var customers = await _unitOfWork.GetRepository<CustomersRepository, Customer>().GetAllAsync();
            return customers;
        }

        public async Task<Customer> GetByEmail(string email)
        {
            var customer = await _unitOfWork.GetRepository<CustomersRepository, Customer>().GetByEmailAsync(email);
            return customer;
        }

        public async void Register(CustomerRegisterDto registerData)
        {
            if (registerData == null)
            {
                return;
            }

            var hashedPassword = _authService.HashPassword(registerData.UserData.Password);
            var newUser = new User
            {
                Email = registerData.UserData.Email,
                PasswordHash = hashedPassword
            };

            var user = await _unitOfWork.GetRepository<UsersRepository, User>().AddAsync(newUser);
            if (user == null) 
            {
                return;
            }

            var newCustomer = new Customer
            {
                FirstName = registerData.FirstName,
                LastName = registerData.LastName,
                User = user
            };

            var customer = await _unitOfWork.GetRepository<CustomersRepository, Customer>().AddAsync(newCustomer);
            if (customer == null)
            {
                return;
            }

            _unitOfWork.Commit();
        }

        public async Task<string> Validate(LoginDto payload)
        {
            var customer = await _unitOfWork.GetRepository<CustomersRepository, Customer>().GetByEmailAsync(payload.Email) ?? throw new ApplicationException("User not found");
            
            var passwordFine = _authService.VerifyHashedPassword(customer.User.PasswordHash, payload.Password);
            if (passwordFine)
            {
                return _authorizationService.GetToken(customer.User, "Customer");
            }
            else
            {
                throw new ApplicationException("Incorrect password");
            }

        }
    }
}
