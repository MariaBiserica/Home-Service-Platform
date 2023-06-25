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
            var serviceType = await _unitOfWork.GetRepository<ServiceTypesRepository, ServiceType>().GetByIdAsync(typeId) ?? throw new ApplicationException("Service type not found");
            var services = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetServicesByType(providerId, serviceType);
            return services;
        }

        public async Task<Service> GetServiceById(int serviceId)
        {
            var service = await _unitOfWork.GetRepository<ServicesRepository, Service>().GetByIdAsync(serviceId) ?? throw new ApplicationException("Service not found");
            return service;
        }

        public async Task<Provider> GetByEmail(string email)
        {
            var provider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetByEmailAsync(email);
            return provider;
        }

        public async void Register(ProviderRegisterDto registerData)
        {
            if (registerData == null)
            {
                return;
            }

            var hashedPassword = _authService.HashPassword(registerData.UserData.Password);
            var newUser = new User
            {
                Username = registerData.Username,
                Email = registerData.UserData.Email,
                PasswordHash = hashedPassword
            };

            var user = await _unitOfWork.GetRepository<UsersRepository, User>().AddAsync(newUser);
            if (user == null)
            {
                return;
            }

            var newProvider = new Provider
            {
                Bio = registerData.Bio,
                User = user
            };

            var provider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().AddAsync(newProvider);
            if (provider == null)
            {
                return;
            }

            _unitOfWork.Commit();
        }

        public async Task<string> Validate(LoginDto payload)
        {
            var provider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetByEmailAsync(payload.Email) ?? throw new ApplicationException("User not found");

            var passwordFine = _authService.VerifyHashedPassword(provider.User.PasswordHash, payload.Password);
            if (passwordFine)
            {
                return _authorizationService.GetToken(provider.User, "Provider");
            }
            else
            {
                throw new ApplicationException("Incorrect password");
            }

        }

        public async Task AddService(int providerId, ServiceDto payload)
        {

            var provider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetByIdAsync(providerId) ?? throw new ApplicationException("Provider not found");
            var type = await _unitOfWork.GetRepository<ServiceTypesRepository, ServiceType>().GetByIdAsync(payload.TypeId) ?? throw new ApplicationException("Service type not found");

            var service = new Service()
            {
                Title = payload.Title,
                Type = type,
                Description = payload.Description,
                Prices = payload.Prices,
                Provider = provider,
            };

            await _unitOfWork.GetRepository<ServicesRepository, Service>().AddAsync(service);
            //await _unitOfWork.GetRepository<ProvidersRepository, Provider>().AddServiceAsync(provider.Id, service);
            _unitOfWork.Commit();
        }

        public async Task<Provider> UpdateProvider(int providerId, UpdateProviderDto payload)
        {
            var provider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetByIdAsync(providerId) ?? throw new ApplicationException("Provider not found");
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

            var updatedProvider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().UpdateAsync(provider);
            if (updatedProvider == null)
            {
                return null;
            }
            _unitOfWork.Commit();
            return updatedProvider;
        }

        public async Task<Service> UpdateService(UpdateServiceDto payload)
        {
            var service = await _unitOfWork.GetRepository<ServicesRepository, Service>().GetByIdAsync(payload.ServiceId) ?? throw new ApplicationException("Service not found");

            try
            {
                var type = await _unitOfWork.GetRepository<ServiceTypesRepository, ServiceType>()
                    .GetByIdAsync((int)payload.TypeId);
                service.Type = type;

            }
            catch
            {
            }


            var commonProperties = service.GetType().GetProperties().Where(x => payload.GetType().GetProperty(x.Name) != null && x.Name != "TypeId");
            foreach (var property in commonProperties)
            {
                var value = payload.GetType().GetProperty(property.Name).GetValue(payload);
                if (value != null)
                {
                    property.SetValue(service, value);
                }
            }
            var updatedService = await _unitOfWork.GetRepository<ServicesRepository, Service>().UpdateAsync(service);
            if (updatedService == null)
            {
                return null;
            }
            _unitOfWork.Commit();
            return updatedService;
        }

        public async Task<Booking> UpdateBookingStatus(UpdateBookingDto payload)
        {
            var booking = await _unitOfWork.GetRepository<BookingsRepository, Booking>().GetByIdAsync(payload.BookingId) ?? throw new ApplicationException("Booking not found");
            booking.Status = payload.Status;
            var updatedBooking = await _unitOfWork.GetRepository<BookingsRepository, Booking>().UpdateAsync(booking);
            if (updatedBooking == null)
            {
                return null;
            }
            _unitOfWork.Commit();
            return updatedBooking;
        }

        public async Task<Provider> GetByUserId(int userId)
        {
            return await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetByUserIdAsync(userId);
        }
    }
}
