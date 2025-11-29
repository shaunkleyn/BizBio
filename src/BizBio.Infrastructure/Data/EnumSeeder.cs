using System.ComponentModel;
using BizBio.Core.Entities.Lookups;
using BizBio.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace BizBio.Infrastructure.Data;

public static class EnumSeeder
{
    /// <summary>
    /// Seeds all enum lookup tables with values from the enums
    /// </summary>
    public static async Task SeedEnumLookupsAsync(ApplicationDbContext context)
    {
        await SeedEnumLookup<ProductLine, ProductLineLookup>(context, context.ProductLines);
        await SeedEnumLookup<SubscriptionStatus, SubscriptionStatusLookup>(context, context.SubscriptionStatuses);
        await SeedEnumLookup<BillingCycle, BillingCycleLookup>(context, context.BillingCycles);
        await SeedEnumLookup<TableCategory, TableCategoryLookup>(context, context.TableCategories);
        await SeedEnumLookup<NFCTagType, NFCTagTypeLookup>(context, context.NFCTagTypes);
        await SeedEnumLookup<NFCTagStatus, NFCTagStatusLookup>(context, context.NFCTagStatuses);
        await SeedEnumLookup<DeviceType, DeviceTypeLookup>(context, context.DeviceTypes);
    }

    /// <summary>
    /// Seeds a specific enum lookup table with values from the enum
    /// </summary>
    private static async Task SeedEnumLookup<TEnum, TLookup>(
        ApplicationDbContext context,
        DbSet<TLookup> dbSet)
        where TEnum : struct, Enum
        where TLookup : EnumLookup, new()
    {
        var enumValues = Enum.GetValues<TEnum>();
        var existingLookups = await dbSet.ToListAsync();

        var sortOrder = 1;
        foreach (var enumValue in enumValues)
        {
            var enumName = enumValue.ToString();
            var existing = existingLookups.FirstOrDefault(l => l.Name == enumName);

            if (existing == null)
            {
                // Add new lookup entry
                var newLookup = new TLookup
                {
                    Name = enumName,
                    Description = GetEnumDescription(enumValue),
                    SortOrder = sortOrder,
                    IsActive = true
                };
                await dbSet.AddAsync(newLookup);
                Console.WriteLine($"Added new {typeof(TLookup).Name}: {enumName}");
            }
            else
            {
                // Update existing entry if needed
                var description = GetEnumDescription(enumValue);
                if (existing.Description != description)
                {
                    existing.Description = description;
                    Console.WriteLine($"Updated {typeof(TLookup).Name}: {enumName}");
                }
            }

            sortOrder++;
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Gets the description from the Description attribute if available
    /// </summary>
    private static string? GetEnumDescription(Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        if (field == null) return null;

        var attribute = (DescriptionAttribute?)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
        return attribute?.Description;
    }
}
