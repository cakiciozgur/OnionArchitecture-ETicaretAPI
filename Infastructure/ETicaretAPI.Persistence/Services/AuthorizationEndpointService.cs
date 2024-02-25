using ETicaretAPI.Application.Abstractions.Services.AuthorizationEndpoint;
using ETicaretAPI.Application.Abstractions.Services.Configurations;
using ETicaretAPI.Application.DTOs.AuthorizationEndpoint;
using ETicaretAPI.Application.Repositories;
using ETicaretAPI.Domain.Entities;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class AuthorizationEndpointService : IAuthorizationEndpointService
    {
        readonly IApplicationService _applicationService;
        readonly IEndpointReadRepository _endpointReadRepository;
        readonly IEndpointWriteRepository _endpointWriteRepository;
        readonly IMenuReadRepository _menuReadRepository;
        readonly IMenuWriteRepository _menuWriteRepository;
        readonly RoleManager<AppRole> _roleManager;
        public AuthorizationEndpointService(IApplicationService applicationService, IEndpointReadRepository endpointReadRepository, IEndpointWriteRepository endpointWriteRepository, IMenuReadRepository menuReadRepository, IMenuWriteRepository menuWriteRepository, RoleManager<AppRole> roleManager)
        {
            _applicationService = applicationService;
            _endpointReadRepository = endpointReadRepository;
            _endpointWriteRepository = endpointWriteRepository;
            _menuReadRepository = menuReadRepository;
            _menuWriteRepository = menuWriteRepository;
            _roleManager = roleManager;
        }

        public async Task AssignRoleEndpointAsync(string[] roles, string menu, string code, Type type)
        {
            Menu? _menu = await _menuReadRepository.GetSingleAsync(x=> x.Name == menu);
            if(_menu == null)
            {
                _menu = new Menu 
                { 
                    Name = menu, 
                    Id = Guid.NewGuid()
                };
                await _menuWriteRepository.AddAsync(_menu);
                await _menuWriteRepository.SaveAsync();
            }


            Endpoint? endpoint = await _endpointReadRepository.Table.Include(x => x.Menu).Include(e=> e.Roles).FirstOrDefaultAsync(x => x.Code == code && x.Menu.Name == menu);
            if (endpoint == null)
            {
                var action = _applicationService.GetAuthorizeDefinitonEndpoints(type)
                    .FirstOrDefault(m => m.Name == menu)?
                    .Actions.FirstOrDefault(x => x.Code == code);

                if(action != null)
                {
                    endpoint = new Endpoint
                    {
                        Code = action.Code,
                        ActionType = action.ActionType,
                        HttpType = action.HttpType,
                        Definition = action.Definition,
                        Id = Guid.NewGuid(),
                        Menu = _menu
                    };
                    await _endpointWriteRepository.AddAsync(endpoint);
                }
                else
                {
                    throw new Exception("Endpoint Bulunamadı!");
                }
                await _endpointWriteRepository.SaveAsync();
            }

            foreach (var role in endpoint.Roles)
            {
                endpoint.Roles.Remove(role);
            }

            List<AppRole>? _roles = _roleManager.Roles.Where(x => roles.Contains(x.Id)).ToList();
            foreach (var role in _roles)
            {
                endpoint.Roles.Add(role);
            }
            await _endpointWriteRepository.SaveAsync();
        }

        public async Task<List<string>> GetRolesToEndpoint(string code, string menu)
        {
            Endpoint? endpoint = await _endpointReadRepository.Table.Include(e => e.Roles).Include(d=> d.Menu).FirstOrDefaultAsync(e => e.Code == code && e.Menu.Name == menu);
            if(endpoint != null)
            {
                return endpoint.Roles.Select(x => x.Name).ToList();
            }

            return null;
        }
    }
}
