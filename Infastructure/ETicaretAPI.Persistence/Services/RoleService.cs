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
                Id = Guid.NewGuid().ToString(),
                Name = name
            });

            return res.Succeeded;
        }

        public async Task<bool> DeleteRoleAsync(string id)
        {
            AppRole role = await _roleManager.FindByIdAsync(id);
            IdentityResult res = await _roleManager.DeleteAsync(role);

            return res.Succeeded;
        }

        public (IDictionary<string, string>,int) GetAllRoles(int page, int size)
        {
            var query = _roleManager.Roles;
            IQueryable<AppRole>? rolesQuery = null;

            if(page != -1 && size != -1)
            {
                rolesQuery = query.Skip(page * size).Take(size);
            }
            else
            {
                rolesQuery = query;
            }

            var roleDict = rolesQuery.ToDictionary(x => x.Id, x => x.Name);
            var totalRoleCount = query.Count();

            return (roleDict, totalRoleCount);
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
            AppRole role = await _roleManager.FindByIdAsync(id);
            role.Name = name;
            IdentityResult res = await _roleManager.UpdateAsync(role);

            return res.Succeeded;
        }
    }
}
