using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseManagerAPI.Entities
{
    public class PermissionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Permission> Permissions { get; set; }
    }

    public class PermissionTypeConfiguration : IEntityTypeConfiguration<PermissionType>
    {
        public void Configure(EntityTypeBuilder<PermissionType> builder)
        {
            
        }
    }
}
