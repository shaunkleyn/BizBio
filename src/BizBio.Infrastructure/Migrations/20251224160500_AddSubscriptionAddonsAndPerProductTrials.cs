using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizBio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionAddonsAndPerProductTrials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop shadow property artifacts if they exist (from previous configuration issue)
            migrationBuilder.Sql(@"
                SET @fk_exists = (
                    SELECT COUNT(*) 
                    FROM information_schema.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_SCHEMA = DATABASE()
                    AND TABLE_NAME = 'CatalogBundles'
                    AND CONSTRAINT_NAME = 'FK_CatalogBundles_Catalogs_CatalogId1'
                );
                SET @sql = IF(@fk_exists > 0, 
                    'ALTER TABLE `CatalogBundles` DROP FOREIGN KEY `FK_CatalogBundles_Catalogs_CatalogId1`', 
                    'SELECT ''Foreign key does not exist, skipping...''');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @idx_exists = (
                    SELECT COUNT(*) 
                    FROM information_schema.STATISTICS 
                    WHERE TABLE_SCHEMA = DATABASE()
                    AND TABLE_NAME = 'CatalogBundles'
                    AND INDEX_NAME = 'IX_CatalogBundles_CatalogId1'
                );
                SET @sql = IF(@idx_exists > 0, 
                    'ALTER TABLE `CatalogBundles` DROP INDEX `IX_CatalogBundles_CatalogId1`', 
                    'SELECT ''Index does not exist, skipping...''');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @col_exists = (
                    SELECT COUNT(*) 
                    FROM information_schema.COLUMNS 
                    WHERE TABLE_SCHEMA = DATABASE()
                    AND TABLE_NAME = 'CatalogBundles'
                    AND COLUMN_NAME = 'CatalogId1'
                );
                SET @sql = IF(@col_exists > 0, 
                    'ALTER TABLE `CatalogBundles` DROP COLUMN `CatalogId1`', 
                    'SELECT ''Column does not exist, skipping...''');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Add ProductLineId column - default to 1 (Menu product) for existing subscriptions
            migrationBuilder.AddColumn<int>(
                name: "ProductLineId",
                table: "UserSubscriptions",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Ensure all existing subscriptions have ProductLineId = 1 (Menu product)
            // This is safe because your first product is Menu
            migrationBuilder.Sql(
                @"UPDATE UserSubscriptions 
                  SET ProductLineId = 1 
                  WHERE ProductLineId = 0 OR ProductLineId IS NULL;");

            migrationBuilder.CreateTable(
                name: "SubscriptionAddons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductLineId = table.Column<int>(type: "int", nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ConfigurationJson = table.Column<string>(type: "varchar(4000)", maxLength: 4000, nullable: true)
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
                    table.PrimaryKey("PK_SubscriptionAddons", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SubscriptionTierAddons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SubscriptionTierId = table.Column<int>(type: "int", nullable: false),
                    AddonId = table.Column<int>(type: "int", nullable: false),
                    IsRequired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
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
                    table.PrimaryKey("PK_SubscriptionTierAddons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionTierAddons_SubscriptionAddons_AddonId",
                        column: x => x.AddonId,
                        principalTable: "SubscriptionAddons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionTierAddons_SubscriptionTiers_SubscriptionTierId",
                        column: x => x.SubscriptionTierId,
                        principalTable: "SubscriptionTiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UserSubscriptionAddons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserSubscriptionId = table.Column<int>(type: "int", nullable: false),
                    AddonId = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CancelledAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    IsActiveAddon = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
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
                    table.PrimaryKey("PK_UserSubscriptionAddons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSubscriptionAddons_SubscriptionAddons_AddonId",
                        column: x => x.AddonId,
                        principalTable: "SubscriptionAddons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSubscriptionAddons_UserSubscriptions_UserSubscriptionId",
                        column: x => x.UserSubscriptionId,
                        principalTable: "UserSubscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptions_ProductLineId",
                table: "UserSubscriptions",
                column: "ProductLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAddons_IsActive",
                table: "SubscriptionAddons",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAddons_ProductLineId",
                table: "SubscriptionAddons",
                column: "ProductLineId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionAddons_ProductLineId_SortOrder",
                table: "SubscriptionAddons",
                columns: new[] { "ProductLineId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionTierAddons_AddonId",
                table: "SubscriptionTierAddons",
                column: "AddonId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionTierAddons_IsActive",
                table: "SubscriptionTierAddons",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionTierAddons_SubscriptionTierId_AddonId",
                table: "SubscriptionTierAddons",
                columns: new[] { "SubscriptionTierId", "AddonId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptionAddons_AddonId",
                table: "UserSubscriptionAddons",
                column: "AddonId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptionAddons_IsActive",
                table: "UserSubscriptionAddons",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptionAddons_UserSubscriptionId",
                table: "UserSubscriptionAddons",
                column: "UserSubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubscriptionAddons_UserSubscriptionId_AddonId_IsActiveAd~",
                table: "UserSubscriptionAddons",
                columns: new[] { "UserSubscriptionId", "AddonId", "IsActiveAddon" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserSubscriptions_ProductLines_ProductLineId",
                table: "UserSubscriptions",
                column: "ProductLineId",
                principalTable: "ProductLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserSubscriptions_ProductLines_ProductLineId",
                table: "UserSubscriptions");

            migrationBuilder.DropTable(
                name: "SubscriptionTierAddons");

            migrationBuilder.DropTable(
                name: "UserSubscriptionAddons");

            migrationBuilder.DropTable(
                name: "SubscriptionAddons");

            migrationBuilder.DropIndex(
                name: "IX_UserSubscriptions_ProductLineId",
                table: "UserSubscriptions");

            migrationBuilder.DropColumn(
                name: "ProductLineId",
                table: "UserSubscriptions");

            migrationBuilder.AddColumn<int>(
                name: "CatalogId1",
                table: "CatalogBundles",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogBundles_CatalogId1",
                table: "CatalogBundles",
                column: "CatalogId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogBundles_Catalogs_CatalogId1",
                table: "CatalogBundles",
                column: "CatalogId1",
                principalTable: "Catalogs",
                principalColumn: "Id");
        }
    }
}
