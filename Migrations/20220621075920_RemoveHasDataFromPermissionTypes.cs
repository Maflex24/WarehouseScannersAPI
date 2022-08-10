using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagerAPI.Migrations
{
    public partial class RemoveHasDataFromPermissionTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PermissionsTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PermissionsTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "PermissionsTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PermissionsTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "PermissionsTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "PermissionsTypes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "PermissionsTypes",
                keyColumn: "Id",
                keyValue: 7);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
        }
    }
}
