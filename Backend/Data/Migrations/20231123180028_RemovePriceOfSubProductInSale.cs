using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovePriceOfSubProductInSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InSalePrice",
                table: "SubProductsInSales");

            migrationBuilder.DropColumn(
                name: "ManualOverride",
                table: "SubProductsInSales");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "InSalePrice",
                table: "SubProductsInSales",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "ManualOverride",
                table: "SubProductsInSales",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
