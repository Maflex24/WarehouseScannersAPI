using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WarehouseScannersAPI.Entities
{
    public class WarehouseScannersDbContext : DbContext
    {
        public WarehouseScannersDbContext(DbContextOptions<WarehouseScannersDbContext> options) : base(options)
        {

        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderPosition> OrderPositions { get; set; }
        public DbSet<Storage> Storages { get; set; }
        public DbSet<StorageContent> StorageContents { get; set; }
        public DbSet<Pallet> Pallets { get; set; }
        public DbSet<PalletContent> PalletContents { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);

            
        }
    }
}
