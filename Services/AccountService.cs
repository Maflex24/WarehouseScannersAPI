using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WarehouseManagerAPI.Dtos;
using WarehouseManagerAPI.Entities;
using WarehouseManagerAPI.Exceptions;

namespace WarehouseManagerAPI.Services
{
    public interface IAccountService
    {
        public Task<string> GenerateJwtToken(EmployeeLoginDto loginDto);
        public Task ChangePassword(EmployeeChangePasswordDto changePasswordDto);
        public Task<Account> AddEmployee(string login, string password, string fullName);
    }


    public class AccountService : IAccountService
    {
        private readonly WarehouseManagerDbContext _dbContext;
        private readonly IPasswordHasher<Account> _passwordHasher;
        private readonly AuthenticationSettings _authenticationSettings;
        private readonly IEmployeeContextService _employeeContextService;

        public AccountService(WarehouseManagerDbContext dbContext, IPasswordHasher<Account> passwordHasher, AuthenticationSettings authenticationSettings, IEmployeeContextService employeeContextService)
        {
            _dbContext = dbContext;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationSettings;
            _employeeContextService = employeeContextService;

        }

        public async Task<string> GenerateJwtToken(EmployeeLoginDto loginDto)
        {
            var employee = await _dbContext.Accounts
                .AsNoTracking()
                .Include(e => e.Permissions)
                .FirstOrDefaultAsync(e => e.Login == loginDto.Login);

            if (employee is null)
                throw new BadRequestException("Login or password is not valid");

            var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(employee, employee.PasswordHash, loginDto.Password);
            if (passwordVerificationResult == PasswordVerificationResult.Failed)
                throw new BadRequestException("Login or password is not valid");

            var permissions = employee.Permissions.Select(p => p.Name.ToString()).ToList();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                new Claim(ClaimTypes.GivenName, employee.Login),
                new Claim(ClaimTypes.Name, employee.FullName),
            };

            foreach (var permission in permissions)
            {
                claims.Add(new Claim(permission, true.ToString()));
            }

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

        public async Task ChangePassword(EmployeeChangePasswordDto changePasswordDto)
        {
            var user = _dbContext.Accounts
                .FirstOrDefault(e => e.Id == _employeeContextService.EmployeeId);

            var oldPasswordVerify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, changePasswordDto.OldPassword);
            if (oldPasswordVerify == PasswordVerificationResult.Failed)
                throw new InvalidPasswordException("Not valid password");

            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
                throw new InvalidPasswordException("New password, and confirmed must be the same");

            user.PasswordHash = _passwordHasher.HashPassword(user, changePasswordDto.NewPassword);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<Account> AddEmployee(string login, string password, string fullName)
        {
            var account = new Account()
            {
                Login = login,
                FullName = fullName
            };

            account.PasswordHash = _passwordHasher.HashPassword(account, password);
            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();

            return account;
        }
    }
}
