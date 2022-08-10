using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagerAPI.Migrations
{
    public partial class AddPalletAndPalletContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pallets",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Weight = table.Column<float>(type: "real", nullable: false),
                    Width = table.Column<int>(type: "int", nullable: false),
                    Height = table.Column<int>(type: "int", nullable: false),
                    Depth = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pallets_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PalletContents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Qty = table.Column<int>(type: "int", nullable: false),
                    PalletId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PalletContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PalletContents_Pallets_PalletId",
                        column: x => x.PalletId,
                        principalTable: "Pallets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PalletContents_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PalletContents_PalletId",
                table: "PalletContents",
                column: "PalletId");

            migrationBuilder.CreateIndex(
                name: "IX_PalletContents_ProductId",
                table: "PalletContents",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Pallets_OrderId",
                table: "Pallets",
                column: "OrderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PalletContents");

            migrationBuilder.DropTable(
                name: "Pallets");
        }
    }
}
