using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WarehouseManagerAPI.Dtos;
using WarehouseManagerAPI.Services;

namespace WarehouseManagerAPI.Entities
{
    public class DataGenerator
    {
        private readonly WarehouseManagerDbContext _dbContext;
        private readonly IAccountService _accountService;

        public DataGenerator(WarehouseManagerDbContext dbContext, IAccountService accountService)
        {
            _dbContext = dbContext;
            _accountService = accountService;
        }

        public async Task GenerateSystemAdmin()
        {
            var systemAdminExist = await _dbContext.Employees
                .AnyAsync(e => e.Login == "SystemAdmin");

            if (systemAdminExist)
                return;

            var systemAdmin = new EmployeeRegisterPost()
            {
                Login = "SystemAdmin",
                Password = "SAdmin12!",
                FirstName = "System",
                LastName = "Admin"
            };

            await _accountService.AddEmployee(systemAdmin);
        }

        public async Task GeneratePermissions()
        {
            var permissions = new List<Permission>()
            {
                new Permission(){Name = "AccountCreate"},
                new Permission(){Name = "AccountActivation"},
                new Permission(){Name = "AccountDeactivation"},
                new Permission(){Name = "AccountRoleAssign"},
                new Permission(){Name = "PermissionCreate"},
                new Permission(){Name = "PermissionModify"},
                new Permission(){Name = "PermissionDelete"},
                new Permission(){Name = "PermissionAssign"},
                new Permission(){Name = "PickingPick"},
                new Permission(){Name = "PickingEdit"},
                new Permission(){Name = "QualityCheck"},
                new Permission(){Name = "QualityEdit"}
            };

            var existedPermissionsCount = _dbContext.Permissions.Count();
            if (existedPermissionsCount == permissions.Count)
                return;

            var existedPermissions = await _dbContext.Permissions.Select(p => p.Name).ToListAsync();
            foreach (var newPermission in permissions.Where(newPermissionsType => !existedPermissions.Contains(newPermissionsType.Name)))
            {
                _dbContext.Permissions.Add(newPermission);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task GenerateRoles()
        {
            var permissions = await _dbContext.Permissions.ToListAsync();

            var roles = new List<Role>()
            {
                new Role {
                    Name = "Picker Trainee", 
                    Permissions = new List<Permission>()
                    {
                         permissions.Single(p => p.Name == "PickingPick")
                    }},
                new Role {
                    Name = "Picker",
                    Permissions = new List<Permission>()
                    {
                        permissions.Single(p => p.Name == "PickingPick"),
                        permissions.Single(p => p.Name == "PickingEdit"),
                    }},
                new Role {
                    Name = "Quality Trainee",
                    Permissions = new List<Permission>()
                    {
                        permissions.Single(p => p.Name == "QualityCheck"),
                    }},
                new Role {
                    Name = "Quality",
                    Permissions = new List<Permission>()
                    {
                        permissions.Single(p => p.Name == "QualityCheck"),
                        permissions.Single(p => p.Name == "QualityEdit"),
                    }},
                new Role {
                    Name = "Account Admin",
                    Permissions = new List<Permission>()
                    {
                        permissions.Single(p => p.Name == "AccountCreate"),
                        permissions.Single(p => p.Name == "AccountActivation"),
                        permissions.Single(p => p.Name == "AccountDeactivation"),
                        permissions.Single(p => p.Name == "AccountRoleAssign"),
                    }},
                //new Role {Name = "Inbound Trainee"},
                //new Role {Name = "Inbound"},
                //new Role {Name = "Outbound Trainee"},
                //new Role {Name = "Outbound"},
                //new Role {Name = "OPS Admin Assisant"},
                //new Role {Name = "OPS Admin"},
                //new Role {Name = "Process Helper"},
                //new Role {Name = "Leader Assistant"},
                //new Role {Name = "Leader"},
            };

            var existedRolesCount = _dbContext.Roles.Count();
            if (existedRolesCount == roles.Count)
                return;

            var existedRoles = await _dbContext.Roles.Select(p => p.Name).ToListAsync();
            foreach (var newRole in roles.Where(newPermissionsType => !existedRoles.Contains(newPermissionsType.Name)))
            {
                await _dbContext.Roles.AddAsync(newRole);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
