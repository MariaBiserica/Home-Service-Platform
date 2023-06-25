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
    public class AdminsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AuthenticationService _authService;
        private readonly AuthorizationService _authorizationService;

        public AdminsService(IUnitOfWork unitOfWork, AuthenticationService authService, AuthorizationService authorizationService)
        {
            _unitOfWork = unitOfWork;
            _authService = authService;
            _authorizationService = authorizationService;
        }

        public async void Register(UserRegisterDto registerData)
        {
            if (registerData == null) throw new ArgumentException("Invalid data");

            var hashedPassword = _authService.HashPassword(registerData.Password);
            var newUser = new User
            {
                Email = registerData.Email,
                PasswordHash = hashedPassword
            };

            var user = await _unitOfWork.GetRepository<UsersRepository, User>().AddAsync(newUser) ?? throw new InvalidOperationException("User not created");

            var newAdmin = new Admin
            {
                User = user,
            };

            var provider = await _unitOfWork.GetRepository<AdminsRepository, Admin>().AddAsync(newAdmin) ?? throw new InvalidOperationException("Admin not created");
            _unitOfWork.Commit();
        }

        public async Task<string> Validate(LoginDto payload)
        {
            var provider = await _unitOfWork.GetRepository<AdminsRepository, Admin>().GetByEmailAsync(payload.Email) ?? throw new KeyNotFoundException("User not found");

            if (provider.User.IsDisabled)
            {
                throw new UnauthorizedAccessException("User is disabled");
            }

            var passwordFine = _authService.VerifyHashedPassword(provider.User.PasswordHash, payload.Password);
            if (passwordFine)
            {
                return _authorizationService.GetToken(provider.User, "Admin");
            }
            else
            {
                throw new UnauthorizedAccessException("Incorrect password");
            }

        }

        public async Task<ServiceType> AddServiceType(string name)
        {
            var serviceType = new ServiceType
            {
                Name = name
            };

            var newServiceType = await _unitOfWork.GetRepository<ServiceTypesRepository, ServiceType>().AddAsync(serviceType) ?? throw new InvalidOperationException("Service type not created");
            _unitOfWork.Commit();
            return newServiceType;
        }

        public async Task DeleteAdmin(int adminId)
        {
            var admin = await _unitOfWork.GetRepository<AdminsRepository, Admin>().GetByIdAsync(adminId) ?? throw new KeyNotFoundException("Admin not found");
            _unitOfWork.GetRepository<AdminsRepository, Admin>().DeleteAsync(admin);
            _unitOfWork.Commit();
        }

        public async Task DeleteServiceType(int serviceTypeId)
        {
            var serviceType = await _unitOfWork.GetRepository<ServiceTypesRepository, ServiceType>().GetByIdAsync(serviceTypeId) ?? throw new KeyNotFoundException("Service type not found");
            var services = await _unitOfWork.GetRepository<ServicesRepository, Service>().GetAllAsync();
            foreach (var service in services)
            {
                if (service.ServiceTypeId == serviceTypeId)
                {
                    service.ServiceTypeId = null;
                }
            }
            _unitOfWork.GetRepository<ServiceTypesRepository, ServiceType>().DeleteAsync(serviceType);
            _unitOfWork.Commit();
        }
    }
}
