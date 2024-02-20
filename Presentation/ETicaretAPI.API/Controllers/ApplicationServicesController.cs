using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.Constants;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.DTOs.Configurations;
using ETicaretAPI.Application.Features.Queries.Basket.GetBasketItems;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ETicaretAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Admin")]

    public class ApplicationServicesController : ControllerBase
    {
        readonly IApplicationService _applicationService;

        public ApplicationServicesController(IApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpGet]
        [AuthorizeDefinition(Menu = AuthorizeDefinitonConstant.ApplicationService, ActionType = Application.Enums.ActionType.Reading, Definition = "Get Authorize Definition Endpoints")]
        public IActionResult GetAuthorizeDefinitionEndpoints()
        {
            List<Menu> response = _applicationService.GetAuthorizeDefinitonEndpoints(typeof(Program));
            return Ok(response);
        }
    }
}
