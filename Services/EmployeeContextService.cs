using System.Security.Claims;

namespace WarehouseScannersAPI.Services
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
