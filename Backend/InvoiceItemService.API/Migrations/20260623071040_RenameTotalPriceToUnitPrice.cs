using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InvoiceItemService.API.Migrations
{
    /// <inheritdoc />
    public partial class RenameTotalPriceToUnitPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "InvoiceItems",
                newName: "UnitPrice");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UnitPrice",
                table: "InvoiceItems",
                newName: "TotalPrice");
        }
    }
}
