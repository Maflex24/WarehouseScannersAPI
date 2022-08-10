using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagerAPI.Migrations
{
    public partial class UpdatedAccountAndPermissionEntities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeePermission");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "RegisteredDate",
                table: "Employees");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Employees",
                newName: "FullName");

            migrationBuilder.RenameTable(
                name: "Employees",
                newName: "Accounts");

            migrationBuilder.CreateTable(
                name: "AccountPermission",
                columns: table => new
                {
                    AccountsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountPermission", x => new { x.AccountsId, x.PermissionsId });
                    table.ForeignKey(
                        name: "FK_AccountPermission_Accounts_AccountsId",
                        column: x => x.AccountsId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountPermission_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountPermission_PermissionsId",
                table: "AccountPermission",
                column: "PermissionsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountPermission");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Accounts",
                newName: "Password");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Accounts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Accounts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredDate",
                table: "Accounts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "Employees");
                
            migrationBuilder.CreateTable(
                name: "EmployeePermission",
                columns: table => new
                {
                    EmployeesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeePermission", x => new { x.EmployeesId, x.PermissionsId });
                    table.ForeignKey(
                        name: "FK_EmployeePermission_Accounts_EmployeesId",
                        column: x => x.EmployeesId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmployeePermission_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeePermission_PermissionsId",
                table: "EmployeePermission",
                column: "PermissionsId");
        }
    }
}
