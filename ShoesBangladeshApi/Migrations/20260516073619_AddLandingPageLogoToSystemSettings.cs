using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoesBangladesh.API.Migrations
{
    /// <inheritdoc />
    public partial class AddLandingPageLogoToSystemSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LandingPageLogoUrl",
                table: "SystemSettings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LandingPageLogoUrl",
                table: "SystemSettings");
        }
    }
}
