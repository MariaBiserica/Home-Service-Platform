using Core.Dtos;
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
                Rating = 0,
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

        public async Task AddService(ServiceDto payload)
        {

            var provider = await _unitOfWork.GetRepository<ProvidersRepository, Provider>().GetByIdAsync(payload.ProviderId) ?? throw new ApplicationException("Provider not found");
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
    }
}
