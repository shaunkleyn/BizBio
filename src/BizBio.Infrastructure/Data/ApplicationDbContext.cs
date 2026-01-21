using Microsoft.EntityFrameworkCore;
using BizBio.Core.Entities;
using BizBio.Core.Entities.Lookups;
using BizBio.Core.Enums;

namespace BizBio.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<SubscriptionTier> SubscriptionTiers => Set<SubscriptionTier>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<ProductSubscription> ProductSubscriptions => Set<ProductSubscription>();
    public DbSet<SubscriptionAddon> SubscriptionAddons => Set<SubscriptionAddon>();
    public DbSet<SubscriptionTierAddon> SubscriptionTierAddons => Set<SubscriptionTierAddon>();
    public DbSet<UserSubscriptionAddon> UserSubscriptionAddons => Set<UserSubscriptionAddon>();
    public DbSet<Entity> Entities => Set<Entity>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Catalog> Catalogs => Set<Catalog>();
    public DbSet<Category> Categories_New => Set<Category>();
    public DbSet<CatalogCategory> CatalogCategories => Set<CatalogCategory>();
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<CatalogItemVariant> CatalogItemVariants => Set<CatalogItemVariant>();
    //public DbSet<CatalogItemAttribute> CatalogItemAttributes => Set<CatalogItemAttribute>();
    //public DbSet<CatalogItemAttributeValue> CatalogItemAttributeValues => Set<CatalogItemAttributeValue>();
    //public DbSet<CatalogItemVariantAttributeValue> CatalogItemVariantAttributeValues => Set<CatalogItemVariantAttributeValue>();
    public DbSet<CatalogItemInventory> CatalogItemInventories => Set<CatalogItemInventory>();
    public DbSet<CatalogItemVariantPrice> CatalogItemVariantPrices => Set<CatalogItemVariantPrice>();
    public DbSet<CatalogBundle> CatalogBundles => Set<CatalogBundle>();
    public DbSet<CatalogBundleStep> CatalogBundleSteps => Set<CatalogBundleStep>();
    public DbSet<CatalogBundleStepProduct> CatalogBundleStepProducts => Set<CatalogBundleStepProduct>();
    public DbSet<CatalogBundleOptionGroup> CatalogBundleOptionGroups => Set<CatalogBundleOptionGroup>();
    public DbSet<CatalogBundleOption> CatalogBundleOptions => Set<CatalogBundleOption>();
    public DbSet<CatalogBundleCategory> CatalogBundleCategories => Set<CatalogBundleCategory>();
    public DbSet<CatalogItemExtra> CatalogItemExtras => Set<CatalogItemExtra>();
    public DbSet<CatalogItemExtraGroup> CatalogItemExtraGroups => Set<CatalogItemExtraGroup>();
    public DbSet<CatalogItemExtraGroupItem> CatalogItemExtraGroupItems => Set<CatalogItemExtraGroupItem>();
    public DbSet<CatalogItemExtraGroupLink> CatalogItemExtraGroupLinks => Set<CatalogItemExtraGroupLink>();
    public DbSet<CatalogItemOption> CatalogItemOptions => Set<CatalogItemOption>();
    public DbSet<CatalogItemOptionGroup> CatalogItemOptionGroups => Set<CatalogItemOptionGroup>();
    public DbSet<CatalogItemOptionGroupItem> CatalogItemOptionGroupItems => Set<CatalogItemOptionGroupItem>();
    public DbSet<CatalogItemOptionGroupLink> CatalogItemOptionGroupLinks => Set<CatalogItemOptionGroupLink>();
    public DbSet<CatalogItemCategory> CatalogItemCategories => Set<CatalogItemCategory>();
    public DbSet<CatalogItemInstance> CatalogItemInstances => Set<CatalogItemInstance>();
    public DbSet<CatalogItemInstanceCategory> CatalogItemInstanceCategories => Set<CatalogItemInstanceCategory>();
    public DbSet<Bundle> Bundles => Set<Bundle>();
    public DbSet<BundleSection> BundleSections => Set<BundleSection>();
    public DbSet<BundleSectionItem> BundleSectionItems => Set<BundleSectionItem>();
    public DbSet<BundleCategory> BundleCategories => Set<BundleCategory>();
    public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
    public DbSet<NFCScan> NFCScans => Set<NFCScan>();

    // Lookup Tables
    public DbSet<ProductLineLookup> ProductLines => Set<ProductLineLookup>();
    public DbSet<SubscriptionStatusLookup> SubscriptionStatuses => Set<SubscriptionStatusLookup>();
    public DbSet<BillingCycleLookup> BillingCycles => Set<BillingCycleLookup>();
    public DbSet<TableCategoryLookup> TableCategories => Set<TableCategoryLookup>();
    public DbSet<NFCTagTypeLookup> NFCTagTypes => Set<NFCTagTypeLookup>();
    public DbSet<NFCTagStatusLookup> NFCTagStatuses => Set<NFCTagStatusLookup>();
    public DbSet<DeviceTypeLookup> DeviceTypes => Set<DeviceTypeLookup>();
    public DbSet<CatalogItemPriceType> CatalogItemPriceTypes => Set<CatalogItemPriceType>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure all DateTime properties to use UTC
        var dateTimeConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        var nullableDateTimeConverter = new Microsoft.EntityFrameworkCore.Storage.ValueConversion.ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : v,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }

        // User Configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.EmailVerified);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Email).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(255).IsRequired();
            entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.EmailVerificationToken).HasMaxLength(255);
            entity.Property(e => e.PasswordResetToken).HasMaxLength(255);
            entity.Property(e => e.Currency).HasMaxLength(3).HasDefaultValue("ZAR");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
        });

        // SubscriptionTier Configuration
        modelBuilder.Entity<SubscriptionTier>(entity =>
        {
            entity.ToTable("SubscriptionTiers");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TierCode).IsUnique();
            entity.HasIndex(e => e.ProductLineId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.TierCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TierName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.DisplayName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.MonthlyPrice).HasPrecision(10, 2);
            entity.Property(e => e.AnnualPrice).HasPrecision(10, 2);
            entity.Property(e => e.AnnualDiscountPercent).HasPrecision(5, 2).HasDefaultValue(15.00m);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.ProductLine)
                .WithMany(p => p.SubscriptionTiers)
                .HasForeignKey(e => e.ProductLineId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // UserSubscription Configuration
        modelBuilder.Entity<UserSubscription>(entity =>
        {
            entity.ToTable("UserSubscriptions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TierId);
            entity.HasIndex(e => e.StatusId);
            entity.HasIndex(e => e.NextBillingDate);
            entity.HasIndex(e => e.TrialEndsAt);

            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.Currency).HasMaxLength(3).HasDefaultValue("ZAR");
            entity.Property(e => e.DiscountPercent).HasPrecision(5, 2).HasDefaultValue(0);
            entity.Property(e => e.LastPaymentAmount).HasPrecision(10, 2);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Subscriptions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tier)
                .WithMany(t => t.Subscriptions)
                .HasForeignKey(e => e.TierId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Status)
                .WithMany(s => s.UserSubscriptions)
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.BillingCycle)
                .WithMany(b => b.UserSubscriptions)
                .HasForeignKey(e => e.BillingCycleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ProductLine)
                .WithMany()
                .HasForeignKey(e => e.ProductLineId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ProductSubscription Configuration
        modelBuilder.Entity<ProductSubscription>(entity =>
        {
            entity.ToTable("ProductSubscriptions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.TierId);
            entity.HasIndex(e => e.ProductType);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => new { e.UserId, e.ProductType }).IsUnique();

            entity.Property(e => e.ProductType).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.BillingCycle).IsRequired();
            entity.Property(e => e.IsTrialActive).HasDefaultValue(true);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Tier)
                .WithMany(t => t.ProductSubscriptions)
                .HasForeignKey(e => e.TierId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Entity Configuration
        modelBuilder.Entity<Entity>(entity =>
        {
            entity.ToTable("Entities");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.EntityType);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.UserId, e.SortOrder });

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Logo).HasMaxLength(500);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Website).HasMaxLength(500);
            entity.Property(e => e.Currency).HasMaxLength(3).HasDefaultValue("ZAR");
            entity.Property(e => e.Timezone).HasMaxLength(50).HasDefaultValue("Africa/Johannesburg");
            entity.Property(e => e.EntityType).IsRequired();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Catalogs)
                .WithOne(c => c.Entity)
                .HasForeignKey(c => c.EntityId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Categories)
                .WithOne(c => c.Entity)
                .HasForeignKey(c => c.EntityId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Category Configuration (entity-level categories)
        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("CategoriesNew");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EntityId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.EntityId, e.SortOrder });

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Icon).HasMaxLength(500);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Entity)
                .WithMany(ent => ent.Categories)
                .HasForeignKey(e => e.EntityId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.CatalogCategories)
                .WithOne(cc => cc.Category)
                .HasForeignKey(cc => cc.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogCategory Configuration (junction table)
        modelBuilder.Entity<CatalogCategory>(entity =>
        {
            entity.ToTable("CatalogCategories");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CatalogId);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => new { e.CatalogId, e.CategoryId }).IsUnique();

            entity.Property(e => e.SortOrder).HasDefaultValue(0);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Catalog)
                .WithMany(c => c.Categories)
                .HasForeignKey(e => e.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.CatalogCategories)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // SubscriptionAddon Configuration
        modelBuilder.Entity<SubscriptionAddon>(entity =>
        {
            entity.ToTable("SubscriptionAddons");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProductLineId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.ProductLineId, e.SortOrder });

            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.MonthlyPrice).HasPrecision(10, 2);
            entity.Property(e => e.ConfigurationJson).HasMaxLength(4000);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
        });

        // SubscriptionTierAddon Configuration
        modelBuilder.Entity<SubscriptionTierAddon>(entity =>
        {
            entity.ToTable("SubscriptionTierAddons");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.SubscriptionTierId, e.AddonId }).IsUnique();
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.DiscountPercentage).HasPrecision(5, 2);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.SubscriptionTier)
                .WithMany(t => t.TierAddons)
                .HasForeignKey(e => e.SubscriptionTierId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Addon)
                .WithMany(a => a.TierAddons)
                .HasForeignKey(e => e.AddonId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // UserSubscriptionAddon Configuration
        modelBuilder.Entity<UserSubscriptionAddon>(entity =>
        {
            entity.ToTable("UserSubscriptionAddons");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserSubscriptionId);
            entity.HasIndex(e => e.AddonId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.UserSubscriptionId, e.AddonId, e.IsActiveAddon });

            entity.Property(e => e.MonthlyPrice).HasPrecision(10, 2);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.UserSubscription)
                .WithMany(s => s.Addons)
                .HasForeignKey(e => e.UserSubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Addon)
                .WithMany(a => a.UserSubscriptionAddons)
                .HasForeignKey(e => e.AddonId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Restaurant Configuration
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.ToTable("Restaurants");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.UserId, e.SortOrder });

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Logo).HasMaxLength(500);
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Country).HasMaxLength(2);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Website).HasMaxLength(500);
            entity.Property(e => e.Currency).HasMaxLength(3).HasDefaultValue("ZAR");
            entity.Property(e => e.Timezone).HasMaxLength(50);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Profile Configuration
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.ToTable("Profiles");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.RestaurantId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Slug).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ProfileType).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Logo).HasMaxLength(500);
            entity.Property(e => e.ContactPhone).HasMaxLength(20);
            entity.Property(e => e.ContactEmail).HasMaxLength(255);
            entity.Property(e => e.Website).HasMaxLength(500);
            entity.Property(e => e.EventModeName).HasMaxLength(100);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.User)
                .WithMany(u => u.Profiles)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Restaurant)
                .WithMany(r => r.Profiles)
                .HasForeignKey(e => e.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Catalog Configuration
        modelBuilder.Entity<Catalog>(entity =>
        {
            entity.ToTable("Catalogs");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.EntityId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.EntityId, e.SortOrder });

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.SortOrder).HasDefaultValue(0);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Entity)
                .WithMany(ent => ent.Catalogs)
                .HasForeignKey(e => e.EntityId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Categories)
                .WithOne(c => c.Catalog)
                .HasForeignKey(c => c.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogItem Configuration
        modelBuilder.Entity<CatalogItem>(entity =>
        {
            entity.ToTable("CatalogItems");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CatalogId);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.BundleId);
            entity.HasIndex(e => e.ParentCatalogItemId);
            entity.HasIndex(e => e.ItemType);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.AvailableInEventMode);

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.PriceOverride).HasPrecision(10, 2);
            entity.Property(e => e.Images).HasMaxLength(5000);
            entity.Property(e => e.Tags).HasMaxLength(2000);
            entity.Property(e => e.ItemType).HasDefaultValue(CatalogItemType.Regular);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Catalog)
                .WithMany(c => c.Items)
                .HasForeignKey(e => e.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Bundle)
                .WithMany()
                .HasForeignKey(e => e.BundleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Self-referencing relationship for item sharing with price overrides
            entity.HasOne(e => e.ParentCatalogItem)
                .WithMany(p => p.ChildCatalogItems)
                .HasForeignKey(e => e.ParentCatalogItemId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Variants)
                .WithOne(v => v.CatalogItem)
                .HasForeignKey(v => v.CatalogItemId)
                .OnDelete(DeleteBehavior.Cascade);

            // Ignore the computed property
            entity.Ignore(e => e.EffectivePrice);
        });

        // CatalogItemVariant Configuration
        modelBuilder.Entity<CatalogItemVariant>(entity =>
        {
            entity.ToTable("CatalogItemVariants");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CatalogItemId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.Sku);

            entity.Property(e => e.Sku).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
            entity.Property(e => e.SizeUnit).HasMaxLength(10);
            entity.Property(e => e.UnitOfMeasure).HasMaxLength(10);
            entity.Property(e => e.Barcode).HasMaxLength(100);
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.Cost).HasPrecision(10, 2);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            // Relationship is configured from CatalogItem side
        });

        // CatalogItemAttribute Configuration
        //modelBuilder.Entity<CatalogItemAttribute>(entity =>
        //{
        //    entity.ToTable("CatalogItemAttributes");
        //    entity.HasKey(e => e.Id);
        //    entity.HasIndex(e => e.Slug).IsUnique();
        //    entity.HasIndex(e => e.IsActive);

        //    entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
        //    entity.Property(e => e.Slug).HasMaxLength(100).IsRequired();

        //    entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        //    entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
        //});

        //// CatalogItemAttributeValue Configuration
        //modelBuilder.Entity<CatalogItemAttributeValue>(entity =>
        //{
        //    entity.ToTable("CatalogItemAttributeValues");
        //    entity.HasKey(e => e.Id);
        //    entity.HasIndex(e => e.AttributeId);
        //    entity.HasIndex(e => e.IsActive);

        //    entity.Property(e => e.Value).HasMaxLength(200).IsRequired();

        //    entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        //    entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

        //    entity.HasOne(e => e.Attribute)
        //        .WithMany(a => a.AttributeValues)
        //        .HasForeignKey(e => e.AttributeId)
        //        .OnDelete(DeleteBehavior.Cascade);
        //});

        //// CatalogItemVariantAttributeValue Configuration
        //modelBuilder.Entity<CatalogItemVariantAttributeValue>(entity =>
        //{
        //    entity.ToTable("CatalogItemVariantAttributeValues");
        //    entity.HasKey(e => e.Id);
        //    entity.HasIndex(e => new { e.VariantId, e.AttributeValueId }).IsUnique();

        //    entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        //    entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

        //    entity.HasOne(e => e.Variant)
        //        .WithMany(v => v.VariantAttributeValues)
        //        .HasForeignKey(e => e.VariantId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    entity.HasOne(e => e.AttributeValue)
        //        .WithMany(av => av.VariantAttributeValues)
        //        .HasForeignKey(e => e.AttributeValueId)
        //        .OnDelete(DeleteBehavior.Cascade);
        //});

        // CatalogItemInventory Configuration
        modelBuilder.Entity<CatalogItemInventory>(entity =>
        {
            entity.ToTable("CatalogItemInventories");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VariantId);
            entity.HasIndex(e => e.LocationId);

            entity.Property(e => e.QtyAvailable).HasPrecision(10, 2);
            entity.Property(e => e.QtyReserved).HasPrecision(10, 2);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Variant)
                .WithMany(v => v.Inventories)
                .HasForeignKey(e => e.VariantId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogItemVariantPrice Configuration
        modelBuilder.Entity<CatalogItemVariantPrice>(entity =>
        {
            entity.ToTable("CatalogItemVariantPrices");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.VariantId);
            entity.HasIndex(e => e.PriceTypeId);
            entity.HasIndex(e => new { e.StartsAt, e.EndsAt });

            entity.Property(e => e.Price).HasPrecision(10, 2);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Variant)
                .WithMany(v => v.Prices)
                .HasForeignKey(e => e.VariantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.PriceType)
                .WithMany()
                .HasForeignKey(e => e.PriceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CatalogBundle Configuration
        modelBuilder.Entity<CatalogBundle>(entity =>
        {
            entity.ToTable("CatalogBundles");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CatalogId);
            entity.HasIndex(e => e.Slug);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.BasePrice).HasPrecision(12, 2);
            entity.Property(e => e.Images).HasMaxLength(5000);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Catalog)
                .WithMany(c => c.Bundles)
                .HasForeignKey(e => e.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Steps)
                .WithOne(s => s.Bundle)
                .HasForeignKey(s => s.BundleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogBundleStep Configuration
        modelBuilder.Entity<CatalogBundleStep>(entity =>
        {
            entity.ToTable("CatalogBundleSteps");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BundleId);
            entity.HasIndex(e => new { e.BundleId, e.StepNumber });

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Bundle)
                .WithMany(b => b.Steps)
                .HasForeignKey(e => e.BundleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.AllowedProducts)
                .WithOne(p => p.Step)
                .HasForeignKey(p => p.StepId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.OptionGroups)
                .WithOne(g => g.Step)
                .HasForeignKey(g => g.StepId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogBundleStepProduct Configuration
        modelBuilder.Entity<CatalogBundleStepProduct>(entity =>
        {
            entity.ToTable("CatalogBundleStepProducts");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.StepId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => new { e.StepId, e.ProductId }).IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Step)
                .WithMany(s => s.AllowedProducts)
                .HasForeignKey(e => e.StepId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogBundleOptionGroup Configuration
        modelBuilder.Entity<CatalogBundleOptionGroup>(entity =>
        {
            entity.ToTable("CatalogBundleOptionGroups");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.StepId);

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Step)
                .WithMany(s => s.OptionGroups)
                .HasForeignKey(e => e.StepId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Options)
                .WithOne(o => o.OptionGroup)
                .HasForeignKey(o => o.OptionGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogBundleOption Configuration
        modelBuilder.Entity<CatalogBundleOption>(entity =>
        {
            entity.ToTable("CatalogBundleOptions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.OptionGroupId);

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.PriceModifier).HasPrecision(12, 2).HasDefaultValue(0M);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.OptionGroup)
                .WithMany(g => g.Options)
                .HasForeignKey(e => e.OptionGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogBundleCategory Configuration
        modelBuilder.Entity<CatalogBundleCategory>(entity =>
        {
            entity.ToTable("CatalogBundleCategories");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CatalogBundleId, e.CategoryId }).IsUnique();
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.CatalogBundle)
                .WithMany(b => b.CatalogBundleCategories)
                .HasForeignKey(e => e.CatalogBundleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // RestaurantTable Configuration
        modelBuilder.Entity<RestaurantTable>(entity =>
        {
            entity.ToTable("RestaurantTables");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProfileId);
            entity.HasIndex(e => e.NFCTagCode).IsUnique();
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.TableName).HasMaxLength(100);
            entity.Property(e => e.NFCTagCode).HasMaxLength(50);
            entity.Property(e => e.Images).HasMaxLength(2000); // JSON array

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Profile)
                .WithMany(p => p.RestaurantTables)
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.TableCategory)
                .WithMany(c => c.RestaurantTables)
                .HasForeignKey(e => e.TableCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.NFCTagType)
                .WithMany(t => t.RestaurantTables)
                .HasForeignKey(e => e.NFCTagTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.NFCTagStatus)
                .WithMany(s => s.RestaurantTables)
                .HasForeignKey(e => e.NFCTagStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ensure unique table numbers per profile
            entity.HasIndex(e => new { e.ProfileId, e.TableNumber }).IsUnique();
        });

        // NFCScan Configuration
        modelBuilder.Entity<NFCScan>(entity =>
        {
            entity.ToTable("NFCScans");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ProfileId, e.ScannedAt });
            entity.HasIndex(e => new { e.TableId, e.ScannedAt });
            entity.HasIndex(e => e.NFCTagCode);
            entity.HasIndex(e => e.ScannedAt);

            entity.Property(e => e.NFCTagCode).HasMaxLength(50).IsRequired();
            entity.Property(e => e.IPAddress).HasMaxLength(45);
            entity.Property(e => e.Country).HasMaxLength(2);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.SessionId).HasMaxLength(100);

            entity.Property(e => e.ScannedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Profile)
                .WithMany()
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Table)
                .WithMany(t => t.NFCScans)
                .HasForeignKey(e => e.TableId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.DeviceType)
                .WithMany(d => d.NFCScans)
                .HasForeignKey(e => e.DeviceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CatalogItemExtra Configuration
        modelBuilder.Entity<CatalogItemExtra>(entity =>
        {
            entity.ToTable("CatalogItemExtras");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CatalogId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Code).HasMaxLength(100);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.BasePrice).HasPrecision(10, 2);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Catalog)
                .WithMany()
                .HasForeignKey(e => e.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogItemExtraGroup Configuration
        modelBuilder.Entity<CatalogItemExtraGroup>(entity =>
        {
            entity.ToTable("CatalogItemExtraGroups");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CatalogId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Catalog)
                .WithMany()
                .HasForeignKey(e => e.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogItemExtraGroupItem Configuration
        modelBuilder.Entity<CatalogItemExtraGroupItem>(entity =>
        {
            entity.ToTable("CatalogItemExtraGroupItems");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.ExtraGroupId, e.ExtraId }).IsUnique();

            entity.Property(e => e.PriceOverride).HasPrecision(10, 2);

            entity.HasOne(e => e.ExtraGroup)
                .WithMany(g => g.GroupItems)
                .HasForeignKey(e => e.ExtraGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Extra)
                .WithMany(ex => ex.ExtraGroupItems)
                .HasForeignKey(e => e.ExtraId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CatalogItemExtraGroupLink Configuration
        modelBuilder.Entity<CatalogItemExtraGroupLink>(entity =>
        {
            entity.ToTable("CatalogItemExtraGroupLinks");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CatalogItemId);
            entity.HasIndex(e => e.ExtraGroupId);
            entity.HasIndex(e => e.VariantId);

            entity.HasOne(e => e.CatalogItem)
                .WithMany(c => c.ExtraGroupLinks)
                .HasForeignKey(e => e.CatalogItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ExtraGroup)
                .WithMany(g => g.ItemLinks)
                .HasForeignKey(e => e.ExtraGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Variant)
                .WithMany()
                .HasForeignKey(e => e.VariantId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // CatalogItemOption Configuration
        modelBuilder.Entity<CatalogItemOption>(entity =>
        {
            entity.ToTable("CatalogItemOptions");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CatalogId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.PriceModifier).HasPrecision(10, 2);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Catalog)
                .WithMany()
                .HasForeignKey(e => e.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogItemOptionGroup Configuration
        modelBuilder.Entity<CatalogItemOptionGroup>(entity =>
        {
            entity.ToTable("CatalogItemOptionGroups");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.CatalogId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(1000);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Catalog)
                .WithMany()
                .HasForeignKey(e => e.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogItemOptionGroupItem Configuration
        modelBuilder.Entity<CatalogItemOptionGroupItem>(entity =>
        {
            entity.ToTable("CatalogItemOptionGroupItems");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.OptionGroupId, e.OptionId }).IsUnique();

            entity.HasOne(e => e.OptionGroup)
                .WithMany(g => g.GroupItems)
                .HasForeignKey(e => e.OptionGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Option)
                .WithMany(o => o.OptionGroupItems)
                .HasForeignKey(e => e.OptionId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CatalogItemOptionGroupLink Configuration
        modelBuilder.Entity<CatalogItemOptionGroupLink>(entity =>
        {
            entity.ToTable("CatalogItemOptionGroupLinks");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CatalogItemId);
            entity.HasIndex(e => e.OptionGroupId);
            entity.HasIndex(e => e.VariantId);

            entity.HasOne(e => e.CatalogItem)
                .WithMany(c => c.OptionGroupLinks)
                .HasForeignKey(e => e.CatalogItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.OptionGroup)
                .WithMany(g => g.ItemLinks)
                .HasForeignKey(e => e.OptionGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Variant)
                .WithMany()
                .HasForeignKey(e => e.VariantId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Lookup Table Configurations
        ConfigureLookupTable<ProductLineLookup>(modelBuilder, "ProductLines");
        ConfigureLookupTable<SubscriptionStatusLookup>(modelBuilder, "SubscriptionStatuses");
        ConfigureLookupTable<BillingCycleLookup>(modelBuilder, "BillingCycles");
        ConfigureLookupTable<TableCategoryLookup>(modelBuilder, "TableCategories");
        ConfigureLookupTable<NFCTagTypeLookup>(modelBuilder, "NFCTagTypes");
        ConfigureLookupTable<NFCTagStatusLookup>(modelBuilder, "NFCTagStatuses");
        ConfigureLookupTable<DeviceTypeLookup>(modelBuilder, "DeviceTypes");

        // CatalogItemPriceType lookup table
        modelBuilder.Entity<CatalogItemPriceType>(entity =>
        {
            entity.ToTable("CatalogItemPriceTypes");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(50).IsRequired();
        });

        // CatalogItemCategory Configuration
        modelBuilder.Entity<CatalogItemCategory>(entity =>
        {
            entity.ToTable("CatalogItemCategory");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.CatalogItemId, e.CategoryId }).IsUnique();
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
            
            entity.Property(e => e.CatalogItemId).HasColumnName("CatalogItemId");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryId");

            entity.HasOne(e => e.CatalogItem)
                .WithMany(c => c.CatalogItemCategories)
                .HasForeignKey(e => e.CatalogItemId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.CatalogItemCategories)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogItemInstance Configuration (Phase 2 - Live Sync)
        modelBuilder.Entity<CatalogItemInstance>(entity =>
        {
            entity.ToTable("CatalogItemInstances");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CatalogId);
            entity.HasIndex(e => e.LibraryItemId);
            entity.HasIndex(e => new { e.CatalogId, e.LibraryItemId });
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsVisible);

            entity.Property(e => e.PriceOverride).HasPrecision(10, 2);
            entity.Property(e => e.NameOverride).HasMaxLength(255);
            entity.Property(e => e.DescriptionOverride).HasMaxLength(2000);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Catalog)
                .WithMany()
                .HasForeignKey(e => e.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.LibraryItem)
                .WithMany()
                .HasForeignKey(e => e.LibraryItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CatalogItemInstanceCategory Configuration (Phase 2 - Category mapping for instances)
        modelBuilder.Entity<CatalogItemInstanceCategory>(entity =>
        {
            entity.ToTable("CatalogItemInstanceCategories");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.InstanceId, e.CategoryId }).IsUnique();
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Instance)
                .WithMany(i => i.Categories)
                .HasForeignKey(e => e.InstanceId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Bundle Configuration
        modelBuilder.Entity<Bundle>(entity =>
        {
            entity.ToTable("Bundles");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CatalogId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.BasePrice).HasPrecision(12, 2);
            entity.Property(e => e.Images).HasMaxLength(5000);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Catalog)
                .WithMany()
                .HasForeignKey(e => e.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Sections)
                .WithOne(s => s.Bundle)
                .HasForeignKey(s => s.BundleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // BundleSection Configuration
        modelBuilder.Entity<BundleSection>(entity =>
        {
            entity.ToTable("BundleSections");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.BundleId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Bundle)
                .WithMany(b => b.Sections)
                .HasForeignKey(e => e.BundleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Items)
                .WithOne(i => i.Section)
                .HasForeignKey(i => i.SectionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // BundleSectionItem Configuration
        modelBuilder.Entity<BundleSectionItem>(entity =>
        {
            entity.ToTable("BundleSectionItems");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SectionId);
            entity.HasIndex(e => e.CatalogItemId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Quantity).HasPrecision(10, 2);
            entity.Property(e => e.PriceAdjustment).HasPrecision(10, 2);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Section)
                .WithMany(s => s.Items)
                .HasForeignKey(e => e.SectionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CatalogItem)
                .WithMany()
                .HasForeignKey(e => e.CatalogItemId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // BundleCategory Configuration
        modelBuilder.Entity<BundleCategory>(entity =>
        {
            entity.ToTable("BundleCategories");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.BundleId, e.CategoryId }).IsUnique();
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Bundle)
                .WithMany(b => b.Categories)
                .HasForeignKey(e => e.BundleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private void ConfigureLookupTable<T>(ModelBuilder modelBuilder, string tableName) where T : EnumLookup
    {
        modelBuilder.Entity<T>(entity =>
        {
            entity.ToTable(tableName);
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
        });
    }
}