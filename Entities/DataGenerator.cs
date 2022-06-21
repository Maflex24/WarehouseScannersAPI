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


        public async Task GeneratePermissionsTypes()
        {

            var newPermissionsTypes = new List<PermissionType>()
            {
                new PermissionType {Name = "Accounts"},
                new PermissionType {Name = "Permissions"},
                new PermissionType {Name = "Employees"},
                new PermissionType {Name = "Orders"},
                new PermissionType {Name = "Picking"},
                new PermissionType {Name = "Quality"},
                new PermissionType {Name = "Inbound"},
                new PermissionType {Name = "Outbound"},
                new PermissionType {Name = "Storage"},
                new PermissionType {Name = "Products"},
            };

            var dbPermissionTypesCount = _dbContext.PermissionsTypes.Count();
            if (dbPermissionTypesCount == newPermissionsTypes.Count)
                return;

            var permissionTypes = await _dbContext.PermissionsTypes.Select(pt => pt.Name).ToListAsync();
            foreach (var newPermissionsType in newPermissionsTypes.Where(newPermissionsType => !permissionTypes.Contains(newPermissionsType.Name)))
            {
                _dbContext.PermissionsTypes.Add(newPermissionsType);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task GeneratePermissions()
        {
            var permissionTypes = await _dbContext.PermissionsTypes.ToListAsync();

            var accountsType = permissionTypes.First(pt => pt.Name == "Accounts");
            var permissionType = permissionTypes.First(pt => pt.Name == "Permissions");
            var pickingType = permissionTypes.First(pt => pt.Name == "Picking");
            var qualityType = permissionTypes.First(pt => pt.Name == "Quality");

            var permissions = new List<Permission>()
            {
                new Permission(){Name = "AccountCreate", PermissionType = accountsType},
                new Permission(){Name = "AccountActivation", PermissionType = accountsType},
                new Permission(){Name = "AccountDeactivation", PermissionType = accountsType},
                new Permission(){Name = "AccountRoleAssign", PermissionType = accountsType},
                new Permission(){Name = "PermissionCreate", PermissionType = permissionType},
                new Permission(){Name = "PermissionModify", PermissionType = permissionType},
                new Permission(){Name = "PermissionDelete", PermissionType = permissionType},
                new Permission(){Name = "PermissionAssign", PermissionType = permissionType},
                new Permission(){Name = "PickingPick", PermissionType = pickingType},
                new Permission(){Name = "PickingEdit", PermissionType = pickingType},
                new Permission(){Name = "QualityCheck", PermissionType = qualityType},
                new Permission(){Name = "QualityEdit", PermissionType = qualityType},
            };

            var existedPermissionsCount = _dbContext.PermissionsTypes.Count();
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
                //new Role {Name = "Inbound Trainee"},
                //new Role {Name = "Inbound"},
                //new Role {Name = "Outbound Trainee"},
                //new Role {Name = "Outbound"},
                //new Role {Name = "OPS Admin Assisant"},
                //new Role {Name = "OPS Admin"},
                //new Role {Name = "Process Helper"},
                //new Role {Name = "Leader Assistant"},
                //new Role {Name = "Leader"},
                new Role {Name = "Supervisor Assistant"},
                new Role {Name = "Supervisor"},
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
