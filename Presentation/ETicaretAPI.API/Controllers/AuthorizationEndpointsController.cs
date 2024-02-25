using ETicaretAPI.Application.Features.Commands.AppUser.LoginUser;
using ETicaretAPI.Application.Features.Commands.AuthorizationEndpoint.AssignRoleEndpoint;
using ETicaretAPI.Application.Features.Queries.AuthorizationEndpoint.GetRolesToEndpoint;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationEndpointsController : ControllerBase
    {
        readonly IMediator _mediator;
        public AuthorizationEndpointsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("get-roles-to-endpoint")]
        public async Task<IActionResult> GetRolesToEndpoints([FromBody] GetRolesToEndpointQueryRequest getRolesEndpointQueryRequest)
        {
            GetRolesToEndpointQueryResponse getRolesEndpointQueryResponse = await _mediator.Send(getRolesEndpointQueryRequest);
            return Ok(getRolesEndpointQueryResponse);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleEndpoint(AssignRoleEndpointRequest assignRoleEndpointRequest)
        {
            assignRoleEndpointRequest.Type = typeof(Program);
            AssignRoleEndpointResponse assignRoleEndpointResponse = await _mediator.Send(assignRoleEndpointRequest);
            return Ok(assignRoleEndpointResponse);
        }
    }
}
