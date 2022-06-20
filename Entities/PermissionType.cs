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
            builder.HasData(
                new PermissionType {Id = 1, Name = "Accounts"},
                new PermissionType {Id = 2, Name = "Orders"},
                new PermissionType {Id = 3, Name = "Picking"},
                new PermissionType {Id = 4, Name = "Quality"},
                new PermissionType {Id = 5, Name = "Inbound"},
                new PermissionType {Id = 6, Name = "Outbound"},
                new PermissionType {Id = 7, Name = "Employees"}
            );
        }
    }
}
