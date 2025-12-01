using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyCoffee.Migrations
{
    /// <inheritdoc />
    public partial class UserCoffeeShopsTableForManyToManyRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserCoffeeShops",
                columns: table => new
                {
                    CoffeeShopsId = table.Column<int>(type: "integer", nullable: false),
                    UsersId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCoffeeShops", x => new { x.CoffeeShopsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_UserCoffeeShops_CoffeeShops_CoffeeShopsId",
                        column: x => x.CoffeeShopsId,
                        principalTable: "CoffeeShops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCoffeeShops_Users_UsersId",
                        column: x => x.UsersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCoffeeShops_UsersId",
                table: "UserCoffeeShops",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCoffeeShops");
        }
    }
}
