using DataLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Dtos;
using DataLayer.Entities;
using DataLayer.Repositories;

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

        public async Task<User?> UpdatePassword(ChangePasswordDto payload)
        {
            var user = await _unitOfWork.GetRepository<UsersRepository, User>().GetByIdAsync(payload.UserId);


            var passwordFine = _authService.VerifyHashedPassword(user.PasswordHash, payload.CurrentPassword);
            if (passwordFine)
            {
                user.PasswordHash = _authService.HashPassword(payload.NewPassword);
                var updatedUser = await _unitOfWork.GetRepository<UsersRepository, User>().UpdateAsync(user);
                _unitOfWork.Commit();
                return updatedUser;
            }

            return null;


        }

        public async Task<User?> UpdateEmail(ChangeEmailDto payload)
        {
            var user = await _unitOfWork.GetRepository<UsersRepository, User>().GetByIdAsync(payload.UserId) ?? throw new ApplicationException("User not found");

            var passwordFine = _authService.VerifyHashedPassword(user.PasswordHash, payload.CurrentPassword);
            if (passwordFine)
            {
                user.Email = payload.NewEmail;
                var updatedUser = await _unitOfWork.GetRepository<UsersRepository, User>().UpdateAsync(user);
                _unitOfWork.Commit();
                return updatedUser;
            }
            return null;
        }

        public async Task<User?> UpdateUsername(ChangeUsernameDto payload)
        {
            var user = await _unitOfWork.GetRepository<UsersRepository, User>().GetByIdAsync(payload.UserId) ?? throw new ApplicationException("User not found");
            var passwordFine = _authService.VerifyHashedPassword(user.PasswordHash, payload.CurrentPassword);
            if (passwordFine)
            {
                user.Username = payload.NewUsername;
                var updatedUser = await _unitOfWork.GetRepository<UsersRepository, User>().UpdateAsync(user);
                _unitOfWork.Commit();
                return updatedUser;
            }
            return null;
        }

        public async Task<User?> UpdatePhoneNumber(ChangePhoneNumberDto payload)
        {
            var user = await _unitOfWork.GetRepository<UsersRepository, User>().GetByIdAsync(payload.UserId) ?? throw new ApplicationException("User not found");
            var passwordFine = _authService.VerifyHashedPassword(user.PasswordHash, payload.CurrentPassword);
            if (passwordFine)
            {
                user.PhoneNumber = payload.NewPhoneNumber;
                var updatedUser = await _unitOfWork.GetRepository<UsersRepository, User>().UpdateAsync(user);
                _unitOfWork.Commit();
                return updatedUser;
            }
            return null;
        }
    }
}
