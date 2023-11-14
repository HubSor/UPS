using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ClientAddresses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientAddress_AddressTypes_TypeObjectId",
                table: "ClientAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientAddress_Clients_ClientId",
                table: "ClientAddress");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientAddress",
                table: "ClientAddress");

            migrationBuilder.RenameTable(
                name: "ClientAddress",
                newName: "ClientAddresses");

            migrationBuilder.RenameIndex(
                name: "IX_ClientAddress_TypeObjectId",
                table: "ClientAddresses",
                newName: "IX_ClientAddresses_TypeObjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientAddress_ClientId",
                table: "ClientAddresses",
                newName: "IX_ClientAddresses_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientAddresses",
                table: "ClientAddresses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientAddresses_AddressTypes_TypeObjectId",
                table: "ClientAddresses",
                column: "TypeObjectId",
                principalTable: "AddressTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientAddresses_Clients_ClientId",
                table: "ClientAddresses",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientAddresses_AddressTypes_TypeObjectId",
                table: "ClientAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientAddresses_Clients_ClientId",
                table: "ClientAddresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientAddresses",
                table: "ClientAddresses");

            migrationBuilder.RenameTable(
                name: "ClientAddresses",
                newName: "ClientAddress");

            migrationBuilder.RenameIndex(
                name: "IX_ClientAddresses_TypeObjectId",
                table: "ClientAddress",
                newName: "IX_ClientAddress_TypeObjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientAddresses_ClientId",
                table: "ClientAddress",
                newName: "IX_ClientAddress_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientAddress",
                table: "ClientAddress",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientAddress_AddressTypes_TypeObjectId",
                table: "ClientAddress",
                column: "TypeObjectId",
                principalTable: "AddressTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientAddress_Clients_ClientId",
                table: "ClientAddress",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
