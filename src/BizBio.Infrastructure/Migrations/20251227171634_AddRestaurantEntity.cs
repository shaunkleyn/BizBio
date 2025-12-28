using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BizBio.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestaurantEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add subscription tier limits
            migrationBuilder.AddColumn<int>(
                name: "MaxLibraryItems",
                table: "SubscriptionTiers",
                type: "int",
                nullable: false,
                defaultValue: 50);

            migrationBuilder.AddColumn<int>(
                name: "MaxProfilesPerRestaurant",
                table: "SubscriptionTiers",
                type: "int",
                nullable: false,
                defaultValue: 3);

            migrationBuilder.AddColumn<int>(
                name: "MaxRestaurants",
                table: "SubscriptionTiers",
                type: "int",
                nullable: false,
                defaultValue: 1);

            // Step 1: Add RestaurantId column as NULLABLE first
            migrationBuilder.AddColumn<int>(
                name: "RestaurantId",
                table: "Profiles",
                type: "int",
                nullable: true);

            // Step 2: Create Restaurants table
            migrationBuilder.CreateTable(
                name: "Restaurants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Logo = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    State = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PostalCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Country = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Website = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Currency = table.Column<string>(type: "varchar(3)", maxLength: 3, nullable: false, defaultValue: "ZAR")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Timezone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
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
                    table.PrimaryKey("PK_Restaurants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Restaurants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_RestaurantId",
                table: "Profiles",
                column: "RestaurantId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_IsActive",
                table: "Restaurants",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_UserId",
                table: "Restaurants",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Restaurants_UserId_SortOrder",
                table: "Restaurants",
                columns: new[] { "UserId", "SortOrder" });

            // Step 3: Create default restaurant for each existing user with profiles
            migrationBuilder.Sql(@"
                INSERT INTO Restaurants (UserId, Name, Currency, SortOrder, IsActive, IsValid, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
                SELECT DISTINCT UserId, 'Main Restaurant', 'ZAR', 0, 1, 1, UTC_TIMESTAMP(), UTC_TIMESTAMP(), 'Migration', 'Migration'
                FROM Profiles
                WHERE UserId IS NOT NULL;
            ");

            // Step 4: Link existing profiles to their user's default restaurant
            migrationBuilder.Sql(@"
                UPDATE Profiles p
                INNER JOIN Restaurants r ON r.UserId = p.UserId
                SET p.RestaurantId = r.Id
                WHERE p.RestaurantId IS NULL;
            ");

            // Step 5: Make RestaurantId NOT NULL now that all profiles have been linked
            migrationBuilder.AlterColumn<int>(
                name: "RestaurantId",
                table: "Profiles",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Step 6: Add foreign key constraint
            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Restaurants_RestaurantId",
                table: "Profiles",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Restaurants_RestaurantId",
                table: "Profiles");

            migrationBuilder.DropTable(
                name: "Restaurants");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_RestaurantId",
                table: "Profiles");

            migrationBuilder.DropColumn(
                name: "MaxLibraryItems",
                table: "SubscriptionTiers");

            migrationBuilder.DropColumn(
                name: "MaxProfilesPerRestaurant",
                table: "SubscriptionTiers");

            migrationBuilder.DropColumn(
                name: "MaxRestaurants",
                table: "SubscriptionTiers");

            migrationBuilder.DropColumn(
                name: "RestaurantId",
                table: "Profiles");
        }
    }
}
