using ETicaretAPI.Application.Abstractions.Services.Auth;
using ETicaretAPI.Application.Abstractions.Token;
using ETicaretAPI.Application.DTOs;
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
        readonly IExternalAuth _authService;
        public GoogleLoginCommandHandler(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<GoogleLoginCommandResponse> Handle(GoogleLoginCommandRequest request, CancellationToken cancellationToken)
        {
            Token token = await _authService.GoogleLoginAsync(new DTOs.User.GoogleLoginRequest
            {
                Id = request.Id,
                IdToken = request.IdToken,
                Name = request.Name,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhotoUrl = request.PhotoUrl,
                Provider = request.Provider
            },900);

            return new GoogleLoginCommandResponse
            {
                Token = token
            };
        }
    }
}
