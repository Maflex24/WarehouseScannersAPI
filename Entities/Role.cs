using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseManagerAPI.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Permission> Permissions { get; set; }
        public List<Employee> Employees { get; set; }
    }

    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasMany(r => r.Permissions)
                .WithMany(p => p.Roles);

            builder.HasData(
                new Role {Id = 1, Name = "Picker Trainee"},
                new Role {Id = 2, Name = "Picker" },
                new Role {Id = 3, Name = "Quality Trainee" },
                new Role {Id = 4, Name = "Quality" },
                new Role {Id = 5, Name = "Inbound Trainee" },
                new Role {Id = 6, Name = "Inbound" },
                new Role {Id = 7, Name = "Outbound Trainee" },
                new Role {Id = 8, Name = "Outbound" },
                new Role {Id = 9, Name = "OPS Admin Assisant" },
                new Role {Id = 10, Name = "OPS Admin" },
                new Role {Id = 11, Name = "Process Helper" },
                new Role {Id = 12, Name = "Leader Assistant" },
                new Role {Id = 13, Name = "Leader" },
                new Role {Id = 14, Name = "Supervisor Assistant" },
                new Role {Id = 15, Name = "Supervisor" }
            );
        }
    }
}
