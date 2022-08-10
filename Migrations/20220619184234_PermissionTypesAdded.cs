using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagerAPI.Migrations
{
    public partial class PermissionTypesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PermissionTypeId",
                table: "Permissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EmployeeRole",
                columns: table => new
                {
                    EmployeesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RolesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeRole", x => new { x.EmployeesId, x.RolesId });
                    table.ForeignKey(
                        name: "FK_EmployeeRole_Employees_EmployeesId",
                        column: x => x.EmployeesId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeeRole_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PermissionsTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionsTypes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "PermissionsTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Accounts" },
                    { 2, "Orders" },
                    { 3, "Picking" },
                    { 4, "Quality" },
                    { 5, "Inbound" },
                    { 6, "Outbound" },
                    { 7, "Accounts" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_PermissionTypeId",
                table: "Permissions",
                column: "PermissionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRole_RolesId",
                table: "EmployeeRole",
                column: "RolesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_PermissionsTypes_PermissionTypeId",
                table: "Permissions",
                column: "PermissionTypeId",
                principalTable: "PermissionsTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_PermissionsTypes_PermissionTypeId",
                table: "Permissions");

            migrationBuilder.DropTable(
                name: "EmployeeRole");

            migrationBuilder.DropTable(
                name: "PermissionsTypes");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_PermissionTypeId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "PermissionTypeId",
                table: "Permissions");
        }
    }
}
