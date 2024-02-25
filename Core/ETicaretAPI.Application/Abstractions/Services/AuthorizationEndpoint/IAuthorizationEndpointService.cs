using ETicaretAPI.Application.DTOs.AuthorizationEndpoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services.AuthorizationEndpoint
{
    public interface IAuthorizationEndpointService
    {
        public Task AssignRoleEndpointAsync(string[] roles, string menu, string code, Type type);
        public Task<List<string>> GetRolesToEndpoint(string code, string menu);
    }
}
