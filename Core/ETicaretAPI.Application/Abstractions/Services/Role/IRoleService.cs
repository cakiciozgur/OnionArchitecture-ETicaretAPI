using ETicaretAPI.Domain.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Abstractions.Services.Role
{
    public interface IRoleService
    {
        Task<bool> CreateRoleAsync(string name);
        Task<bool> DeleteRoleAsync(string name);
        Task<bool> UpdateRoleAsync(string id, string name);
        IDictionary<string,string> GetAllRoles();
        Task<(string, string)> GetRoleById(string id);
    }
}
