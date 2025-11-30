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
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<RestaurantTable> RestaurantTables => Set<RestaurantTable>();
    public DbSet<NFCScan> NFCScans => Set<NFCScan>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductAddOn> ProductAddOns => Set<ProductAddOn>();

    // Lookup Tables
    public DbSet<ProductLineLookup> ProductLines => Set<ProductLineLookup>();
    public DbSet<ProductTypeLookup> ProductTypes => Set<ProductTypeLookup>();
    public DbSet<ProductCategoryLookup> ProductCategories => Set<ProductCategoryLookup>();
    public DbSet<AddOnTypeLookup> AddOnTypes => Set<AddOnTypeLookup>();
    public DbSet<SubscriptionStatusLookup> SubscriptionStatuses => Set<SubscriptionStatusLookup>();
    public DbSet<BillingCycleLookup> BillingCycles => Set<BillingCycleLookup>();
    public DbSet<TableCategoryLookup> TableCategories => Set<TableCategoryLookup>();
    public DbSet<NFCTagTypeLookup> NFCTagTypes => Set<NFCTagTypeLookup>();
    public DbSet<NFCTagStatusLookup> NFCTagStatuses => Set<NFCTagStatusLookup>();
    public DbSet<DeviceTypeLookup> DeviceTypes => Set<DeviceTypeLookup>();

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
        });

        // CatalogItem Configuration
        modelBuilder.Entity<CatalogItem>(entity =>
        {
            entity.ToTable("CatalogItems");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.CatalogId);
            entity.HasIndex(e => e.CategoryId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.AvailableInEventMode);

            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.Images).HasMaxLength(2000); // JSON array stored as string

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.Catalog)
                .WithMany(c => c.Items)
                .HasForeignKey(e => e.CatalogId)
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

        // Product Configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.HasIndex(e => e.ProductTypeId);
            entity.HasIndex(e => e.ProductCategoryId);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsFeatured);

            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.SKU).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.MonthlyPrice).HasPrecision(10, 2);
            entity.Property(e => e.AnnualPrice).HasPrecision(10, 2);
            entity.Property(e => e.AnnualDiscountPercent).HasPrecision(5, 2);
            entity.Property(e => e.SalePrice).HasPrecision(10, 2);
            entity.Property(e => e.CostPrice).HasPrecision(10, 2);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.ProductType)
                .WithMany(p => p.Products)
                .HasForeignKey(e => e.ProductTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ProductCategory)
                .WithMany(p => p.Products)
                .HasForeignKey(e => e.ProductCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.ProductLine)
                .WithMany()
                .HasForeignKey(e => e.ProductLineId)
                .OnDelete(DeleteBehavior.SetNull);

            // Many-to-many relationship with ProductAddOns
            entity.HasMany(e => e.CompatibleAddOns)
                .WithMany(a => a.ApplicableProducts)
                .UsingEntity(j => j.ToTable("ProductAddOnMappings"));
        });

        // ProductAddOn Configuration
        modelBuilder.Entity<ProductAddOn>(entity =>
        {
            entity.ToTable("ProductAddOns");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.HasIndex(e => e.AddOnTypeId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.IsActive);

            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.SKU).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.MonthlyPrice).HasPrecision(10, 2);
            entity.Property(e => e.AnnualPrice).HasPrecision(10, 2);

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP");

            entity.HasOne(e => e.AddOnType)
                .WithMany(a => a.AddOns)
                .HasForeignKey(e => e.AddOnTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Product)
                .WithMany(p => p.AddOns)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Lookup Table Configurations
        ConfigureLookupTable<ProductLineLookup>(modelBuilder, "ProductLines");
        ConfigureLookupTable<ProductTypeLookup>(modelBuilder, "ProductTypes");
        ConfigureLookupTable<ProductCategoryLookup>(modelBuilder, "ProductCategories");
        ConfigureLookupTable<AddOnTypeLookup>(modelBuilder, "AddOnTypes");
        ConfigureLookupTable<SubscriptionStatusLookup>(modelBuilder, "SubscriptionStatuses");
        ConfigureLookupTable<BillingCycleLookup>(modelBuilder, "BillingCycles");
        ConfigureLookupTable<TableCategoryLookup>(modelBuilder, "TableCategories");
        ConfigureLookupTable<NFCTagTypeLookup>(modelBuilder, "NFCTagTypes");
        ConfigureLookupTable<NFCTagStatusLookup>(modelBuilder, "NFCTagStatuses");
        ConfigureLookupTable<DeviceTypeLookup>(modelBuilder, "DeviceTypes");
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