using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoesBangladesh.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyDescription",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductSectionDescription",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyDescription",
                table: "SystemSettings");

            migrationBuilder.DropColumn(
                name: "ProductSectionDescription",
                table: "SystemSettings");
        }
    }
}
