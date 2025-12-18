using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizBio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCatalogItemExtrasSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CatalogItemExtraGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CatalogId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MinRequired = table.Column<int>(type: "int", nullable: false),
                    MaxAllowed = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    AllowMultipleQuantities = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItemExtraGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogItemExtraGroups_Catalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogItemExtraGroups_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogItemExtras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    CatalogId = table.Column<int>(type: "int", nullable: true),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BasePrice = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ImageUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItemExtras", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogItemExtras_Catalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogItemExtras_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogItemExtraGroupLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CatalogItemId = table.Column<int>(type: "int", nullable: false),
                    ExtraGroupId = table.Column<int>(type: "int", nullable: false),
                    VariantId = table.Column<int>(type: "int", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItemExtraGroupLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogItemExtraGroupLinks_CatalogItemExtraGroups_ExtraGroup~",
                        column: x => x.ExtraGroupId,
                        principalTable: "CatalogItemExtraGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogItemExtraGroupLinks_CatalogItemVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "CatalogItemVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CatalogItemExtraGroupLinks_CatalogItems_CatalogItemId",
                        column: x => x.CatalogItemId,
                        principalTable: "CatalogItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CatalogItemExtraGroupItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExtraGroupId = table.Column<int>(type: "int", nullable: false),
                    ExtraId = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    PriceOverride = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsValid = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedBy = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogItemExtraGroupItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogItemExtraGroupItems_CatalogItemExtraGroups_ExtraGroup~",
                        column: x => x.ExtraGroupId,
                        principalTable: "CatalogItemExtraGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogItemExtraGroupItems_CatalogItemExtras_ExtraId",
                        column: x => x.ExtraId,
                        principalTable: "CatalogItemExtras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemExtraGroupItems_ExtraGroupId_ExtraId",
                table: "CatalogItemExtraGroupItems",
                columns: new[] { "ExtraGroupId", "ExtraId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemExtraGroupItems_ExtraId",
                table: "CatalogItemExtraGroupItems",
                column: "ExtraId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemExtraGroupLinks_CatalogItemId",
                table: "CatalogItemExtraGroupLinks",
                column: "CatalogItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemExtraGroupLinks_ExtraGroupId",
                table: "CatalogItemExtraGroupLinks",
                column: "ExtraGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemExtraGroupLinks_VariantId",
                table: "CatalogItemExtraGroupLinks",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemExtraGroups_CatalogId",
                table: "CatalogItemExtraGroups",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemExtraGroups_IsActive",
                table: "CatalogItemExtraGroups",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemExtraGroups_UserId",
                table: "CatalogItemExtraGroups",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemExtras_CatalogId",
                table: "CatalogItemExtras",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemExtras_IsActive",
                table: "CatalogItemExtras",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogItemExtras_UserId",
                table: "CatalogItemExtras",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogItemExtraGroupItems");

            migrationBuilder.DropTable(
                name: "CatalogItemExtraGroupLinks");

            migrationBuilder.DropTable(
                name: "CatalogItemExtras");

            migrationBuilder.DropTable(
                name: "CatalogItemExtraGroups");
        }
    }
}
