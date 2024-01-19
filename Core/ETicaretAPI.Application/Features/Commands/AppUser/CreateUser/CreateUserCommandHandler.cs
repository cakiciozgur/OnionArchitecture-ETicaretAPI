using ETicaretAPI.Application.Abstractions.Services.User;
using ETicaretAPI.Application.DTOs.User;
using ETicaretAPI.Application.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommandRequest, CreateUserCommandResponse>
    {
        readonly IUserService _userService;
        public CreateUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<CreateUserCommandResponse> Handle(CreateUserCommandRequest request, CancellationToken cancellationToken)
        {
            CreateUserResponse result = await _userService.CreateTaskAsync(new DTOs.User.CreateUser
            {
                NameSurname = request.NameSurname,
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                PasswordConfirm = request.PasswordConfirm
            });

            return new CreateUserCommandResponse
            {
                Message = result.Message,
                Success = result.Success
            };
        }
    }
}
