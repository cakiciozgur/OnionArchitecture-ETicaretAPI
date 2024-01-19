using ETicaretAPI.Application.Features.Commands.AppUser.FacebookLogin;
using ETicaretAPI.Application.Features.Commands.AppUser.GoogleLogin;
using ETicaretAPI.Application.Features.Commands.AppUser.LoginUser;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login(LoginUserCommandRequest loginUserCommandRequest)
        {
            LoginUserCommandResponse coginUserCommandResponse = await _mediator.Send(loginUserCommandRequest);
            return Ok(coginUserCommandResponse);

        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin(GoogleLoginCommandRequest googleLoginCommandRequest)
        {
            GoogleLoginCommandResponse googleLoginCommandResponse = await _mediator.Send(googleLoginCommandRequest); //external bir kaynaktan gelen login işlemleri için genel bir endpoint yaratılıp(external-login) request içerisinden kaynak yakalanıp ilgili handler'a mediatR ile yönlendirilebilir.
            return Ok(googleLoginCommandResponse);
        }

        [HttpPost("facebook-login")]
        public async Task<IActionResult> FacebookLogin(FacebookLoginCommandRequest facebookLoginCommandRequest)
        {
            FacebookLoginCommandResponse facebookLoginCommandResponse = await _mediator.Send(facebookLoginCommandRequest);
            return Ok(facebookLoginCommandResponse);
        }
    }
}
