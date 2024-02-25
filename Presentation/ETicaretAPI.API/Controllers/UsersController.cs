using ETicaretAPI.Application.Constants;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.Features.Commands.AppUser.AssignRoleToUser;
using ETicaretAPI.Application.Features.Commands.AppUser.CreateUser;
using ETicaretAPI.Application.Features.Commands.AppUser.FacebookLogin;
using ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin;
using ETicaretAPI.Application.Features.Commands.AppUser.LoginUser;
using ETicaretAPI.Application.Features.Commands.AppUser.UpdatePassword;
using ETicaretAPI.Application.Features.Queries.AppUser.GetAllUsers;
using ETicaretAPI.Application.Features.Queries.AppUser.GetRolesToUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        readonly IMediator _mediator;
        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserCommandRequest createUserCommandRequest)
        {
            CreateUserCommandResponse createUserCommandResponse = await _mediator.Send(createUserCommandRequest);
            return Ok(createUserCommandResponse);
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordCommandRequest updatePasswordCommandRequest)
        {
            UpdatePasswordCommandResponse updatePasswordCommandResponse = await _mediator.Send(updatePasswordCommandRequest);
            return Ok(updatePasswordCommandResponse);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitonConstant.Users, ActionType = Application.Enums.ActionType.Reading, Definition = "Get All Users")]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQueryRequest getAllUsersQueryRequest)
        {
            GetAllUsersQueryResponse getAllUsersQueryResponse = await _mediator.Send(getAllUsersQueryRequest);
            return Ok(getAllUsersQueryResponse);
        }

        [HttpGet("get-roles-to-user/{UserId}")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitonConstant.Users, ActionType = Application.Enums.ActionType.Reading, Definition = "Get Roles To User")]
        public async Task<IActionResult> GetRolesToUser([FromRoute] GetRolesToUserQueryRequest getRolesToUserQueryRequest)
        {
            GetRolesToUserQueryResponse getRolesToUserQueryResponse = await _mediator.Send(getRolesToUserQueryRequest);
            return Ok(getRolesToUserQueryResponse);
        }

        [HttpPost("assign-role-to-user")]
        [Authorize(AuthenticationSchemes = "Admin")]
        [AuthorizeDefinition(Menu = AuthorizeDefinitonConstant.Users, ActionType = Application.Enums.ActionType.Writing, Definition = "Assign Role To User")]
        public async Task<IActionResult> AssignRoleToUser([FromBody] AssignRoleToUserCommandRequest assignRoleToUserCommandRequest)
        {
            AssignRoleToUserCommandResponse assignRoleToUserCommandResponse = await _mediator.Send(assignRoleToUserCommandRequest);
            return Ok(assignRoleToUserCommandResponse);
        }

    }
}
