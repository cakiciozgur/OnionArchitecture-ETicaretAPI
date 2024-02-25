using MediatR;

namespace ETicaretAPI.Application.Features.Commands.AppUser.AssignRoleToUser
{
    public class AssignRoleToUserCommandRequest : IRequest<AssignRoleToUserCommandResponse>
    {
        public string Id { get; set; }
        public string[] Roles { get; set; }
    }
}