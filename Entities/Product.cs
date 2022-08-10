using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseManagerAPI.Entities
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public float Weight { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public virtual List<OrderPosition> OrderPositions { get; set; }
        public virtual List<StorageContent> StorageContent { get; set; }
        public virtual List<PalletContent> PalletContent { get; set; }
    }

    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasMany(p => p.OrderPositions)
                .WithOne(op => op.Product)
                .HasForeignKey(op => op.ProductId);

            builder.HasMany(p => p.StorageContent)
                .WithOne(sc => sc.Product)
                .HasForeignKey(sc => sc.ProductId);

            builder.HasMany(p => p.PalletContent)
                .WithOne(pt => pt.Product)
                .HasForeignKey(pt => pt.ProductId);
        }
    }
}
