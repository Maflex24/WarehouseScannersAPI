using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseScannersAPI.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string FullName { get; set; }
        public string PasswordHash { get; set; }
        public List<Permission> Permissions { get; set; }
    }

    public class EmployeeConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasMany(e => e.Permissions)
                .WithMany(p => p.Accounts);
        }
    }
}
