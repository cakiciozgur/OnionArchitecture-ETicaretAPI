using ETicaretAPI.Application.DTOs.User;

namespace ETicaretAPI.Application.Abstractions.Services.Auth
{
    public interface IExternalAuth
    {
        Task<DTOs.Token> GoogleLoginAsync(GoogleLoginRequest model, int expirationTime);
        Task<DTOs.Token> FacebookLoginAsync(string authToken, int expirationTime);
    }
}
