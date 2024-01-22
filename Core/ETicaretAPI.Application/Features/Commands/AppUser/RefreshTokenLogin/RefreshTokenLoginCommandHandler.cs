using ETicaretAPI.Application.Abstractions.Services.Auth;
using ETicaretAPI.Application.DTOs;
using ETicaretAPI.Application.DTOs.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.RefreshTokenLogin
{
    public class RefreshTokenLoginCommandHandler : IRequestHandler<RefreshTokenLoginCommandRequest, RefreshTokenLoginCommandResponse>
    {
        readonly IInternalAuth _authService;

        public RefreshTokenLoginCommandHandler(IInternalAuth authService)
        {
            _authService = authService;
        }

        public async Task<RefreshTokenLoginCommandResponse> Handle(RefreshTokenLoginCommandRequest request, CancellationToken cancellationToken)
        {
            LoginUserResponse loginUserResponse = await _authService.RefreshTokenLoginAsync(request.RefreshToken, 300);

            return new RefreshTokenLoginCommandResponse
            {
                Token = loginUserResponse.Token
            };
        }
    }
}
