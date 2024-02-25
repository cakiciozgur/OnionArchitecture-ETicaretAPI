using MediatR;

namespace ETicaretAPI.Application.Features.Commands.AuthorizationEndpoint.AssignRoleEndpoint
{
    public class AssignRoleEndpointRequest : IRequest<AssignRoleEndpointResponse>
    {
        public string[] Roles { get; set; }
        public string Code { get; set; }
        public string Menu { get; set; }
        public Type? Type { get; set; }
    }
}