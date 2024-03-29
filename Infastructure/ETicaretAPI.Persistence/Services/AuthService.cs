﻿using ETicaretAPI.Application.Abstractions.Services.Auth;
using ETicaretAPI.Application.Abstractions.Services.Mail;
using ETicaretAPI.Application.Abstractions.Services.User;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.DTOs.Facebook;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using ETicaretAPI.Application.Helpers;
using ETicaretAPI.Domain.Entities.Identity;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace ETicaretAPI.Persistence.Services
{
    public class AuthService : IAuthService
    {
        readonly UserManager<AppUser> _userManager;
        readonly SignInManager<AppUser> _signInManager;
        readonly IConfiguration _configuration;
        readonly ITokenHandler _tokenHandler;
        readonly HttpClient _httpClient;
        readonly IUserService _userService;
        readonly IMailService _mailService;
        public AuthService(UserManager<AppUser> userManager, IConfiguration configuration, ITokenHandler tokenHandler, IHttpClientFactory httpClient, SignInManager<AppUser> signInManager, IUserService userService, IMailService mailService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenHandler = tokenHandler;
            _httpClient = httpClient.CreateClient();
            _signInManager = signInManager;
            _userService = userService;
            _mailService = mailService;
        }
        private async Task<Token> CreateUserExternalLoginAsync(AppUser user, UserLoginInfo info, string name, string email, int expirationTime)
        {
            bool result = user != null;

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new AppUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = email,
                        NameSurname = name,
                        UserName = email
                    };
                    IdentityResult createResult = await _userManager.CreateAsync(user);
                    result = createResult.Succeeded;
                }
            }

            if (result)
            {
                await _userManager.AddLoginAsync(user, info); // dış kaynaktan geldiğini biliyoruz o yüzden AspNetUsersLogins tablosuna da dış kaynak bilgilerini ekliyoruz!

                Token token = _tokenHandler.CreateAccessToken(expirationTime, user);

                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, 5);

                return token;
            }

            throw new Exception("Invalid External Authentication");

        }
        public async Task<Token> FacebookLoginAsync(string authToken, int expirationTime)
        {
            string clientId = _configuration["ExternalLogin:FacebookClient:ClientId"];
            string clientSecret = _configuration["ExternalLogin:FacebookClient:ClientSecret"];
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials");

            FacebookAccessTokenResponse? facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponse>(accessTokenResponse);
            if (facebookAccessTokenResponse == null)
                throw new Exception("AccessToken Error!");

            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={authToken}&access_token={facebookAccessTokenResponse.AccessToken}");

            FacebookUserAccessTokenValidation? validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidation>(userAccessTokenValidation);

            if (validation == null)
                throw new Exception("UserAccessToken Validation Error!");

            if (validation.Data.IsValid)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={authToken}");

                FacebookUserInfoResponse? fbUserInfoResponse = JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoResponse);

                var info = new UserLoginInfo("FACEBOOK", validation.Data.UserId, "FACEBOOK"); //AspNetIdentity tablolarında register olan kişiler AspNetUsers dış kaynaktan gelen kullanıcıların hangi dış kaynaktan geldiği bilgisi ise AspNetUsersLogins tablosunda tutulur.

                AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                Token token = await CreateUserExternalLoginAsync(user, info, fbUserInfoResponse.Name, fbUserInfoResponse.Email, expirationTime);

                return token;
            }

            throw new Exception("Facebook UserAccessToken Validation Error!");
        }
        public async Task<Token> GoogleLoginAsync(GoogleLoginRequest model, int expirationTime)
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>
                {
                    _configuration["ExternalLogin:GoogleClientId"]
                }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, settings);
            var info = new UserLoginInfo(model.Provider, payload.Subject, model.Provider); //AspNetIdentity tablolarında register olan kişiler AspNetUsers dış kaynaktan gelen kullanıcıların hangi dış kaynaktan geldiği bilgisi ise AspNetUsersLogins tablosunda tutulur.

            AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            Token token = await CreateUserExternalLoginAsync(user, info, payload.Name, payload.Email, expirationTime);

            return token;
        }
        public async Task<LoginUserResponse> LoginAsync(string userNameOrEmail, string password, int expirationTime)
        {
            AppUser user = await _userManager.FindByNameAsync(userNameOrEmail);
            if (user == null)
                user = await _userManager.FindByEmailAsync(userNameOrEmail);

            if (user == null)
                throw new NotFounUserException();

            SignInResult result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (result.Succeeded) // Authentication success
            {
                Token token = _tokenHandler.CreateAccessToken(expirationTime, user);

                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, 15);

                return new LoginUserResponse
                {
                    Success = true,
                    Token = token
                };
            }

            throw new AuthenticationErrorException();
        }
        public async Task<LoginUserResponse> RefreshTokenLoginAsync(string refreshToken, int addOnAccessTokenDate)
        {
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);

            if (user != null && user.RefreshTokenEndDate > DateTime.UtcNow)
            {
                Token token = _tokenHandler.CreateAccessToken(15, user);
                await _userService.UpdateRefreshTokenAsync(token.RefreshToken, user, token.Expiration, addOnAccessTokenDate);

                return new LoginUserResponse
                {
                    Success = true,
                    Token = token
                };
            }
            else
                throw new NotFounUserException();
        }
        public async  Task PasswordResetAsync(string email)
        {
            AppUser user = await _userManager.FindByEmailAsync(email);

            if (user != null)
            {
                string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

                resetToken = CustomEncoders.UrlEncode(resetToken);

                await _mailService.SendPasswordResetMailAsync(email, user.Id, resetToken);
            }
        }
        public async Task<bool> VerifyResetTokenAsync(string resetToken, string userId)
        {
            AppUser user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                resetToken = CustomEncoders.UrlDecode(resetToken);

                return await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetToken);
            }

            return false;
        }
    }
}
