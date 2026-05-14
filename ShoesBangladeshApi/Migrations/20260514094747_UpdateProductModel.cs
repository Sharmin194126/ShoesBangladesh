using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoesBangladesh.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AdditionalImages",
                table: "Products",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "AdditionalImagesJson",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AssignedEmployeeId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvailableSizesJson",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DeliveryCharge",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "DisplaySectionId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MaxOrderLimit",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "OfferPercentage",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductType",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SizeQuantitiesJson",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "VatPercentage",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalImagesJson",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AssignedEmployeeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "AvailableSizesJson",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DeliveryCharge",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DisplaySectionId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "MaxOrderLimit",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "OfferPercentage",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SizeQuantitiesJson",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "VatPercentage",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Products",
                newName: "AdditionalImages");
        }
    }
}
