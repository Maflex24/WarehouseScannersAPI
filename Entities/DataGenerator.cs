using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Services.Services;
using WarehouseManagerAPI.Dtos;

namespace WarehouseManagerAPI.Entities
{
    public class DataGenerator
    {
        private readonly WarehouseManagerDbContext _dbContext;
        private readonly IEmployeeService _employeeService;

        public DataGenerator(WarehouseManagerDbContext dbContext, IEmployeeService employeeService)
        {
            _dbContext = dbContext;
            _employeeService = employeeService;
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

            await _employeeService.AddEmployee(systemAdmin);
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

            var permissions = new List<Permission>()
            {
                new Permission(){Name = "AccountCreate", PermissionType = accountsType},
                new Permission(){Name = "AccountActivation", PermissionType = accountsType},
                new Permission(){Name = "AccountDisable", PermissionType = accountsType},
                new Permission(){Name = "AccountRoleAssign", PermissionType = accountsType},
                new Permission(){Name = "PermissionCreate", PermissionType = permissionType},
                new Permission(){Name = "PermissionModify", PermissionType = permissionType},
                new Permission(){Name = "PermissionDelete", PermissionType = permissionType},
                new Permission(){Name = "PermissionAssign", PermissionType = permissionType}
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
    }
}
