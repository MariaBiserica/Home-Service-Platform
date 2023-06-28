using Core.Dtos;
using DataLayer.Entities;
using DataLayer.Repositories;
using DataLayer.UnitOfWork;
using DataLayer.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class ProvidersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AuthenticationService _authService;
        private readonly AuthorizationService _authorizationService;

        public ProvidersService(IUnitOfWork unitOfWork, AuthenticationService authService, AuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _authorizationService = authorizationService;
        }

        public async Task<List<Provider>> GetAll()
        {
            var providers = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetAllAsync();
            return providers;
        }

        public async Task<List<Service>> GetAllServices(int providerId)
        {

            var services = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetServices(providerId);
            return services;
        }

        public async Task<List<Service>> GetServicesByType(int providerId, int typeId)
        {
            var serviceType = await _unitOfWork.GetRepository<ServiceTypesRepository, ServiceType>().GetByIdAsync(typeId) ?? throw new KeyNotFoundException("Service type not found");
            var services = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetServicesByType(providerId, serviceType);
            return services;
        }

        public async Task<Service> GetServiceById(int serviceId)
        {
            var service = await _unitOfWork.GetRepository<ServicesRepository, Service>().GetByIdAsync(serviceId) ?? throw new KeyNotFoundException("Service not found");
            return service;
        }

        public async Task<Provider> GetByEmail(string email)
        {
            var provider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetByEmailAsync(email) ?? throw new KeyNotFoundException("User not found");
            return provider;
        }

        public async void Register(ProviderRegisterDto registerData)
        {
            if (registerData == null) throw new ArgumentException("Invalid data");

            var hashedPassword = _authService.HashPassword(registerData.UserData.Password);
            var newUser = new User
            {
                Username = registerData.UserData.Username,
                Email = registerData.UserData.Email,
                PasswordHash = hashedPassword
            };

            var user = await _unitOfWork.GetRepository<UsersRepository, User>().AddAsync(newUser) ?? throw new InvalidOperationException("User not created");

            var newProvider = new Provider
            {
                Bio = registerData.Bio,
                User = user
            };

            _ = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().AddAsync(newProvider) ?? throw new InvalidOperationException("Provider not created");
            _unitOfWork.Commit();
        }

        public async Task<string> Validate(LoginDto payload)
        {
            var provider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetByEmailAsync(payload.Email) ?? throw new KeyNotFoundException("User not found");

            if (provider.User.IsDisabled)
            {
                throw new UnauthorizedAccessException("Account is disabled");
            }

            var passwordFine = _authService.VerifyHashedPassword(provider.User.PasswordHash, payload.Password);
            if (passwordFine)
            {
                return _authorizationService.GetToken(provider.User, "Provider");
            }
            else
            {
                throw new UnauthorizedAccessException("Incorrect password");
            }

        }

        public async Task AddService(int providerId, ServiceDto payload)
        {
            var provider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetByIdAsync(providerId) ?? throw new KeyNotFoundException("Provider not found");
            var type = await _unitOfWork.GetRepository<ServiceTypesRepository, ServiceType>().GetByIdAsync(payload.TypeId) ?? throw new KeyNotFoundException("Service type not found");

            var service = new Service()
            {
                Title = payload.Title,
                Type = type,
                Description = payload.Description,
                Prices = payload.Prices,
                Provider = provider,
            };

            await _unitOfWork.GetRepository<ServicesRepository, Service>().AddAsync(service);
            _unitOfWork.Commit();
        }

        public async Task<Provider> UpdateProvider(int providerId, UpdateProviderDto payload)
        {
            var provider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetByIdAsync(providerId) ?? throw new KeyNotFoundException("Provider not found");
            if (payload.Address != null)
            {
                if (provider.Address == null)
                {
                    var address = new Address()
                    {
                        State = payload.Address.State,
                        City = payload.Address.City,
                        Street = payload.Address.Street,
                        StreetNumber = payload.Address.StreetNumber,
                        PostalCode = payload.Address.PostalCode
                    };

                    var addedAddress = await _unitOfWork.GetRepository<AddressRepository, Address>().AddAsync(address);
                    if (addedAddress == null)
                    {
                        return null;
                    }
                    provider.Address = addedAddress;
                    _unitOfWork.Commit();
                }
                else
                {
                    var address = provider.Address;

                    address.State = payload.Address.State;
                    address.City = payload.Address.City;
                    address.Street = payload.Address.Street;
                    address.StreetNumber = payload.Address.StreetNumber;
                    address.PostalCode = payload.Address.PostalCode;

                    var updatedAddress = await _unitOfWork.GetRepository<AddressRepository, Address>().UpdateAsync(address);

                    if (updatedAddress == null)
                    {
                        return null;
                    }

                    provider.Address = updatedAddress;
                    _unitOfWork.Commit();
                }
            }
            var commonProperties = provider.GetType().GetProperties().Where(x => payload.GetType().GetProperty(x.Name) != null && x.Name != "Address");
            foreach (var property in commonProperties)
            {
                var value = payload.GetType().GetProperty(property.Name).GetValue(payload);
                if (value != null)
                {
                    property.SetValue(provider, value);
                }
            }

            var updatedProvider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().UpdateAsync(provider) ?? throw new DbUpdateException("Provider update failed");

            _unitOfWork.Commit();
            return updatedProvider;
        }

        public async Task<Service> UpdatePrice(UpdatePriceDto payload)
        {
            var service = await _unitOfWork.GetRepository<ServicesRepository, Service>().GetByIdAsync(payload.ServiceId) ?? throw new KeyNotFoundException("Service not found");
            var prices = service.Prices;
            prices[payload.PriceKey] = payload.PriceValue;
            service.Prices = prices;
            var updatedService = await _unitOfWork.GetRepository<ServicesRepository, Service>().UpdateAsync(service) ?? throw new DbUpdateException("Service update failed");
            _unitOfWork.Commit();
            return updatedService;
        }

        public async Task<Service> UpdateService(UpdateServiceDto payload)
        {
            var service = await _unitOfWork.GetRepository<ServicesRepository, Service>().GetByIdAsync(payload.ServiceId) ?? throw new KeyNotFoundException("Service not found");

            try
            {
                var type = await _unitOfWork.GetRepository<ServiceTypesRepository, ServiceType>()
                    .GetByIdAsync((int)payload.TypeId);
                service.Type = type;
            }
            catch {}

            var commonProperties = service.GetType().GetProperties().Where(x => payload.GetType().GetProperty(x.Name) != null && x.Name != "TypeId");
            foreach (var property in commonProperties)
            {
                var value = payload.GetType().GetProperty(property.Name).GetValue(payload);
                if (value != null)
                {
                    property.SetValue(service, value);
                }
            }
            var updatedService = await _unitOfWork.GetRepository<ServicesRepository, Service>().UpdateAsync(service) ?? throw new DbUpdateException("Service update failed");

            _unitOfWork.Commit();
            return updatedService;
        }

        public async Task<Booking> UpdateBookingStatus(UpdateBookingDto payload)
        {
            var booking = await _unitOfWork.GetRepository<BookingsRepository, Booking>().GetByIdAsync(payload.BookingId) ?? throw new KeyNotFoundException("Booking not found");
            booking.Status = payload.Status;
            var updatedBooking = await _unitOfWork.GetRepository<BookingsRepository, Booking>().UpdateAsync(booking) ?? throw new DbUpdateException("Booking update failed");

            _unitOfWork.Commit();
            return updatedBooking;
        }

        public async Task<Provider> GetByUserId(int userId)
        {
            var provider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetByUserIdAsync(userId) ?? throw new KeyNotFoundException("Provider not found");
            return provider;
        }

        public async Task<List<ServiceType>> GetServiceTypes()
        {
            return await _unitOfWork.GetRepository<ServiceTypesRepository, ServiceType>().GetAllAsync();
        }

        public async Task<Service> DisableService(int serviceId)
        {
            var service = await _unitOfWork.GetRepository<ServicesRepository, Service>().GetByIdAsync(serviceId) ?? throw new KeyNotFoundException("Service not found");
            service.IsDisabled = true;
            var updatedService = await _unitOfWork.GetRepository<ServicesRepository, Service>().UpdateAsync(service) ?? throw new DbUpdateException("Service update failed");

            _unitOfWork.Commit();
            return updatedService;
        }

        public async Task DisableProviderServices(int providerId)
        {
            var provider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetByIdAsync(providerId) ?? throw new KeyNotFoundException("Provider not found");
            var services = provider.Services;
            foreach (var service in services)
            {
                service.IsDisabled = true;
                _ = await _unitOfWork.GetRepository<ServicesRepository, Service>().UpdateAsync(service) ?? throw new DbUpdateException("Service update failed");
            }
            _unitOfWork.Commit();
        }
    }
}
