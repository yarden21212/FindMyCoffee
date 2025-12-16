using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FindMyCoffee.Migrations
{
    /// <inheritdoc />
    public partial class AddUserCoffeeShopsManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCoffeeShops_CoffeeShops_CoffeeShopsId",
                table: "UserCoffeeShops");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCoffeeShops_Users_UsersId",
                table: "UserCoffeeShops");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "UserCoffeeShops",
                newName: "CoffeeShopId");

            migrationBuilder.RenameColumn(
                name: "CoffeeShopsId",
                table: "UserCoffeeShops",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCoffeeShops_UsersId",
                table: "UserCoffeeShops",
                newName: "IX_UserCoffeeShops_CoffeeShopId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserCoffeeShops",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoffeeShops_CoffeeShops_CoffeeShopId",
                table: "UserCoffeeShops",
                column: "CoffeeShopId",
                principalTable: "CoffeeShops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoffeeShops_Users_UserId",
                table: "UserCoffeeShops",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserCoffeeShops_CoffeeShops_CoffeeShopId",
                table: "UserCoffeeShops");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCoffeeShops_Users_UserId",
                table: "UserCoffeeShops");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserCoffeeShops");

            migrationBuilder.RenameColumn(
                name: "CoffeeShopId",
                table: "UserCoffeeShops",
                newName: "UsersId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserCoffeeShops",
                newName: "CoffeeShopsId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCoffeeShops_CoffeeShopId",
                table: "UserCoffeeShops",
                newName: "IX_UserCoffeeShops_UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoffeeShops_CoffeeShops_CoffeeShopsId",
                table: "UserCoffeeShops",
                column: "CoffeeShopsId",
                principalTable: "CoffeeShops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCoffeeShops_Users_UsersId",
                table: "UserCoffeeShops",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
