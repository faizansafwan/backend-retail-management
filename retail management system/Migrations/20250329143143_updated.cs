using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace retail_management_system.Migrations
{
    /// <inheritdoc />
    public partial class updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShopId",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ShopId",
                table: "Stocks",
                column: "ShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Shops_ShopId",
                table: "Stocks",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Shops_ShopId",
                table: "Stocks");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_ShopId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "ShopId",
                table: "Stocks");
        }
    }
}
