using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseManagerAPI.Entities
{
    public class Pallet
    {
        public string Id { get; set; }
        public float Weight { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }

        public string? OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public List<PalletContent> PalletContent { get; set; }
    }

    public class PalletConfiguration : IEntityTypeConfiguration<Pallet>
    {
        public void Configure(EntityTypeBuilder<Pallet> builder)
        {
            builder.HasMany(p => p.PalletContent)
                .WithOne(pc => pc.Pallet)
                .HasForeignKey(pc => pc.PalletId);
        }
    }
}
