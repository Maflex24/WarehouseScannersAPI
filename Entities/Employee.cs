using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseManagerAPI.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; } // Entity just for easier testing, with fake data! 
        public string PasswordHash { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegisteredDate { get; set; }
        public bool IsActive { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<Role> Roles { get; set; }
    }

    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasMany(e => e.Permissions)
                .WithMany(p => p.Employees);

            builder.HasMany(e => e.Roles)
                .WithMany(r => r.Employees);
        }
    }
}
