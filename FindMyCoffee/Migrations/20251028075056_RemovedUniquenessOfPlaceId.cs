using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyCoffee.Migrations
{
    /// <inheritdoc />
    public partial class RemovedUniquenessOfPlaceId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CoffeeShops_PlaceId",
                table: "CoffeeShops");

            migrationBuilder.AlterColumn<string>(
                name: "PlaceId",
                table: "CoffeeShops",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeShops_PlaceId",
                table: "CoffeeShops",
                column: "PlaceId",
                filter: "\"PlaceId\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CoffeeShops_PlaceId",
                table: "CoffeeShops");

            migrationBuilder.AlterColumn<string>(
                name: "PlaceId",
                table: "CoffeeShops",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoffeeShops_PlaceId",
                table: "CoffeeShops",
                column: "PlaceId",
                unique: true);
        }
    }
}
