﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WarehouseScannersAPI.Entities;

#nullable disable

namespace WarehouseScannersAPI.Migrations
{
    [DbContext(typeof(WarehouseScannersDbContext))]
    partial class WarehouseScannersDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("AccountPermission", b =>
                {
                    b.Property<Guid>("AccountsId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("PermissionsId")
                        .HasColumnType("int");

                    b.HasKey("AccountsId", "PermissionsId");

                    b.HasIndex("PermissionsId");

                    b.ToTable("AccountPermission");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Login")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.Order", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.OrderPosition", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<bool>("Completed")
                        .HasColumnType("bit");

                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("PickedQty")
                        .HasColumnType("int");

                    b.Property<string>("ProductId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Qty")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderPositions");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.Pallet", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Depth")
                        .HasColumnType("int");

                    b.Property<int>("Height")
                        .HasColumnType("int");

                    b.Property<string>("OrderId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<int>("Width")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.ToTable("Pallets");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.PalletContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("PalletId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProductId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Qty")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PalletId");

                    b.HasIndex("ProductId");

                    b.ToTable("PalletContents");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Permissions");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.Product", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Depth")
                        .HasColumnType("int");

                    b.Property<int>("Height")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float>("Volume")
                        .HasColumnType("real");

                    b.Property<float>("Weight")
                        .HasColumnType("real");

                    b.Property<int>("Width")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.Storage", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Depth")
                        .HasColumnType("int");

                    b.Property<int>("Height")
                        .HasColumnType("int");

                    b.Property<float>("MaxWeight")
                        .HasColumnType("real");

                    b.Property<bool>("Temporary")
                        .HasColumnType("bit");

                    b.Property<int>("Width")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Storages");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.StorageContent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ProductId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Qty")
                        .HasColumnType("int");

                    b.Property<string>("StorageId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("ProductId");

                    b.HasIndex("StorageId");

                    b.ToTable("StorageContents");
                });

            modelBuilder.Entity("AccountPermission", b =>
                {
                    b.HasOne("WarehouseScannersAPI.Entities.Account", null)
                        .WithMany()
                        .HasForeignKey("AccountsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarehouseScannersAPI.Entities.Permission", null)
                        .WithMany()
                        .HasForeignKey("PermissionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.OrderPosition", b =>
                {
                    b.HasOne("WarehouseScannersAPI.Entities.Order", "Order")
                        .WithMany("OrderPositions")
                        .HasForeignKey("OrderId");

                    b.HasOne("WarehouseScannersAPI.Entities.Product", "Product")
                        .WithMany("OrderPositions")
                        .HasForeignKey("ProductId");

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.Pallet", b =>
                {
                    b.HasOne("WarehouseScannersAPI.Entities.Order", "Order")
                        .WithMany()
                        .HasForeignKey("OrderId");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.PalletContent", b =>
                {
                    b.HasOne("WarehouseScannersAPI.Entities.Pallet", "Pallet")
                        .WithMany("PalletContent")
                        .HasForeignKey("PalletId");

                    b.HasOne("WarehouseScannersAPI.Entities.Product", "Product")
                        .WithMany("PalletContent")
                        .HasForeignKey("ProductId");

                    b.Navigation("Pallet");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.StorageContent", b =>
                {
                    b.HasOne("WarehouseScannersAPI.Entities.Product", "Product")
                        .WithMany("StorageContent")
                        .HasForeignKey("ProductId");

                    b.HasOne("WarehouseScannersAPI.Entities.Storage", "Storage")
                        .WithMany("StorageContent")
                        .HasForeignKey("StorageId");

                    b.Navigation("Product");

                    b.Navigation("Storage");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.Order", b =>
                {
                    b.Navigation("OrderPositions");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.Pallet", b =>
                {
                    b.Navigation("PalletContent");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.Product", b =>
                {
                    b.Navigation("OrderPositions");

                    b.Navigation("PalletContent");

                    b.Navigation("StorageContent");
                });

            modelBuilder.Entity("WarehouseScannersAPI.Entities.Storage", b =>
                {
                    b.Navigation("StorageContent");
                });
#pragma warning restore 612, 618
        }
    }
}
