using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizBio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBundlesToCatalogItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BundleId",
                table: "CatalogItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ItemType",
                table: "CatalogItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_BundleId",
                table: "CatalogItems",
                column: "BundleId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_ItemType",
                table: "CatalogItems",
                column: "ItemType");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItems_CatalogBundles_BundleId",
                table: "CatalogItems",
                column: "BundleId",
                principalTable: "CatalogBundles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItems_CatalogBundles_BundleId",
                table: "CatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItems_BundleId",
                table: "CatalogItems");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItems_ItemType",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "BundleId",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "ItemType",
                table: "CatalogItems");
        }
    }
}
