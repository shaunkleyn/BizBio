namespace BizBio.Core.DTOs;

/// <summary>
/// Full catalog details for editing
/// </summary>
public class CatalogDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public List<CategoryDetailDto> Categories { get; set; } = new();
    public List<ItemDetailDto> Items { get; set; } = new();
    public List<BundleDetailDto> Bundles { get; set; } = new();
}

/// <summary>
/// Category information for menu editor
/// </summary>
public class CategoryDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; }
    public int ItemCount { get; set; }
}

/// <summary>
/// Item information for menu editor
/// </summary>
public class ItemDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public List<string> Images { get; set; } = new();
    public List<int> CategoryIds { get; set; } = new();
    public int SortOrder { get; set; }
    public int VariantCount { get; set; }
    public bool HasOptions { get; set; }
    public bool HasExtras { get; set; }
}

/// <summary>
/// Bundle information for menu editor
/// </summary>
public class BundleDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal BasePrice { get; set; }
    public List<string> Images { get; set; } = new();
    public List<int> CategoryIds { get; set; } = new();
    public int SortOrder { get; set; }
}

/// <summary>
/// DTO for reordering categories
/// </summary>
public class ReorderDto
{
    public List<ReorderItemDto> Items { get; set; } = new();
}

/// <summary>
/// Single item to reorder
/// </summary>
public class ReorderItemDto
{
    public int Id { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>
/// DTO for reordering items with optional category change
/// </summary>
public class ReorderItemsDto
{
    public List<ReorderItemWithCategoryDto> Items { get; set; } = new();
}

/// <summary>
/// Single item to reorder with category
/// </summary>
public class ReorderItemWithCategoryDto
{
    public int Id { get; set; }
    public int? CategoryId { get; set; }
    public int SortOrder { get; set; }
}

/// <summary>
/// DTO for adding library item to catalog
/// </summary>
public class AddItemToCatalogDto
{
    public int LibraryItemId { get; set; }
    public List<int> CategoryIds { get; set; } = new();
    public int SortOrder { get; set; }
}

/// <summary>
/// DTO for adding bundle to catalog
/// </summary>
public class AddBundleToCatalogDto
{
    public int BundleId { get; set; }
    public List<int> CategoryIds { get; set; } = new();
    public int SortOrder { get; set; }
}

/// <summary>
/// DTO for updating item categories
/// </summary>
public class UpdateItemCategoriesDto
{
    public List<int> CategoryIds { get; set; } = new();
}

/// <summary>
/// DTO for updating bundle categories
/// </summary>
public class UpdateBundleCategoriesDto
{
    public List<int> CategoryIds { get; set; } = new();
}

/// <summary>
/// DTO for creating a category
/// </summary>
public class CategoryCreateDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public int SortOrder { get; set; } = 0;
}

/// <summary>
/// DTO for updating a category
/// </summary>
public class CategoryUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
}
