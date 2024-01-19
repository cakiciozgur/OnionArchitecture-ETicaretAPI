using ETicaretAPI.Application.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services.Auth
{
    public interface IInternalAuth
    {
        Task<LoginUserResponse> LoginAsync(string userNameOrEmail, string password, int expirationTime);
    }
}
