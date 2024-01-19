using ETicaretAPI.Application.Abstractions.Services.User;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace ETicaretAPI.Persistence.Services
{
    public class UserService : IUserService
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        public UserService(UserManager<Domain.Entities.Identity.AppUser> userManager)
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

        public async Task<bool> UpdateRefreshToken(string refreshToken, AppUser user, DateTime refreshTokenEndDate, int addOnAccessTokenDate)
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
    }
}
