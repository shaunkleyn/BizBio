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
    public DbSet<Profile> Profiles => Set<Profile>();
    public DbSet<Catalog> Catalogs => Set<Catalog>();
    public DbSet<CatalogCategory> Categories => Set<CatalogCategory>();
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<CatalogItemVariant> CatalogItemVariants => Set<CatalogItemVariant>();
    public DbSet<CatalogItemAttribute> CatalogItemAttributes => Set<CatalogItemAttribute>();
    public DbSet<CatalogItemAttributeValue> CatalogItemAttributeValues => Set<CatalogItemAttributeValue>();
    public DbSet<CatalogItemVariantAttributeValue> CatalogItemVariantAttributeValues => Set<CatalogItemVariantAttributeValue>();
    public DbSet<CatalogItemInventory> CatalogItemInventories => Set<CatalogItemInventory>();
    public DbSet<CatalogItemVariantPrice> CatalogItemVariantPrices => Set<CatalogItemVariantPrice>();
    public DbSet<CatalogBundle> CatalogBundles => Set<CatalogBundle>();
    public DbSet<CatalogBundleStep> CatalogBundleSteps => Set<CatalogBundleStep>();
    public DbSet<CatalogBundleStepProduct> CatalogBundleStepProducts => Set<CatalogBundleStepProduct>();
    public DbSet<CatalogBundleOptionGroup> CatalogBundleOptionGroups => Set<CatalogBundleOptionGroup>();
    public DbSet<CatalogBundleOption> CatalogBundleOptions => Set<CatalogBundleOption>();
    public DbSet<CatalogItemExtra> CatalogItemExtras => Set<CatalogItemExtra>();
    public DbSet<CatalogItemExtraGroup> CatalogItemExtraGroups => Set<CatalogItemExtraGroup>();
    public DbSet<CatalogItemExtraGroupItem> CatalogItemExtraGroupItems => Set<CatalogItemExtraGroupItem>();
    public DbSet<CatalogItemExtraGroupLink> CatalogItemExtraGroupLinks => Set<CatalogItemExtraGroupLink>();
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
        });

        // Profile Configuration
        modelBuilder.Entity<Profile>(entity =>
        {
            entity.ToTable("Profiles");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.UserId);
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
        });

        // Catalog Configuration
        modelBuilder.Entity<Catalog>(entity =>
        {
            entity.ToTable("Catalogs");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ProfileId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Profile)
                .WithMany(p => p.Catalogs)
                .HasForeignKey(e => e.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(e => e.Categories)
                .WithOne(c => c.Catalog)
                .HasForeignKey(c => c.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Category Configuration
        modelBuilder.Entity<CatalogCategory>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CatalogId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => new { e.CatalogId, e.SortOrder });

            entity.Property(e => e.Name).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Icon).HasMaxLength(100);
            entity.Property(e => e.Images).HasMaxLength(5000);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Catalog)
                .WithMany(c => c.Categories)
                .HasForeignKey(e => e.CatalogId)
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
            entity.HasIndex(e => e.ItemType);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.AvailableInEventMode);

            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.Images).HasMaxLength(2000); // JSON array stored as string
            entity.Property(e => e.ItemType).HasDefaultValue(CatalogItemType.Regular);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Catalog)
                .WithMany(c => c.Items)
                .HasForeignKey(e => e.CatalogId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne<CatalogCategory>()
                .WithMany(cat => cat.Items)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.Bundle)
                .WithMany()
                .HasForeignKey(e => e.BundleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(e => e.Variants)
                .WithOne(v => v.CatalogItem)
                .HasForeignKey(v => v.CatalogItemId)
                .OnDelete(DeleteBehavior.Cascade);
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
        modelBuilder.Entity<CatalogItemAttribute>(entity =>
        {
            entity.ToTable("CatalogItemAttributes");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Slug).HasMaxLength(100).IsRequired();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");
        });

        // CatalogItemAttributeValue Configuration
        modelBuilder.Entity<CatalogItemAttributeValue>(entity =>
        {
            entity.ToTable("CatalogItemAttributeValues");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.AttributeId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Value).HasMaxLength(200).IsRequired();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Attribute)
                .WithMany(a => a.AttributeValues)
                .HasForeignKey(e => e.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CatalogItemVariantAttributeValue Configuration
        modelBuilder.Entity<CatalogItemVariantAttributeValue>(entity =>
        {
            entity.ToTable("CatalogItemVariantAttributeValues");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.VariantId, e.AttributeValueId }).IsUnique();

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Variant)
                .WithMany(v => v.VariantAttributeValues)
                .HasForeignKey(e => e.VariantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.AttributeValue)
                .WithMany(av => av.VariantAttributeValues)
                .HasForeignKey(e => e.AttributeValueId)
                .OnDelete(DeleteBehavior.Cascade);
        });

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
                .WithMany()
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
                .WithMany()
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