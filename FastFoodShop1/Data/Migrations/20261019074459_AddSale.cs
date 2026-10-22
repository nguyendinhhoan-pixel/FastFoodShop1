using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FastFoodShop1.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSale : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SaleEnd",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalePrice",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaleEnd",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SalePrice",
                table: "Products");
        }
    }
}
