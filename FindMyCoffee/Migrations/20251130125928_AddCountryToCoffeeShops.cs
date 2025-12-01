using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyCoffee.Migrations
{
    /// <inheritdoc />
    public partial class AddCountryToCoffeeShops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "CoffeeShops",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "CoffeeShops");
        }
    }
}
