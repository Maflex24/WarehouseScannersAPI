using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WarehouseManagerAPI.Entities
{
    public class Storage
    {
        public string Id { get; set; }
        public float MaxWeight { get; set; }
        public bool Temporary { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public List<StorageContent> StorageContent { get; set; }
    }

    public class StorageConfiguration : IEntityTypeConfiguration<Storage>
    {
        public void Configure(EntityTypeBuilder<Storage> builder)
        {
            builder.HasMany(s => s.StorageContent)
                .WithOne(sc => sc.Storage)
                .HasForeignKey(sc => sc.StorageId);
        }
    }
}
