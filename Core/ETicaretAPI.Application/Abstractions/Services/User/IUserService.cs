using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services.User
{
    public interface IUserService
    {
        Task<CreateUserResponse> CreateTaskAsync(CreateUser createUser);
        Task<bool> UpdateRefreshTokenAsync(string refreshToken, AppUser user, DateTime refreshTokenEndDate, int addOnAccessTokenDate);
        Task UpdatePasswordAsync(string userId, string resetToken, string newPassword);
        Task<(List<ListUser>, int totalUserCount)> GetAllUsersAsync(int page, int size);
        Task<bool> AssignRoleToUserAsync(string[] roles, string userId);
        Task<List<string>> GetRolesToUser(string userId);
    }
}
