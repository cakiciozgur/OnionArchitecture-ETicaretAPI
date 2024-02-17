using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.CustomAttributes;
using ETicaretAPI.Application.DTOs.Configurations;
using ETicaretAPI.Application.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Reflection;
using Action = ETicaretAPI.Application.DTOs.Configurations.Action;

namespace ETicaretAPI.Infastructure.Services.Configurations
{
    public class ApplicationService : IApplicationService
    {
        public List<Menu> GetAuthorizeDefinitonEndpoints(Type assemblyType)
        {
            List<Menu> menus = new List<Menu>();
             
            Assembly? assembly = Assembly.GetAssembly(assemblyType);
            List<Type> controllers = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(ControllerBase))).ToList();
            if(controllers != null)
            {
                foreach (Type controller in controllers)
                {
                    var actions = controller.GetMethods().Where(m => m.IsDefined(typeof(AuthorizeDefinitionAttribute)));

                    if(actions != null)
                    {
                        foreach(var action in actions)
                        {
                            var attributes = action.GetCustomAttributes();

                            if (attributes != null)
                            {
                                Menu menu = null;
                                var authorizeDefinitionAttribute = attributes.FirstOrDefault(a => a.GetType() == typeof(AuthorizeDefinitionAttribute)) as AuthorizeDefinitionAttribute;
                                var httpMethodAttribute = attributes.FirstOrDefault(a => a.GetType().IsAssignableTo(typeof(HttpMethodAttribute))) as HttpMethodAttribute;
                                
                                if (!menus.Exists(m => m.Name == authorizeDefinitionAttribute?.Menu))
                                {
                                    menu = new Menu { Name = authorizeDefinitionAttribute.Menu };
                                    menus.Add(menu);
                                }
                                else
                                {
                                    menu = menus.FirstOrDefault(m => m.Name == authorizeDefinitionAttribute?.Menu);
                                }

                                Action _action = new Action
                                {
                                    ActionType = Enum.GetName(typeof(ActionType),authorizeDefinitionAttribute?.ActionType),
                                    Definition = authorizeDefinitionAttribute.Definition,
                                };

                                if(httpMethodAttribute != null)
                                {
                                    _action.HttpType = httpMethodAttribute.HttpMethods.First();
                                } 
                                else
                                {
                                    _action.HttpType = HttpMethods.Get;
                                }

                                _action.Code = $"{menu.Name}.{_action.HttpType}.{_action.ActionType}.{_action.Definition.Replace(" ","")}";
                                
                                menu.Actions.Add(_action);
                            }
                        }
                    }
                }
            }

            return menus;
        }
    }
}
