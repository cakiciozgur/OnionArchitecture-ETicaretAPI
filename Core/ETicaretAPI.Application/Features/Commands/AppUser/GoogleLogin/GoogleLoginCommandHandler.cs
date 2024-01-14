using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin
{
    public class GoogleLoginCommandHandler : IRequestHandler<GoogleLoginCommandRequest, GoogleLoginCommandResponse>
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly IConfiguration _configuration;
        readonly ITokenHandler _tokenHandler;
        public GoogleLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, IConfiguration configuration, ITokenHandler tokenHandler)
        {
            _userManager = userManager;
            _configuration = configuration;
            _tokenHandler = tokenHandler;
        }

        public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandRequest request, CancellationToken cancellationToken)
        {
            GoogleJsonWebSignature.ValidationSettings settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>
                {
                    _configuration["ExternalLogin:GoogleClientId"]
                }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, settings);

            var info = new UserLoginInfo(request.Provider, payload.Subject, request.Provider); //AspNetIdentity tablolarında register olan kişiler AspNetUsers dış kaynaktan gelen kullanıcıların hangi dış kaynaktan geldiği bilgisi ise AspNetUsersLogins tablosunda tutulur.

            Domain.Entities.Identity.AppUser user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            bool result = user != null; 

            if(user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);

                if(user == null)
                {
                    user = new Domain.Entities.Identity.AppUser
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = payload.Email,
                        NameSurname = payload.Name,
                        UserName = payload.Email
                    };
                    IdentityResult createResult = await _userManager.CreateAsync(user);
                    result = createResult.Succeeded;
                }
            }

            if (result)
                await _userManager.AddLoginAsync(user, info); // dış kaynaktan geldiğini biliyoruz o yüzden AspNetUsersLogins tablosuna da dış kaynak bilgilerini ekliyoruz!
            else
                throw new Exception("Invalid external Authentication");

            Token token = _tokenHandler.CreateAccessToken(5);

            return new GoogleLoginCommandResponse
            {
                Token = token
            };
        }
    }
}
