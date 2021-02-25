using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.Persistence.Database.Migrations
{
    public partial class Initialize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Catalog");

            migrationBuilder.CreateTable(
                name: "Products",
                schema: "Catalog",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                });

            migrationBuilder.CreateTable(
                name: "Stocks",
                schema: "Catalog",
                columns: table => new
                {
                    ProductInStockId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.ProductInStockId);
                    table.ForeignKey(
                        name: "FK_Stocks_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "Catalog",
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "Catalog",
                table: "Products",
                columns: new[] { "ProductId", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Description for product 1", "Product 1", 361m },
                    { 73, "Description for product 73", "Product 73", 352m },
                    { 72, "Description for product 72", "Product 72", 233m },
                    { 71, "Description for product 71", "Product 71", 517m },
                    { 70, "Description for product 70", "Product 70", 101m },
                    { 69, "Description for product 69", "Product 69", 942m },
                    { 68, "Description for product 68", "Product 68", 916m },
                    { 67, "Description for product 67", "Product 67", 270m },
                    { 66, "Description for product 66", "Product 66", 318m },
                    { 65, "Description for product 65", "Product 65", 907m },
                    { 64, "Description for product 64", "Product 64", 128m },
                    { 63, "Description for product 63", "Product 63", 962m },
                    { 62, "Description for product 62", "Product 62", 715m },
                    { 61, "Description for product 61", "Product 61", 670m },
                    { 60, "Description for product 60", "Product 60", 665m },
                    { 59, "Description for product 59", "Product 59", 263m },
                    { 58, "Description for product 58", "Product 58", 900m },
                    { 57, "Description for product 57", "Product 57", 153m },
                    { 56, "Description for product 56", "Product 56", 942m },
                    { 55, "Description for product 55", "Product 55", 360m },
                    { 54, "Description for product 54", "Product 54", 985m },
                    { 53, "Description for product 53", "Product 53", 239m },
                    { 74, "Description for product 74", "Product 74", 774m },
                    { 52, "Description for product 52", "Product 52", 845m },
                    { 75, "Description for product 75", "Product 75", 350m },
                    { 77, "Description for product 77", "Product 77", 776m },
                    { 98, "Description for product 98", "Product 98", 613m },
                    { 97, "Description for product 97", "Product 97", 895m },
                    { 96, "Description for product 96", "Product 96", 886m },
                    { 95, "Description for product 95", "Product 95", 579m },
                    { 94, "Description for product 94", "Product 94", 649m },
                    { 93, "Description for product 93", "Product 93", 839m },
                    { 92, "Description for product 92", "Product 92", 873m },
                    { 91, "Description for product 91", "Product 91", 427m },
                    { 90, "Description for product 90", "Product 90", 601m },
                    { 89, "Description for product 89", "Product 89", 732m },
                    { 88, "Description for product 88", "Product 88", 692m },
                    { 87, "Description for product 87", "Product 87", 802m },
                    { 86, "Description for product 86", "Product 86", 405m },
                    { 85, "Description for product 85", "Product 85", 540m },
                    { 84, "Description for product 84", "Product 84", 963m },
                    { 83, "Description for product 83", "Product 83", 441m }
                });

            migrationBuilder.InsertData(
                schema: "Catalog",
                table: "Products",
                columns: new[] { "ProductId", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { 82, "Description for product 82", "Product 82", 675m },
                    { 81, "Description for product 81", "Product 81", 854m },
                    { 80, "Description for product 80", "Product 80", 466m },
                    { 79, "Description for product 79", "Product 79", 189m },
                    { 78, "Description for product 78", "Product 78", 525m },
                    { 76, "Description for product 76", "Product 76", 265m },
                    { 51, "Description for product 51", "Product 51", 297m },
                    { 50, "Description for product 50", "Product 50", 417m },
                    { 49, "Description for product 49", "Product 49", 216m },
                    { 22, "Description for product 22", "Product 22", 419m },
                    { 21, "Description for product 21", "Product 21", 123m },
                    { 20, "Description for product 20", "Product 20", 852m },
                    { 19, "Description for product 19", "Product 19", 924m },
                    { 18, "Description for product 18", "Product 18", 567m },
                    { 17, "Description for product 17", "Product 17", 237m },
                    { 16, "Description for product 16", "Product 16", 291m },
                    { 15, "Description for product 15", "Product 15", 481m },
                    { 14, "Description for product 14", "Product 14", 486m },
                    { 13, "Description for product 13", "Product 13", 909m },
                    { 12, "Description for product 12", "Product 12", 848m },
                    { 11, "Description for product 11", "Product 11", 260m },
                    { 10, "Description for product 10", "Product 10", 158m },
                    { 9, "Description for product 9", "Product 9", 766m },
                    { 8, "Description for product 8", "Product 8", 343m },
                    { 7, "Description for product 7", "Product 7", 273m },
                    { 6, "Description for product 6", "Product 6", 839m },
                    { 5, "Description for product 5", "Product 5", 776m },
                    { 4, "Description for product 4", "Product 4", 958m },
                    { 3, "Description for product 3", "Product 3", 196m },
                    { 2, "Description for product 2", "Product 2", 382m },
                    { 23, "Description for product 23", "Product 23", 328m },
                    { 24, "Description for product 24", "Product 24", 296m },
                    { 25, "Description for product 25", "Product 25", 794m },
                    { 26, "Description for product 26", "Product 26", 211m },
                    { 48, "Description for product 48", "Product 48", 593m },
                    { 47, "Description for product 47", "Product 47", 356m },
                    { 46, "Description for product 46", "Product 46", 427m },
                    { 45, "Description for product 45", "Product 45", 206m },
                    { 44, "Description for product 44", "Product 44", 803m },
                    { 43, "Description for product 43", "Product 43", 642m },
                    { 42, "Description for product 42", "Product 42", 857m },
                    { 41, "Description for product 41", "Product 41", 601m }
                });

            migrationBuilder.InsertData(
                schema: "Catalog",
                table: "Products",
                columns: new[] { "ProductId", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { 40, "Description for product 40", "Product 40", 249m },
                    { 39, "Description for product 39", "Product 39", 315m },
                    { 99, "Description for product 99", "Product 99", 896m },
                    { 38, "Description for product 38", "Product 38", 103m },
                    { 36, "Description for product 36", "Product 36", 480m },
                    { 35, "Description for product 35", "Product 35", 544m },
                    { 34, "Description for product 34", "Product 34", 992m },
                    { 33, "Description for product 33", "Product 33", 678m },
                    { 32, "Description for product 32", "Product 32", 941m },
                    { 31, "Description for product 31", "Product 31", 250m },
                    { 30, "Description for product 30", "Product 30", 737m },
                    { 29, "Description for product 29", "Product 29", 381m },
                    { 28, "Description for product 28", "Product 28", 557m },
                    { 27, "Description for product 27", "Product 27", 269m },
                    { 37, "Description for product 37", "Product 37", 773m },
                    { 100, "Description for product 100", "Product 100", 887m }
                });

            migrationBuilder.InsertData(
                schema: "Catalog",
                table: "Stocks",
                columns: new[] { "ProductInStockId", "ProductId", "Stock" },
                values: new object[,]
                {
                    { 1, 1, 15 },
                    { 73, 73, 3 },
                    { 72, 72, 0 },
                    { 71, 71, 4 },
                    { 70, 70, 2 },
                    { 69, 69, 8 },
                    { 68, 68, 7 },
                    { 67, 67, 13 },
                    { 66, 66, 7 },
                    { 65, 65, 17 },
                    { 64, 64, 18 },
                    { 63, 63, 4 },
                    { 62, 62, 10 },
                    { 61, 61, 4 },
                    { 60, 60, 0 },
                    { 59, 59, 11 },
                    { 58, 58, 15 },
                    { 57, 57, 2 },
                    { 56, 56, 10 },
                    { 55, 55, 12 },
                    { 54, 54, 10 },
                    { 53, 53, 17 },
                    { 74, 74, 14 },
                    { 52, 52, 17 },
                    { 75, 75, 4 },
                    { 77, 77, 1 },
                    { 98, 98, 4 },
                    { 97, 97, 2 },
                    { 96, 96, 17 },
                    { 95, 95, 18 },
                    { 94, 94, 3 },
                    { 93, 93, 14 },
                    { 92, 92, 1 },
                    { 91, 91, 9 },
                    { 90, 90, 3 },
                    { 89, 89, 5 },
                    { 88, 88, 17 },
                    { 87, 87, 7 },
                    { 86, 86, 14 },
                    { 85, 85, 11 },
                    { 84, 84, 0 },
                    { 83, 83, 19 }
                });

            migrationBuilder.InsertData(
                schema: "Catalog",
                table: "Stocks",
                columns: new[] { "ProductInStockId", "ProductId", "Stock" },
                values: new object[,]
                {
                    { 82, 82, 18 },
                    { 81, 81, 2 },
                    { 80, 80, 15 },
                    { 79, 79, 1 },
                    { 78, 78, 5 },
                    { 76, 76, 7 },
                    { 51, 51, 7 },
                    { 50, 50, 0 },
                    { 49, 49, 14 },
                    { 22, 22, 4 },
                    { 21, 21, 14 },
                    { 20, 20, 13 },
                    { 19, 19, 17 },
                    { 18, 18, 7 },
                    { 17, 17, 6 },
                    { 16, 16, 1 },
                    { 15, 15, 14 },
                    { 14, 14, 2 },
                    { 13, 13, 4 },
                    { 12, 12, 1 },
                    { 11, 11, 11 },
                    { 10, 10, 8 },
                    { 9, 9, 17 },
                    { 8, 8, 5 },
                    { 7, 7, 5 },
                    { 6, 6, 13 },
                    { 5, 5, 1 },
                    { 4, 4, 14 },
                    { 3, 3, 4 },
                    { 2, 2, 12 },
                    { 23, 23, 9 },
                    { 24, 24, 9 },
                    { 25, 25, 3 },
                    { 26, 26, 1 },
                    { 48, 48, 10 },
                    { 47, 47, 19 },
                    { 46, 46, 7 },
                    { 45, 45, 8 },
                    { 44, 44, 4 },
                    { 43, 43, 3 },
                    { 42, 42, 6 },
                    { 41, 41, 8 }
                });

            migrationBuilder.InsertData(
                schema: "Catalog",
                table: "Stocks",
                columns: new[] { "ProductInStockId", "ProductId", "Stock" },
                values: new object[,]
                {
                    { 40, 40, 2 },
                    { 39, 39, 4 },
                    { 99, 99, 6 },
                    { 38, 38, 5 },
                    { 36, 36, 12 },
                    { 35, 35, 13 },
                    { 34, 34, 17 },
                    { 33, 33, 7 },
                    { 32, 32, 4 },
                    { 31, 31, 11 },
                    { 30, 30, 18 },
                    { 29, 29, 0 },
                    { 28, 28, 14 },
                    { 27, 27, 9 },
                    { 37, 37, 16 },
                    { 100, 100, 0 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ProductId",
                schema: "Catalog",
                table: "Stocks",
                column: "ProductId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stocks",
                schema: "Catalog");

            migrationBuilder.DropTable(
                name: "Products",
                schema: "Catalog");
        }
    }
}
