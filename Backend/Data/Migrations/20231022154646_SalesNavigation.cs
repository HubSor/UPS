using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class SalesNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterOptions_Parameters_ParameterId",
                table: "ParameterOptions");

            migrationBuilder.RenameColumn(
                name: "IsRequired",
                table: "Parameters",
                newName: "Required");

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "SubProducts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Parameters",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterOptions_Parameters_ParameterId",
                table: "ParameterOptions",
                column: "ParameterId",
                principalTable: "Parameters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterOptions_Parameters_ParameterId",
                table: "ParameterOptions");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "SubProducts");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Parameters");

            migrationBuilder.RenameColumn(
                name: "Required",
                table: "Parameters",
                newName: "IsRequired");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterOptions_Parameters_ParameterId",
                table: "ParameterOptions",
                column: "ParameterId",
                principalTable: "Parameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
