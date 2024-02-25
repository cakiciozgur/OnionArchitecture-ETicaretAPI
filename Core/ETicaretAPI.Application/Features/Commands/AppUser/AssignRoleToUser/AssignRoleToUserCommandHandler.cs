using ETicaretAPI.Application.Abstractions.Services.User;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.AssignRoleToUser
{
    public class AssignRoleToUserCommandHandler : IRequestHandler<AssignRoleToUserCommandRequest, AssignRoleToUserCommandResponse>
    {
        readonly IUserService _userService;

        public AssignRoleToUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<AssignRoleToUserCommandResponse> Handle(AssignRoleToUserCommandRequest request, CancellationToken cancellationToken)
        {
            bool succeeded = await _userService.AssignRoleToUserAsync(request.Roles, request.Id);

            return new AssignRoleToUserCommandResponse
            {
                Succeeded = succeeded,
            };
        }
    }
}
