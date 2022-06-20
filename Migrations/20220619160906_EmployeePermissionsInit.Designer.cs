﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WarehouseManagerAPI.Entities;

#nullable disable

namespace WarehouseManagerAPI.Migrations
{
    [DbContext(typeof(WarehouseManagerDbContext))]
    [Migration("20220619160906_EmployeePermissionsInit")]
    partial class EmployeePermissionsInit
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("EmployeePermission", b =>
                {
                    b.Property<Guid>("EmployeesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("PermissionsId")
                        .HasColumnType("int");

                    b.HasKey("EmployeesId", "PermissionsId");

                    b.HasIndex("PermissionsId");

                    b.ToTable("EmployeePermission");
                });

            modelBuilder.Entity("PermissionRole", b =>
                {
                    b.Property<int>("PermissionsId")
                        .HasColumnType("int");

                    b.Property<int>("RolesId")
                        .HasColumnType("int");

                    b.HasKey("PermissionsId", "RolesId");

                    b.HasIndex("RolesId");

                    b.ToTable("PermissionRole");
                });

            modelBuilder.Entity("WarehouseManagerAPI.Entities.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GenerateJwtToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RegisteredDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("WarehouseManagerAPI.Entities.Permission", b =>
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

            modelBuilder.Entity("WarehouseManagerAPI.Entities.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Picker Trainee"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Picker"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Quality Trainee"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Quality"
                        },
                        new
                        {
                            Id = 5,
                            Name = "Inbound Trainee"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Inbound"
                        },
                        new
                        {
                            Id = 7,
                            Name = "Outbound Trainee"
                        },
                        new
                        {
                            Id = 8,
                            Name = "Outbound"
                        },
                        new
                        {
                            Id = 9,
                            Name = "OPS Admin Assisant"
                        },
                        new
                        {
                            Id = 10,
                            Name = "OPS Admin"
                        },
                        new
                        {
                            Id = 11,
                            Name = "Process Helper"
                        },
                        new
                        {
                            Id = 12,
                            Name = "Leader Assistant"
                        },
                        new
                        {
                            Id = 13,
                            Name = "Leader"
                        },
                        new
                        {
                            Id = 14,
                            Name = "Supervisor Assistant"
                        },
                        new
                        {
                            Id = 15,
                            Name = "Supervisor"
                        });
                });

            modelBuilder.Entity("EmployeePermission", b =>
                {
                    b.HasOne("WarehouseManagerAPI.Entities.Employee", null)
                        .WithMany()
                        .HasForeignKey("EmployeesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarehouseManagerAPI.Entities.Permission", null)
                        .WithMany()
                        .HasForeignKey("PermissionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("PermissionRole", b =>
                {
                    b.HasOne("WarehouseManagerAPI.Entities.Permission", null)
                        .WithMany()
                        .HasForeignKey("PermissionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WarehouseManagerAPI.Entities.Role", null)
                        .WithMany()
                        .HasForeignKey("RolesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
