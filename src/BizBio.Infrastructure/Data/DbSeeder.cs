using BizBio.Core.Entities;
using BizBio.Core.Enums;
using BizBio.Infrastructure.Data.Seeding;
using Microsoft.EntityFrameworkCore;

namespace BizBio.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Note: Database migrations are applied in Program.cs before calling this method

        // Seed Enum Lookups first (required for foreign keys)
        Console.WriteLine("Seeding enum lookups...");
        await EnumSeeder.SeedEnumLookupsAsync(context);
//        await MenuSeeder.SeedAsync(
//    context.Database.GetConnectionString()
//);
//        await context.SaveChangesAsync();

        Console.WriteLine("Enum lookups seeded successfully");

        // Seed Subscription Tiers if not already present
        if (!await context.SubscriptionTiers.AnyAsync())
        {
            Console.WriteLine("Seeding subscription tiers...");
            var tiers = await GetSubscriptionTiersAsync(context);
            await context.SubscriptionTiers.AddRangeAsync(tiers);
            await context.SaveChangesAsync();
            Console.WriteLine($"Seeded {tiers.Count} subscription tiers");
        }
        else
        {
            Console.WriteLine("Subscription tiers already exist, skipping seed");
        }
    }

    private static async Task<List<SubscriptionTier>> GetSubscriptionTiersAsync(ApplicationDbContext context)
    {
        var now = DateTime.UtcNow;

        // Get lookup IDs
        var connectId = (await context.ProductLines.FirstAsync(p => p.Name == nameof(ProductLine.Connect))).Id;
        var menuId = (await context.ProductLines.FirstAsync(p => p.Name == nameof(ProductLine.Menu))).Id;
        var retailId = (await context.ProductLines.FirstAsync(p => p.Name == nameof(ProductLine.Retail))).Id;

        return new List<SubscriptionTier>
        {
            // PROFESSIONAL TIERS
            new SubscriptionTier
            {
                ProductLineId = connectId,
                TierName = "Free",
                TierCode = "CONNECT_FREE",
                DisplayName = "Free",
                Description = "Perfect for trying out BizBio",
                MonthlyPrice = 0,
                AnnualPrice = 0,
                MaxProfiles = 1,
                MaxCatalogItems = 0,
                MaxLocations = 0,
                MaxTeamMembers = 0,
                MaxDocuments = 0,
                DisplayOrder = 1,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "System",
                UpdatedBy = "System"
            },
            new SubscriptionTier
            {
                ProductLineId = connectId,
                TierName = "Pro",
                TierCode = "CONNECT_PRO",
                DisplayName = "Pro",
                Description = "For individual professionals",
                MonthlyPrice = 99,
                AnnualPrice = 999,
                MaxProfiles = 5,
                MaxDocuments = 10,
                MaxDocumentSizeMB = 5,
                CustomBranding = true,
                RemoveBranding = true,
                Analytics = true,
                PrioritySupport = true,
                DisplayOrder = 2,
                IsActive = true,
                IsPopular = true,
                RecommendedFor = "Consultants, Freelancers, Professionals",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "System",
                UpdatedBy = "System"
            },
            new SubscriptionTier
            {
                ProductLineId = connectId,
                TierName = "Business",
                TierCode = "CONNECT_BUSINESS",
                DisplayName = "Business",
                Description = "For teams and growing businesses",
                MonthlyPrice = 249,
                AnnualPrice = 2490,
                MaxProfiles = 20,
                MaxTeamMembers = 20,
                MaxDocuments = 50,
                MaxDocumentSizeMB = 10,
                CustomBranding = true,
                RemoveBranding = true,
                Analytics = true,
                AdvancedAnalytics = true,
                CustomSubdomain = true,
                PrioritySupport = true,
                TeamManagement = true,
                OrganizationalHierarchy = true,
                DisplayOrder = 3,
                IsActive = true,
                RecommendedFor = "Small to medium businesses",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "System",
                UpdatedBy = "System"
            },

            // MENU TIERS
            new SubscriptionTier
            {
                ProductLineId = menuId,
                TierName = "Starter",
                TierCode = "MENU_STARTER",
                DisplayName = "Starter",
                Description = "For single-location restaurants",
                MonthlyPrice = 149,
                AnnualPrice = 1490,
                MaxProfiles = 1,
                MaxCatalogItems = 50,
                MaxLocations = 1,
                MaxImagesPerItem = 5,
                DisplayOrder = 1,
                IsActive = true,
                RecommendedFor = "Food trucks, Small cafes",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "System",
                UpdatedBy = "System"
            },
            new SubscriptionTier
            {
                ProductLineId = menuId,
                TierName = "Restaurant",
                TierCode = "MENU_RESTAURANT",
                DisplayName = "Restaurant",
                Description = "For established restaurants",
                MonthlyPrice = 299,
                AnnualPrice = 2990,
                MaxProfiles = 3,
                MaxCatalogItems = 200,
                MaxLocations = 3,
                MaxImagesPerItem = -1, // Unlimited
                CustomBranding = true,
                RemoveBranding = true,
                Analytics = true,
                ItemVariants = true,
                ItemAddons = true,
                AllergenInfo = true,
                DietaryTags = true,
                MenuScheduling = true,
                PrioritySupport = true,
                DisplayOrder = 2,
                IsActive = true,
                IsPopular = true,
                RecommendedFor = "Full-service restaurants",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "System",
                UpdatedBy = "System"
            },

            // RETAIL TIERS
            new SubscriptionTier
            {
                ProductLineId = retailId,
                TierName = "Starter",
                TierCode = "RETAIL_STARTER",
                DisplayName = "Starter",
                Description = "For small retail businesses",
                MonthlyPrice = 149,
                AnnualPrice = 1490,
                MaxProfiles = 1,
                MaxCatalogItems = 100,
                MaxLocations = 1,
                MaxImagesPerItem = 5,
                DisplayOrder = 1,
                IsActive = true,
                RecommendedFor = "Small shops, Online stores",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "System",
                UpdatedBy = "System"
            },
            new SubscriptionTier
            {
                ProductLineId = retailId,
                TierName = "Business",
                TierCode = "RETAIL_BUSINESS",
                DisplayName = "Business",
                Description = "For growing retail businesses",
                MonthlyPrice = 299,
                AnnualPrice = 2990,
                MaxProfiles = 5,
                MaxCatalogItems = 500,
                MaxLocations = 5,
                MaxImagesPerItem = -1,
                CustomBranding = true,
                RemoveBranding = true,
                Analytics = true,
                InventoryTracking = true,
                BulkOperations = true,
                BarcodeGeneration = true,
                PrioritySupport = true,
                DisplayOrder = 2,
                IsActive = true,
                IsPopular = true,
                RecommendedFor = "Retail chains, E-commerce",
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = "System",
                UpdatedBy = "System"
            }
        };
    }
}