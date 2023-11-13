using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class OnDeleteSetNullSalesParametersOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleParameters_ParameterOptions_OptionId",
                table: "SaleParameters");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleParameters_ParameterOptions_OptionId",
                table: "SaleParameters",
                column: "OptionId",
                principalTable: "ParameterOptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleParameters_ParameterOptions_OptionId",
                table: "SaleParameters");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleParameters_ParameterOptions_OptionId",
                table: "SaleParameters",
                column: "OptionId",
                principalTable: "ParameterOptions",
                principalColumn: "Id");
        }
    }
}
