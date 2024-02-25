using ETicaretAPI.Application.Abstractions.Services.User;
using ETicaretAPI.Application.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;

namespace ETicaretAPI.API.Filters
{
    public class RolePermissionFilter : IAsyncActionFilter
    {
        readonly IUserService _userService;

        public RolePermissionFilter(IUserService userService)
        {
            _userService = userService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool hasRole = false;

            var name = context.HttpContext.User.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(name) && name != "cakiciozgur")
            {
                var descriptor = context.ActionDescriptor as ControllerActionDescriptor;

                AuthorizeDefinitionAttribute? authAttribute = descriptor.MethodInfo.GetCustomAttribute(typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;
                HttpMethodAttribute? httpMethodAttribute = descriptor.MethodInfo.GetCustomAttribute(typeof(HttpMethodAttribute)) as HttpMethodAttribute;

                if(authAttribute != null)
                {
                    string code = $"{authAttribute.Menu}.{(httpMethodAttribute != null ? httpMethodAttribute.HttpMethods.First() : HttpMethods.Get)}.{authAttribute.ActionType}.{authAttribute.Definition.Replace(" ","")}";
                    hasRole = await _userService.HasRolePermissionToPaEndpointAsync(name, code);
                }

                if (!hasRole)
                {
                    context.Result = new UnauthorizedResult();
                }
                else
                {
                    await next();
                }
            }
            else
            {
                await next();
            }
        }
    }
}
