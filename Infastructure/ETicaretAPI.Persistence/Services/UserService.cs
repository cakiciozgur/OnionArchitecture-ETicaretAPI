﻿using ETicaretAPI.Application.Abstractions.Services.User;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace ETicaretAPI.Persistence.Services
{
    public class UserService : IUserService
    {
        readonly UserManager<AppUser> _userManager;
        public UserService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public async Task<CreateUserResponse> CreateTaskAsync(CreateUser createUser)
        {
            IdentityResult result = await _userManager.CreateAsync(new Domain.Entities.Identity.AppUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = createUser.Username,
                Email = createUser.Email,
                NameSurname = createUser.NameSurname
            }, createUser.Password);

            CreateUserResponse response = new CreateUserResponse { Success = result.Succeeded };

            if (!result.Succeeded)
            {
                var errorMessage = result.Errors.Select(x => x.Description).ToList();
                response.Message = string.Join(",", errorMessage);
                return response;
            }

            response.Message = "Kullanıcı başarıyla oluşturulmuştur.";
            return response;
        }
        public async Task<bool> UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime refreshTokenEndDate, int addOnAccessTokenDate)
        {
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenEndDate = refreshTokenEndDate.AddSeconds(addOnAccessTokenDate);

                IdentityResult identityResult = await _userManager.UpdateAsync(user);

                if (!identityResult.Succeeded)
                    return false;
            }
            else
            {
                throw new NotFounUserException();
            }

            return true;
        }
        public async Task UpdatePasswordAsync(string userId, string resetToken, string newPassword)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                resetToken = CustomEncoders.UrlDecode(resetToken);

                IdentityResult result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

                if(result.Succeeded)
                    await _userManager.UpdateSecurityStampAsync(user);
                else
                    throw new PasswordChangeFailedException();
            }
        }
        public async Task<(List<ListUser>, int totalUserCount)> GetAllUsersAsync(int page, int size)
        {
            var query = _userManager.Users;

            IQueryable<AppUser> users = null;

            if(page != -1 && size != -1)
            {
               users = query.Skip(page * size).Take(size);
            }
            else
            {
               users = query;
            }

            var userList = users.Select(u => new ListUser
            {
                Id = u.Id,
                Email = u.Email,
                NameSurname = u.NameSurname,
                Username = u.UserName,
                TwoFactorEnabled = u.TwoFactorEnabled
            }).ToList();

            return (userList, query.Count());
        }

        public async Task<bool> AssignRoleToUserAsync(string[] roles, string userId)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if(user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, userRoles);

                try
                {
                    IdentityResult result = await _userManager.AddToRolesAsync(user, roles);
                    return result.Succeeded;

                }
                catch (Exception ex)
                {

                }
            }

            return false;
        }

        public async Task<List<string>> GetRolesToUser(string userId)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                return userRoles.ToList();
                
            }

            return new List<string>();
        }
    }
}
