namespace BizBio.Core.DTOs;

/// <summary>
/// MRD-style menu response - efficient structure with array indexing
/// </summary>
public class MenuResponseDto
{
    public int Id { get; set; }
    public int? CatalogId { get; set; }
    public string CatalogType { get; set; } = "Menu"; // or "Restaurant"
    public string Name { get; set; } = null!;
    public string Status { get; set; } = "PUBLISHED";

    public List<MenuSectionDto> Sections { get; set; } = new();

    // MRD Pattern: Define options/extras once at menu level
    // Variants reference them by array index
    public List<ExtraGroupDto> Extras { get; set; } = new();
    public List<OptionGroupDto> Options { get; set; } = new();
}

public class MenuSectionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }
    public bool Visible { get; set; } = true;

    public List<MenuItemDto> Items { get; set; } = new();

    public AvailabilityDto Availability { get; set; } = new() { Status = "visible" };
    public string? HeaderImageUrl { get; set; }
    public string? HeaderStyling { get; set; }
}

public class MenuItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int OrderIndex { get; set; }

    public List<VariantDto> Variants { get; set; } = new();

    public AvailabilityDto Availability { get; set; } = new() { Status = "visible" };
    public BusinessIdentityDto? BusinessIdentity { get; set; }
    public MediaDto? Media { get; set; }
    public bool Featured { get; set; }
}

public class VariantDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool Default { get; set; }
    public decimal Price { get; set; }
    public decimal? OldPrice { get; set; }

    // MRD Pattern: Reference options/extras by array index
    // [0, 1, 2] means this variant uses extras at index 0, 1, and 2 from the Extras[] array
    public List<int> OptionIndices { get; set; } = new();
    public List<int> ExtraIndices { get; set; } = new();

    public AvailabilityDto Availability { get; set; } = new() { Status = "visible" };
    public BusinessIdentityDto? BusinessIdentity { get; set; }
    public int? PercentageDiscount { get; set; }
    public string? MenuDealAction { get; set; }
}

/// <summary>
/// Options are REQUIRED choices (min_select >= 1)
/// Examples: "Choose a drink", "Select condiments", "Pick a side"
/// </summary>
public class OptionGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Label { get; set; } = null!; // "Which cold drink flavour would you like?"
    public int MinimumSelect { get; set; } = 1;
    public int MaximumSelect { get; set; } = 1;
    public int OrderIndex { get; set; }

    public List<OptionItemDto> Items { get; set; } = new();

    public BusinessIdentityDto? BusinessIdentity { get; set; }
}

/// <summary>
/// Extras are OPTIONAL add-ons (min_select = 0)
/// Examples: "Add extra cheese", "Would you like more?", "Upsells"
/// </summary>
public class ExtraGroupDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Label { get; set; } = null!; // "Would you like to add more?"
    public int MinimumSelect { get; set; } = 0;
    public int MaximumSelect { get; set; } = 99;
    public int OrderIndex { get; set; }

    public List<ExtraItemDto> Items { get; set; } = new();

    public BusinessIdentityDto? BusinessIdentity { get; set; }
}

public class OptionItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool Default { get; set; }
    public decimal Price { get; set; } // Additional cost (can be 0)
    public int OrderIndex { get; set; }

    public AvailabilityDto Availability { get; set; } = new() { Status = "visible" };
    public BusinessIdentityDto? BusinessIdentity { get; set; }
}

public class ExtraItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool Default { get; set; }
    public decimal Price { get; set; }
    public int OrderIndex { get; set; }

    public AvailabilityDto Availability { get; set; } = new() { Status = "visible" };
    public BusinessIdentityDto? BusinessIdentity { get; set; }
}

public class AvailabilityDto
{
    public string Status { get; set; } = "visible"; // "visible", "hidden", "sold_out"
    public string Description { get; set; } = "Available (visible)";
}

public class BusinessIdentityDto
{
    public string? Internal { get; set; }
    public string? External { get; set; }
}

public class MediaDto
{
    public string Provider { get; set; } = "thumbor";
    public string BaseUrl { get; set; } = null!;
    public string? Filename { get; set; }
    public string? RawImage { get; set; }
    public string MimeType { get; set; } = "image";
}
