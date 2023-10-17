using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class SubProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Products",
                newName: "BasePrice");

            migrationBuilder.CreateTable(
                name: "SubProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    BasePrice = table.Column<decimal>(type: "money", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubProductsInProducts",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    SubProductId = table.Column<int>(type: "integer", nullable: false),
                    InProductPrice = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubProductsInProducts", x => new { x.ProductId, x.SubProductId });
                    table.ForeignKey(
                        name: "FK_SubProductsInProducts_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubProductsInProducts_SubProducts_SubProductId",
                        column: x => x.SubProductId,
                        principalTable: "SubProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubProductsInProducts_SubProductId",
                table: "SubProductsInProducts",
                column: "SubProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubProductsInProducts");

            migrationBuilder.DropTable(
                name: "SubProducts");

            migrationBuilder.RenameColumn(
                name: "BasePrice",
                table: "Products",
                newName: "Price");
        }
    }
}
