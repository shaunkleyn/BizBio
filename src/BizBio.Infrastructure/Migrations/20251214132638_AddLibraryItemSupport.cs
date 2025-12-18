using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizBio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLibraryItemSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "CatalogId",
                table: "Categories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CatalogId",
                table: "CatalogItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "SourceLibraryItemId",
                table: "CatalogItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "CatalogItems",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "CatalogItems",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SourceLibraryItemId",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CatalogItems");

            migrationBuilder.AlterColumn<int>(
                name: "CatalogId",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CatalogId",
                table: "CatalogItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
