using ETicaretAPI.Application.Abstractions.Services.Role;
using ETicaretAPI.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Persistence.Services
{
    public class RoleService : IRoleService
    {
        readonly RoleManager<AppRole> _roleManager;

        public RoleService(RoleManager<AppRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<bool> CreateRoleAsync(string name)
        {
            IdentityResult res = await _roleManager.CreateAsync(new AppRole
            {
                Name = name
            });

            return res.Succeeded;
        }

        public async Task<bool> DeleteRoleAsync(string name)
        {
            IdentityResult res = await _roleManager.DeleteAsync(new AppRole
            {
                Name = name
            });

            return res.Succeeded;
        }

        public IDictionary<string, string> GetAllRoles()
        {
            return _roleManager.Roles.ToDictionary(x => x.Id, x => x.Name);
        }

        public async Task<(string, string)> GetRoleById(string id)
        {
            string role = await _roleManager.GetRoleIdAsync(new AppRole
            {
                Id = id
            });

            return (id, role);
        }

        public async Task<bool> UpdateRoleAsync(string id, string name)
        {
            IdentityResult res = await _roleManager.UpdateAsync(new AppRole
            {
                Id = id,
                Name = name
            });

            return res.Succeeded;
        }
    }
}
