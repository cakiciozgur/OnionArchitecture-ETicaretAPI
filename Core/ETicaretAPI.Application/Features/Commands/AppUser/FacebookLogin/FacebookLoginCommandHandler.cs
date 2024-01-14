using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.DTOs.Facebook;
using ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandRequest, FacebookLoginCommandResponse>
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly IConfiguration _configuration;
        readonly ITokenHandler _tokenHandler;
        readonly HttpClient _httpClient;
        public FacebookLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, IConfiguration configuration, ITokenHandler tokenHandler, IHttpClientFactory httpClient)
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenHandler = tokenHandler;
            _httpClient = httpClient.CreateClient();
        }

        public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
        {
            string clientId = _configuration["ExternalLogin:FacebookClient:ClientId"];
            string clientSecret = _configuration["ExternalLogin:FacebookClient:ClientSecret"];
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={clientId}&client_secret={clientSecret}&grant_type=client_credentials");

            FacebookAccessTokenResponse? facebookAccessTokenResponse = JsonSerializer.Deserialize<FacebookAccessTokenResponse>( accessTokenResponse );

            if (facebookAccessTokenResponse == null)
                throw new Exception("AccessToken Error!");

            string userAccessTokenValidation = await _httpClient.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={request.AuthToken}&access_token={facebookAccessTokenResponse.AccessToken}");

            FacebookUserAccessTokenValidation? validation = JsonSerializer.Deserialize<FacebookUserAccessTokenValidation>(userAccessTokenValidation);

            if (validation == null)
                throw new Exception("UserAccessToken Validation Error!");

            if (validation.Data.IsValid)
            {
                string userInfoResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/me?fields=email,name&access_token={request.AuthToken}");

                FacebookUserInfoResponse? fbUserInfoResponse = JsonSerializer.Deserialize<FacebookUserInfoResponse>(userInfoResponse);

                var info = new UserLoginInfo("FACEBOOK", validation.Data.UserId, "FACEBOOK"); //AspNetIdentity tablolarında register olan kişiler AspNetUsers dış kaynaktan gelen kullanıcıların hangi dış kaynaktan geldiği bilgisi ise AspNetUsersLogins tablosunda tutulur.

                Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

                bool result = user != null;

                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(fbUserInfoResponse.Email);

                    if (user == null)
                    {
                        user = new Domain.Entities.Identity.AppUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            Email = fbUserInfoResponse.Email,
                            NameSurname = fbUserInfoResponse.Name,
                            UserName = fbUserInfoResponse.Email
                        };
                        IdentityResult createResult = await _userManager.CreateAsync(user);
                        result = createResult.Succeeded;
                    }
                }

                if (result)
                {
                    await _userManager.AddLoginAsync(user, info); // dış kaynaktan geldiğini biliyoruz o yüzden AspNetUsersLogins tablosuna da dış kaynak bilgilerini ekliyoruz!

                    Token token = _tokenHandler.CreateAccessToken(5);

                    return new FacebookLoginCommandResponse
                    {
                        Token = token
                    };
                }

            }

            throw new Exception("Invalid External Authentication");
        }
    }
}
