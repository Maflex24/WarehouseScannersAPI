using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagerAPI.Migrations
{
    public partial class RemovePermissionTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_PermissionsTypes_PermissionTypeId",
                table: "Permissions");

            migrationBuilder.DropTable(
                name: "PermissionsTypes");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_PermissionTypeId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "PermissionTypeId",
                table: "Permissions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PermissionTypeId",
                table: "Permissions",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_PermissionTypeId",
                table: "Permissions",
                column: "PermissionTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_PermissionsTypes_PermissionTypeId",
                table: "Permissions",
                column: "PermissionTypeId",
                principalTable: "PermissionsTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
