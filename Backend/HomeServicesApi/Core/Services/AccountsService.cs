using DataLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Dtos;
using DataLayer.Entities;
using DataLayer.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class AccountsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly AuthenticationService _authService;
        public AccountsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _authService = new AuthenticationService();
        }

        public async Task<User> UpdatePassword(int userId, ChangePasswordDto payload)
        {
            var user = await _unitOfWork.GetRepository<UsersRepository, User>().GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");

            var passwordFine = _authService.VerifyHashedPassword(user.PasswordHash, payload.CurrentPassword);
            if (passwordFine)
            {
                try
                {
                    user.PasswordHash = _authService.HashPassword(payload.NewPassword);
                }
                catch 
                { 
                    throw;
                }
                var updatedUser = await _unitOfWork.GetRepository<UsersRepository, User>().UpdateAsync(user) ?? throw new DbUpdateException("User not updated");
                _unitOfWork.Commit();
                return updatedUser;
            }

            throw new UnauthorizedAccessException("Wrong password");
        }

        public async Task<User> UpdateEmail(int userId, ChangeEmailDto payload)
        {
            var user = await _unitOfWork.GetRepository<UsersRepository, User>().GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");

            try
            {
                var passwordFine = _authService.VerifyHashedPassword(user.PasswordHash, payload.CurrentPassword);
                if (passwordFine)
                {
                    user.Email = payload.NewEmail;
                    var updatedUser = await _unitOfWork.GetRepository<UsersRepository, User>().UpdateAsync(user) ?? throw new DbUpdateException("User not updated");
                    _unitOfWork.Commit();
                    return updatedUser;
                }
                throw new UnauthorizedAccessException("Wrong password");
            }
            catch
            {
                throw;
            }
        }

        public async Task<User> UpdateUsername(int userId, ChangeUsernameDto payload)
        {
            var user = await _unitOfWork.GetRepository<UsersRepository, User>().GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
            try
            {
                var passwordFine = _authService.VerifyHashedPassword(user.PasswordHash, payload.CurrentPassword);
                if (passwordFine)
                {
                    user.Username = payload.NewUsername;
                    var updatedUser = await _unitOfWork.GetRepository<UsersRepository, User>().UpdateAsync(user) ?? throw new DbUpdateException("User not updated");
                    _unitOfWork.Commit();
                    return updatedUser;
                }
                throw new UnauthorizedAccessException("Wrong password");
            }
            catch
            {
                throw;
            }
        }

        public async Task<User> UpdatePhoneNumber(int userId, ChangePhoneNumberDto payload)
        {
            var user = await _unitOfWork.GetRepository<UsersRepository, User>().GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
            try
            {
                var passwordFine = _authService.VerifyHashedPassword(user.PasswordHash, payload.CurrentPassword);
                if (passwordFine)
                {
                    user.PhoneNumber = payload.NewPhoneNumber;
                    var updatedUser = await _unitOfWork.GetRepository<UsersRepository, User>().UpdateAsync(user) ?? throw new DbUpdateException("User not updated");
                    _unitOfWork.Commit();
                    return updatedUser;
                }
                throw new UnauthorizedAccessException("Wrong password");
            }
            catch
            {
                throw;
            }
        }

        public async Task<User> DisableCustomerAccount(int userId, string password)
        {
            var user = await _unitOfWork.GetRepository<UsersRepository, User>().GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
            try
            {
                var passwordFine = _authService.VerifyHashedPassword(user.PasswordHash, password);
                if (passwordFine)
                {
                    user.IsDisabled = true;
                    var updatedUser = await _unitOfWork.GetRepository<UsersRepository, User>().UpdateAsync(user) ?? throw new DbUpdateException("User not updated");
                    _unitOfWork.Commit();
                    return updatedUser;
                }
                throw new UnauthorizedAccessException("Wrong password");
            }
            catch
            {
                throw;
            }
        }

        public async Task AdminDisableCustomerAccount(int userId)
        {
            var user = await _unitOfWork.GetRepository<UsersRepository, User>().GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
            user.IsDisabled = true;
            _ = await _unitOfWork.GetRepository<UsersRepository, User>().UpdateAsync(user) ?? throw new DbUpdateException("User not updated");
            _unitOfWork.Commit();
        }

        public async Task DisableProviderAccount(int userId, string password)
        {
            var user = await _unitOfWork.GetRepository<UsersRepository, User>().GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");

            try
            {
                var passwordFine = _authService.VerifyHashedPassword(user.PasswordHash, password);
                if (passwordFine)
                {
                    user.IsDisabled = true;
                    _ = await _unitOfWork.GetRepository<UsersRepository, User>().UpdateAsync(user) ?? throw new DbUpdateException("User not updated");
                    _unitOfWork.Commit();
                }
                throw new UnauthorizedAccessException("Wrong password");
            }
            catch
            {
                throw;
            }
        }

        public async Task AdminDisableProviderAccount(int userId)
        {
            var user = await _unitOfWork.GetRepository<UsersRepository, User>().GetByIdAsync(userId) ?? throw new KeyNotFoundException("User not found");
            user.IsDisabled = true;
            _ = await _unitOfWork.GetRepository<UsersRepository, User>().UpdateAsync(user) ?? throw new DbUpdateException("User not updated");
            _unitOfWork.Commit();
        }
    }
}
