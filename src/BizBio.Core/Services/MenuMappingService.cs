namespace BizBio.Core.Services;

using BizBio.Core.DTOs;
using BizBio.Core.Entities;

/// <summary>
/// Maps database entities to MRD-style menu DTOs with array indexing pattern
/// </summary>
public class MenuMappingService
{
    /// <summary>
    /// Converts a Catalog to MRD-style MenuResponseDto
    /// Key feature: Options/Extras are defined once, variants reference them by index
    /// </summary>
    public MenuResponseDto MapCatalogToMenu(Catalog catalog)
    {
        var menu = new MenuResponseDto
        {
            Id = catalog.Id,
            CatalogId = catalog.Id,
            Name = catalog.Name,
            Status = "PUBLISHED"
        };

        // Step 1: Build the global options/extras arrays
        // These will be referenced by variants via array indices
        var (optionGroups, extraGroups, optionGroupIndexMap, extraGroupIndexMap) =
            BuildGlobalModifierArrays(catalog);

        menu.Options = optionGroups;
        menu.Extras = extraGroups;

        // Step 2: Map categories to sections
        menu.Sections = catalog.Categories
            .OrderBy(c => c.SortOrder)
            .Select(category => MapCategoryToSection(
                category,
                optionGroupIndexMap,
                extraGroupIndexMap))
            .ToList();

        return menu;
    }

    /// <summary>
    /// Builds the global options[] and extras[] arrays at menu level
    /// Returns maps of GroupId -> ArrayIndex for efficient lookup
    /// </summary>
    private (
        List<OptionGroupDto> options,
        List<ExtraGroupDto> extras,
        Dictionary<int, int> optionIndexMap,
        Dictionary<int, int> extraIndexMap
    ) BuildGlobalModifierArrays(Catalog catalog)
    {
        var options = new List<OptionGroupDto>();
        var extras = new List<ExtraGroupDto>();
        var optionIndexMap = new Dictionary<int, int>();
        var extraIndexMap = new Dictionary<int, int>();

        // Get all unique extra groups used in this catalog
        var allExtraGroups = catalog.Items
            .SelectMany(item => item.ExtraGroupLinks)
            .Select(link => link.ExtraGroup)
            .DistinctBy(g => g.Id)
            .OrderBy(g => g.DisplayOrder);

        int optionIndex = 0;
        int extraIndex = 0;

        foreach (var group in allExtraGroups)
        {
            // Separate into Options (required) vs Extras (optional)
            // Options have MinRequired >= 1
            if (group.MinRequired >= 1)
            {
                options.Add(MapToOptionGroup(group));
                optionIndexMap[group.Id] = optionIndex++;
            }
            else
            {
                extras.Add(MapToExtraGroup(group));
                extraIndexMap[group.Id] = extraIndex++;
            }
        }

        return (options, extras, optionIndexMap, extraIndexMap);
    }

    private MenuSectionDto MapCategoryToSection(
        CatalogCategory category,
        Dictionary<int, int> optionIndexMap,
        Dictionary<int, int> extraIndexMap)
    {
        return new MenuSectionDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            OrderIndex = category.SortOrder,
            Visible = true,
            Items = category.CatalogItemCategories
                .Select(cic => cic.CatalogItem)
                .OrderBy(item => item.SortOrder)
                .Select(item => MapItemToMenuItemDto(item, optionIndexMap, extraIndexMap))
                .ToList()
        };
    }

    private MenuItemDto MapItemToMenuItemDto(
        CatalogItem item,
        Dictionary<int, int> optionIndexMap,
        Dictionary<int, int> extraIndexMap)
    {
        return new MenuItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            OrderIndex = item.SortOrder,
            Featured = false,
            Variants = item.Variants
                .OrderBy(v => v.IsDefault ? 0 : 1)
                .ThenBy(v => v.Price)
                .Select(v => MapVariantToDto(v, item, optionIndexMap, extraIndexMap))
                .ToList(),
            Media = ParseMediaFromImages(item.Images),
            BusinessIdentity = new BusinessIdentityDto
            {
                Internal = $"ITEM/{item.Id}",
                External = item.SourceLibraryItemId?.ToString()
            }
        };
    }

    private VariantDto MapVariantToDto(
        CatalogItemVariant variant,
        CatalogItem item,
        Dictionary<int, int> optionIndexMap,
        Dictionary<int, int> extraIndexMap)
    {
        // Build option_indices and extra_indices arrays
        var optionIndices = new List<int>();
        var extraIndices = new List<int>();

        foreach (var link in item.ExtraGroupLinks)
        {
            // Check if this extra group applies to this specific variant
            // or if it's linked to the item level (applies to all variants)
            if (link.VariantId == null || link.VariantId == variant.Id)
            {
                var groupId = link.ExtraGroupId;

                // Add to appropriate indices list
                if (optionIndexMap.ContainsKey(groupId))
                {
                    optionIndices.Add(optionIndexMap[groupId]);
                }
                else if (extraIndexMap.ContainsKey(groupId))
                {
                    extraIndices.Add(extraIndexMap[groupId]);
                }
            }
        }

        return new VariantDto
        {
            Id = variant.Id,
            Name = variant.Title,
            Default = variant.IsDefault,
            Price = variant.Price,
            OldPrice = null, // Calculate from promotional pricing if needed
            OptionIndices = optionIndices.OrderBy(i => i).ToList(),
            ExtraIndices = extraIndices.OrderBy(i => i).ToList(),
            BusinessIdentity = new BusinessIdentityDto
            {
                External = variant.Sku
            }
        };
    }

    private OptionGroupDto MapToOptionGroup(CatalogItemExtraGroup group)
    {
        return new OptionGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Label = group.Description ?? $"Please select {group.Name.ToLower()}",
            MinimumSelect = group.MinRequired,
            MaximumSelect = group.MaxAllowed > 0 ? group.MaxAllowed : 1,
            OrderIndex = group.DisplayOrder,
            Items = group.GroupItems
                .OrderBy(gi => gi.DisplayOrder)
                .Select(gi => new OptionItemDto
                {
                    Id = gi.Extra.Id,
                    Name = gi.Extra.Name,
                    Default = false,
                    Price = gi.PriceOverride ?? gi.Extra.BasePrice,
                    OrderIndex = gi.DisplayOrder,
                    BusinessIdentity = new BusinessIdentityDto
                    {
                        External = gi.Extra.Code
                    }
                })
                .ToList()
        };
    }

    private ExtraGroupDto MapToExtraGroup(CatalogItemExtraGroup group)
    {
        return new ExtraGroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Label = group.Description ?? "Would you like to add more?",
            MinimumSelect = group.MinRequired,
            MaximumSelect = group.MaxAllowed > 0 ? group.MaxAllowed : 99,
            OrderIndex = group.DisplayOrder,
            Items = group.GroupItems
                .OrderBy(gi => gi.DisplayOrder)
                .Select(gi => new ExtraItemDto
                {
                    Id = gi.Extra.Id,
                    Name = gi.Extra.Name,
                    Default = false,
                    Price = gi.PriceOverride ?? gi.Extra.BasePrice,
                    OrderIndex = gi.DisplayOrder,
                    BusinessIdentity = new BusinessIdentityDto
                    {
                        External = gi.Extra.Code
                    }
                })
                .ToList()
        };
    }

    private MediaDto? ParseMediaFromImages(string? imagesJson)
    {
        if (string.IsNullOrEmpty(imagesJson))
            return null;

        try
        {
            // Assuming Images is JSON array: ["url1.jpg", "url2.jpg"]
            var urls = System.Text.Json.JsonSerializer.Deserialize<List<string>>(imagesJson);
            var firstImage = urls?.FirstOrDefault();

            if (string.IsNullOrEmpty(firstImage))
                return null;

            return new MediaDto
            {
                Provider = "local",
                BaseUrl = "/images", // Configure this based on your CDN
                RawImage = firstImage,
                MimeType = "image"
            };
        }
        catch
        {
            return null;
        }
    }
}
