using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WarehouseManagerAPI;
using WarehouseManagerAPI.Dtos;
using WarehouseManagerAPI.Entities;
using WarehouseManagerAPI.Exceptions;

namespace Services.Services
{
    public interface IEmployeeService
    {
        public Task<Employee> AddEmployee(EmployeeRegisterPost employeeDto);
        public Task<string> GenerateJwtToken(EmployeeLoginDto loginDto);
    }


    public class EmployeeService : IEmployeeService
    {
        private readonly WarehouseManagerDbContext _dbContext;
        private readonly IPasswordHasher<Employee> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;

        public EmployeeService(WarehouseManagerDbContext dbContext, IPasswordHasher<Employee> passwordHasher, AuthenticationSettings authenticationSettings)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
        }

        public async Task<Employee> AddEmployee(EmployeeRegisterPost employeeDto)
        {
            var employee = new Employee()
            {
                Login = employeeDto.Login,
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Password = employeeDto.Password, // todo change later
                RegisteredDate = DateTime.Now,
                IsActive = true
            };

            employee.PasswordHash = _passwordHasher.HashPassword(employee, employeeDto.Password);
            await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync();

            return employee;
        }

        public async Task<string> GenerateJwtToken(EmployeeLoginDto loginDto)
        {
            var employee = await _dbContext.Employees
                .AsNoTracking()
                .Include(e => e.Roles)
                .Include(e => e.Permissions)
                .FirstOrDefaultAsync(e => e.Login == loginDto.Login);

            if (employee is null || !employee.IsActive)
                throw new BadRequestException("Login or password is not valid");

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(employee, employee.PasswordHash, loginDto.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
                throw new BadRequestException("Login or password is not valid");

            var roles = employee.Roles.Select(role => role.Id.ToString()).ToList();
            var permissions = employee.Permissions.Select(p => p.Id.ToString()).ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.GivenName, employee.Login),
                new Claim(ClaimTypes.Name, $"{employee.FirstName} {employee.LastName}"),
                new Claim("Roles", string.Join("-", roles)) 
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddHours(_authenticationSettings.JwtExpireHours);

            var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer, _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
    }
}
