using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WarehouseManagerAPI.Entities;

namespace WarehouseManagerAPI.Services
{
    public interface IEmployeeContextService
    {
        ClaimsPrincipal Employee { get; }
        Guid? EmployeeId { get; }
    }

    public class EmployeeContextService : IEmployeeContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal Employee => _httpContextAccessor.HttpContext?.User;

        public Guid? EmployeeId => Employee is null
            ? null
            : Guid.Parse(Employee.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
    }
}
