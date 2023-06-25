using Core.Dtos;
using DataLayer.Entities;
using DataLayer.Repositories;
using DataLayer.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
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
            var customer = await _unitOfWork.GetRepository<CustomersRepository, Customer>().GetByEmailAsync(email) ?? throw new KeyNotFoundException("User not found");
            return customer;
        }

        public async Task<Customer> GetByUserId(int userId)
        {
            var customer = await _unitOfWork.GetRepository<CustomersRepository, Customer>().GetByUserIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
            return customer;
        }
        
        public async void Register(CustomerRegisterDto registerData)
        {
            if (registerData == null) throw new ArgumentException("Invalid data");
            
            var hashedPassword = _authService.HashPassword(registerData.UserData.Password);
            var newUser = new User
            {
                Email = registerData.UserData.Email,
                PasswordHash = hashedPassword
            };

            var user = await _unitOfWork.GetRepository<UsersRepository, User>().AddAsync(newUser) ?? throw new InvalidOperationException("User not created");
            var newCustomer = new Customer
            {
                FirstName = registerData.FirstName,
                LastName = registerData.LastName,
                User = user
            };

            _ = await _unitOfWork.GetRepository<CustomersRepository, Customer>().AddAsync(newCustomer) ?? throw new InvalidOperationException("Customer not created");
            _unitOfWork.Commit();
        }

        public async Task<string> Validate(LoginDto payload)
        {
            var customer = await _unitOfWork.GetRepository<CustomersRepository, Customer>().GetByEmailAsync(payload.Email) ?? throw new KeyNotFoundException("User not found");

            //verify if account is disabled
            if (customer.User.IsDisabled)
            {
                throw new UnauthorizedAccessException("Account is disabled");
            }

            var passwordFine = _authService.VerifyHashedPassword(customer.User.PasswordHash, payload.Password);
            if (passwordFine)
            {
                return _authorizationService.GetToken(customer.User, "Customer");
            }
            else
            {
                throw new UnauthorizedAccessException("Incorrect password");
            }

        }

        public async Task AddBooking(BookingDto payload)
        {
            var customer = await _unitOfWork.GetRepository<CustomersRepository, Customer>().GetByIdAsync(payload.CustomerId) ?? throw new KeyNotFoundException("Customer not found");
            var service = await _unitOfWork.GetRepository<ServicesRepository, Service>().GetByIdAsync(payload.ServiceId) ?? throw new KeyNotFoundException("Service not found");

            var payment = new Payment();

            if (payload.PaymentId == null)
            {
                payment.IsProcessed = false;
                _ = await _unitOfWork.GetRepository<PaymentRepository, Payment>().AddAsync(payment) ?? throw new InvalidOperationException("Payment not created");

                _unitOfWork.Commit();
            }
            else
            {
                payment = await _unitOfWork.GetRepository<PaymentRepository, Payment>().GetByIdAsync((int)payload.PaymentId) ?? throw new KeyNotFoundException("Payment not found");
            }

            var booking = new Booking()
            {
                Customer = customer,
                Service = service,
                Payment = payment,
                Date = DateTime.Now,
                Status = DataLayer.Enums.BookingStatus.Pending
            };

            _ = await _unitOfWork.GetRepository<BookingsRepository, Booking>().AddAsync(booking) ?? throw new InvalidOperationException("Booking not created");

            _unitOfWork.Commit();
        }

        public async Task<List<Booking>> GetAllBookings(int customerId)
        {
            var bookings = await _unitOfWork.GetRepository<CustomersRepository, Customer>().GetBookingsAsync(customerId);
            return bookings;
        }

        public async Task<List<Booking>> GetBookingsByStatus(int customerId, DataLayer.Enums.BookingStatus status)
        {
            var bookings = await _unitOfWork.GetRepository<CustomersRepository, Customer>().GetBookingsByStatusAsync(customerId, status);
            return bookings;
        }

        public async Task<List<Booking>> GetBookingsByDate(int customerId, DateTime date)
        {
            var bookings = await _unitOfWork.GetRepository<CustomersRepository, Customer>().GetBookingsByDateAsync(customerId, date);
            return bookings;
        }

        public async Task<Customer> UpdateCustomer(int customerId, UpdateCustomerDto payload)
        {
            var customer = await _unitOfWork.GetRepository<CustomersRepository, Customer>().GetByIdAsync(customerId) ?? throw new KeyNotFoundException("Customer not found");

            if (payload.Address != null)
            {
                if (customer.Address == null)
                {
                    var address = new Address()
                    {
                        State = payload.Address.State,
                        City = payload.Address.City,
                        Street = payload.Address.Street,
                        StreetNumber = payload.Address.StreetNumber,
                        PostalCode = payload.Address.PostalCode
                    };

                    var addedAddress = await _unitOfWork.GetRepository<AddressRepository, Address>().AddAsync(address) ?? throw new InvalidOperationException("Address not created");
                    customer.Address = addedAddress;

                    _unitOfWork.Commit();
                }
                else
                {
                    var address = customer.Address;

                    address.State = payload.Address.State;
                    address.City = payload.Address.City;
                    address.Street = payload.Address.Street;
                    address.StreetNumber = payload.Address.StreetNumber;
                    address.PostalCode = payload.Address.PostalCode;

                    var updatedAddress = await _unitOfWork.GetRepository<AddressRepository, Address>().UpdateAsync(address) ?? throw new DbUpdateException("Address not updated");
                    customer.Address = updatedAddress;
                    _unitOfWork.Commit();
                }
            }

            var commonProperties = customer.GetType().GetProperties().Where(x => payload.GetType().GetProperty(x.Name) != null && x.Name != "Address");
            foreach (var property in commonProperties)
            {
                var value = payload.GetType().GetProperty(property.Name).GetValue(payload);
                if (value != null)
                {
                    property.SetValue(customer, value);
                }
            }

            var updatedCustomer = await _unitOfWork.GetRepository<CustomersRepository, Customer>().UpdateAsync(customer) ?? throw new DbUpdateException("Customer not updated");
            _unitOfWork.Commit();
            return updatedCustomer;
        }

    }
}
