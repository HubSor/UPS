using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class NoClientCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterOptions_Parameters_ParameterId",
                table: "ParameterOptions");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterOptions_Parameters_ParameterId",
                table: "ParameterOptions",
                column: "ParameterId",
                principalTable: "Parameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterOptions_Parameters_ParameterId",
                table: "ParameterOptions");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterOptions_Parameters_ParameterId",
                table: "ParameterOptions",
                column: "ParameterId",
                principalTable: "Parameters",
                principalColumn: "Id");
        }
    }
}
