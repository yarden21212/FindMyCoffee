using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyCoffee.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessFieldsToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AcceptBusinessTerms",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "BusinessContactEmail",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessPhone",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBusiness",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptBusinessTerms",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BusinessContactEmail",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BusinessPhone",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsBusiness",
                table: "Users");
        }
    }
}
