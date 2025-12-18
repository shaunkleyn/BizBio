using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizBio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogItemVariants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "UserSubscriptions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "UserSubscriptions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "UserSubscriptions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "UserSubscriptions",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "Users",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "SubscriptionTiers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "SubscriptionTiers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "SubscriptionTiers",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "RestaurantTables",
                keyColumn: "CreatedBy",
                keyValue: null,
                column: "CreatedBy",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "CreatedBy",
                table: "RestaurantTables",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "RestaurantTables",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "RestaurantTables",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Profiles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "Profiles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Profiles",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Categories",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Images",
                table: "Categories",
                type: "varchar(5000)",
                maxLength: 5000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "Categories",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Categories",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Catalogs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "Catalogs",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Catalogs",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidFrom",
                table: "Catalogs",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidTo",
                table: "Catalogs",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CatalogItems",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "IsValid",
                table: "CatalogItems",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CatalogItems",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogItemAttributes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItemAttributes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogItemPriceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItemPriceTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogItemVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CatalogItemId = table.Column<int>(type: "int", nullable: false),
                    Sku = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SizeValue = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    SizeUnit = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UnitOfMeasure = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    WeightG = table.Column<int>(type: "int", nullable: true),
                    Barcode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDefault = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItemVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogItemVariants_CatalogItems_CatalogItemId",
                        column: x => x.CatalogItemId,
                        principalTable: "CatalogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogItemAttributeValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AttributeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItemAttributeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogItemAttributeValues_CatalogItemAttributes_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "CatalogItemAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogItemInventories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VariantId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: true),
                    QtyAvailable = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    QtyReserved = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItemInventories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogItemInventories_CatalogItemVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "CatalogItemVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogItemVariantPrices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VariantId = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    PriceTypeId = table.Column<int>(type: "int", nullable: true),
                    StartsAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItemVariantPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogItemVariantPrices_CatalogItemPriceTypes_PriceTypeId",
                        column: x => x.PriceTypeId,
                        principalTable: "CatalogItemPriceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CatalogItemVariantPrices_CatalogItemVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "CatalogItemVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogItemVariantAttributeValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VariantId = table.Column<int>(type: "int", nullable: false),
                    AttributeValueId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP"),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItemVariantAttributeValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogItemVariantAttributeValues_CatalogItemAttributeValues~",
                        column: x => x.AttributeValueId,
                        principalTable: "CatalogItemAttributeValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogItemVariantAttributeValues_CatalogItemVariants_Varian~",
                        column: x => x.VariantId,
                        principalTable: "CatalogItemVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemAttributes_IsActive",
                table: "CatalogItemAttributes",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemAttributes_Slug",
                table: "CatalogItemAttributes",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemAttributeValues_AttributeId",
                table: "CatalogItemAttributeValues",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemAttributeValues_IsActive",
                table: "CatalogItemAttributeValues",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemInventories_LocationId",
                table: "CatalogItemInventories",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemInventories_VariantId",
                table: "CatalogItemInventories",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemPriceTypes_Slug",
                table: "CatalogItemPriceTypes",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemVariantAttributeValues_AttributeValueId",
                table: "CatalogItemVariantAttributeValues",
                column: "AttributeValueId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemVariantAttributeValues_VariantId_AttributeValueId",
                table: "CatalogItemVariantAttributeValues",
                columns: new[] { "VariantId", "AttributeValueId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemVariantPrices_PriceTypeId",
                table: "CatalogItemVariantPrices",
                column: "PriceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemVariantPrices_StartsAt_EndsAt",
                table: "CatalogItemVariantPrices",
                columns: new[] { "StartsAt", "EndsAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemVariantPrices_VariantId",
                table: "CatalogItemVariantPrices",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemVariants_CatalogItemId",
                table: "CatalogItemVariants",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemVariants_IsActive",
                table: "CatalogItemVariants",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemVariants_Sku",
                table: "CatalogItemVariants",
                column: "Sku");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogItemInventories");

            migrationBuilder.DropTable(
                name: "CatalogItemVariantAttributeValues");

            migrationBuilder.DropTable(
                name: "CatalogItemVariantPrices");

            migrationBuilder.DropTable(
                name: "CatalogItemAttributeValues");

            migrationBuilder.DropTable(
                name: "CatalogItemPriceTypes");

            migrationBuilder.DropTable(
                name: "CatalogItemVariants");

            migrationBuilder.DropTable(
                name: "CatalogItemAttributes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SubscriptionTiers");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "SubscriptionTiers");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SubscriptionTiers");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "RestaurantTables");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "RestaurantTables");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Images",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "ValidFrom",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "ValidTo",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "IsValid",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CatalogItems");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedBy",
                table: "RestaurantTables",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
