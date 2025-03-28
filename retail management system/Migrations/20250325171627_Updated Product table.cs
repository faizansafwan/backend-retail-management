using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace retail_management_system.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedProducttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Products");
        }
    }
}
