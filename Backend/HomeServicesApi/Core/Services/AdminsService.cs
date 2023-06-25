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
            if (registerData == null)
            {
                return;
            }

            var hashedPassword = _authService.HashPassword(registerData.Password);
            var newUser = new User
            {
                Email = registerData.Email,
                PasswordHash = hashedPassword
            };

            var user = await _unitOfWork.GetRepository<UsersRepository, User>().AddAsync(newUser);
            if (user == null)
            {
                return;
            }

            var newAdmin = new Admin
            {
                User = user,
            };

            var provider = await _unitOfWork.GetRepository<AdminsRepository, Admin>().AddAsync(newAdmin);
            if (provider == null)
            {
                return;
            }

            _unitOfWork.Commit();
        }

        public async Task<string> Validate(LoginDto payload)
        {
            var provider = await _unitOfWork.GetRepository<AdminsRepository, Admin>().GetByEmailAsync(payload.Email) ?? throw new ApplicationException("User not found");

            var passwordFine = _authService.VerifyHashedPassword(provider.User.PasswordHash, payload.Password);
            if (passwordFine)
            {
                return _authorizationService.GetToken(provider.User, "Admin");
            }
            else
            {
                throw new ApplicationException("Incorrect password");
            }

        }

        public async Task<ServiceType> AddServiceType(string name)
        {
            var serviceType = new ServiceType
            {
                Name = name
            };

            var newServiceType = await _unitOfWork.GetRepository<ServiceTypesRepository, ServiceType>().AddAsync(serviceType);
            if (newServiceType == null)
            {
                return null;
            }
            _unitOfWork.Commit();
            return newServiceType;
        }

        public async Task DeleteAdmin(int adminId)
        {
            var admin = await _unitOfWork.GetRepository<AdminsRepository, Admin>().GetByIdAsync(adminId);
            if (admin == null)
            {
                return;
            }
            _unitOfWork.GetRepository<AdminsRepository, Admin>().DeleteAsync(admin);
            _unitOfWork.Commit();
        }

        public async Task DeleteServiceType(int serviceTypeId)
        {
            var serviceType = await _unitOfWork.GetRepository<ServiceTypesRepository, ServiceType>().GetByIdAsync(serviceTypeId);
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
