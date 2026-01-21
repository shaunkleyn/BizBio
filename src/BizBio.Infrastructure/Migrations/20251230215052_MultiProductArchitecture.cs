using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizBio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MultiProductArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleCategories_Categories_CategoryId",
                table: "BundleCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogBundleCategories_Categories_CategoryId",
                table: "CatalogBundleCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItemCategory_Categories_CategoryId",
                table: "CatalogItemCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItemInstanceCategories_Categories_CategoryId",
                table: "CatalogItemInstanceCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Catalogs_Profiles_ProfileId",
                table: "Catalogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Catalogs_CatalogId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CatalogId_SortOrder",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_IsActive",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_UserId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "OwnerType",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Images",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "CatalogCategories");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "Catalogs",
                newName: "EntityId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_CatalogId",
                table: "CatalogCategories",
                newName: "IX_CatalogCategories_CatalogId");

            migrationBuilder.AddColumn<int>(
                name: "MaxBundles",
                table: "SubscriptionTiers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxCatalogsPerEntity",
                table: "SubscriptionTiers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxCategoriesPerCatalog",
                table: "SubscriptionTiers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxEntities",
                table: "SubscriptionTiers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductType",
                table: "SubscriptionTiers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrialDays",
                table: "SubscriptionTiers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ProfileId",
                table: "Catalogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Catalogs",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Catalogs",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Catalogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CatalogItems",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Images",
                table: "CatalogItems",
                type: "varchar(5000)",
                maxLength: 5000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ParentCatalogItemId",
                table: "CatalogItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceOverride",
                table: "CatalogItems",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "CatalogCategories",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            migrationBuilder.AlterColumn<int>(
                name: "SortOrder",
                table: "CatalogCategories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "CatalogId",
                table: "CatalogCategories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "CatalogCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatalogCategories",
                table: "CatalogCategories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Logo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PostalCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Website = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Currency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false, defaultValue: "ZAR")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Timezone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "Africa/Johannesburg")
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
                    table.PrimaryKey("PK_Entities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entities_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductSubscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ProductType = table.Column<int>(type: "int", nullable: false),
                    TierId = table.Column<int>(type: "int", nullable: false),
                    IsTrialActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    TrialStartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TrialEndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    BillingCycle = table.Column<int>(type: "int", nullable: false),
                    CurrentPeriodStart = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CurrentPeriodEnd = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    NextBillingDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CancelledAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
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
                    table.PrimaryKey("PK_ProductSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductSubscriptions_SubscriptionTiers_TierId",
                        column: x => x.TierId,
                        principalTable: "SubscriptionTiers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductSubscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CategoriesNew",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Slug = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Icon = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
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
                    table.PrimaryKey("PK_CategoriesNew", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoriesNew_Entities_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Entities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_EntityId",
                table: "Catalogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_EntityId_SortOrder",
                table: "Catalogs",
                columns: new[] { "EntityId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItems_ParentCatalogItemId",
                table: "CatalogItems",
                column: "ParentCatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCategories_CatalogId_CategoryId",
                table: "CatalogCategories",
                columns: new[] { "CatalogId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCategories_CategoryId",
                table: "CatalogCategories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesNew_EntityId",
                table: "CategoriesNew",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesNew_EntityId_SortOrder",
                table: "CategoriesNew",
                columns: new[] { "EntityId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_CategoriesNew_IsActive",
                table: "CategoriesNew",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_EntityType",
                table: "Entities",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_IsActive",
                table: "Entities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_Slug",
                table: "Entities",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entities_UserId",
                table: "Entities",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Entities_UserId_SortOrder",
                table: "Entities",
                columns: new[] { "UserId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubscriptions_ProductType",
                table: "ProductSubscriptions",
                column: "ProductType");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubscriptions_Status",
                table: "ProductSubscriptions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubscriptions_TierId",
                table: "ProductSubscriptions",
                column: "TierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubscriptions_UserId",
                table: "ProductSubscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSubscriptions_UserId_ProductType",
                table: "ProductSubscriptions",
                columns: new[] { "UserId", "ProductType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BundleCategories_CatalogCategories_CategoryId",
                table: "BundleCategories",
                column: "CategoryId",
                principalTable: "CatalogCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogBundleCategories_CatalogCategories_CategoryId",
                table: "CatalogBundleCategories",
                column: "CategoryId",
                principalTable: "CatalogCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogCategories_Catalogs_CatalogId",
                table: "CatalogCategories",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogCategories_CategoriesNew_CategoryId",
                table: "CatalogCategories",
                column: "CategoryId",
                principalTable: "CategoriesNew",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItemCategory_CategoriesNew_CategoryId",
                table: "CatalogItemCategory",
                column: "CategoryId",
                principalTable: "CategoriesNew",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItemInstanceCategories_CatalogCategories_CategoryId",
                table: "CatalogItemInstanceCategories",
                column: "CategoryId",
                principalTable: "CatalogCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItems_CatalogItems_ParentCatalogItemId",
                table: "CatalogItems",
                column: "ParentCatalogItemId",
                principalTable: "CatalogItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogs_Entities_EntityId",
                table: "Catalogs",
                column: "EntityId",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogs_Profiles_ProfileId",
                table: "Catalogs",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BundleCategories_CatalogCategories_CategoryId",
                table: "BundleCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogBundleCategories_CatalogCategories_CategoryId",
                table: "CatalogBundleCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogCategories_Catalogs_CatalogId",
                table: "CatalogCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogCategories_CategoriesNew_CategoryId",
                table: "CatalogCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItemCategory_CategoriesNew_CategoryId",
                table: "CatalogItemCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItemInstanceCategories_CatalogCategories_CategoryId",
                table: "CatalogItemInstanceCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_CatalogItems_CatalogItems_ParentCatalogItemId",
                table: "CatalogItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Catalogs_Entities_EntityId",
                table: "Catalogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Catalogs_Profiles_ProfileId",
                table: "Catalogs");

            migrationBuilder.DropTable(
                name: "CategoriesNew");

            migrationBuilder.DropTable(
                name: "ProductSubscriptions");

            migrationBuilder.DropTable(
                name: "Entities");

            migrationBuilder.DropIndex(
                name: "IX_Catalogs_EntityId",
                table: "Catalogs");

            migrationBuilder.DropIndex(
                name: "IX_Catalogs_EntityId_SortOrder",
                table: "Catalogs");

            migrationBuilder.DropIndex(
                name: "IX_CatalogItems_ParentCatalogItemId",
                table: "CatalogItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CatalogCategories",
                table: "CatalogCategories");

            migrationBuilder.DropIndex(
                name: "IX_CatalogCategories_CatalogId_CategoryId",
                table: "CatalogCategories");

            migrationBuilder.DropIndex(
                name: "IX_CatalogCategories_CategoryId",
                table: "CatalogCategories");

            migrationBuilder.DropColumn(
                name: "MaxBundles",
                table: "SubscriptionTiers");

            migrationBuilder.DropColumn(
                name: "MaxCatalogsPerEntity",
                table: "SubscriptionTiers");

            migrationBuilder.DropColumn(
                name: "MaxCategoriesPerCatalog",
                table: "SubscriptionTiers");

            migrationBuilder.DropColumn(
                name: "MaxEntities",
                table: "SubscriptionTiers");

            migrationBuilder.DropColumn(
                name: "ProductType",
                table: "SubscriptionTiers");

            migrationBuilder.DropColumn(
                name: "TrialDays",
                table: "SubscriptionTiers");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "ParentCatalogItemId",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "PriceOverride",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CatalogCategories");

            migrationBuilder.RenameTable(
                name: "CatalogCategories",
                newName: "Categories");

            migrationBuilder.RenameColumn(
                name: "EntityId",
                table: "Catalogs",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_CatalogCategories_CatalogId",
                table: "Categories",
                newName: "IX_Categories_CatalogId");

            migrationBuilder.AlterColumn<int>(
                name: "ProfileId",
                table: "Catalogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Catalogs",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "Catalogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OwnerType",
                table: "Catalogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "CatalogItems",
                type: "varchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Images",
                table: "CatalogItems",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(5000)",
                oldMaxLength: 5000,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Categories",
                type: "datetime(6)",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.AlterColumn<int>(
                name: "SortOrder",
                table: "Categories",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "CatalogId",
                table: "Categories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Categories",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Categories",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Images",
                table: "Categories",
                type: "varchar(5000)",
                maxLength: 5000,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Categories",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "ParentCategoryId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CatalogId_SortOrder",
                table: "Categories",
                columns: new[] { "CatalogId", "SortOrder" });

            migrationBuilder.CreateIndex(
                name: "IX_Categories_IsActive",
                table: "Categories",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BundleCategories_Categories_CategoryId",
                table: "BundleCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogBundleCategories_Categories_CategoryId",
                table: "CatalogBundleCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItemCategory_Categories_CategoryId",
                table: "CatalogItemCategory",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CatalogItemInstanceCategories_Categories_CategoryId",
                table: "CatalogItemInstanceCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogs_Profiles_ProfileId",
                table: "Catalogs",
                column: "ProfileId",
                principalTable: "Profiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Catalogs_CatalogId",
                table: "Categories",
                column: "CatalogId",
                principalTable: "Catalogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Users_UserId",
                table: "Categories",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
