using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace retail_management.Migrations
{
    /// <inheritdoc />
    public partial class Quantitydeleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "InvoiceStocks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "InvoiceStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
