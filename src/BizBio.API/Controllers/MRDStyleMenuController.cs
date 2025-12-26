namespace BizBio.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BizBio.Core.DTOs;
using BizBio.Core.Services;
using BizBio.Infrastructure.Data;

/// <summary>
/// Example controller showing MRD-style menu API with efficient array indexing
/// This complements the existing MenuController with an alternative response format
/// </summary>
[ApiController]
[Route("api/v2/menu")]
public class MRDStyleMenuController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly MenuMappingService _menuMapper;

    public MRDStyleMenuController(ApplicationDbContext context, MenuMappingService menuMapper)
    {
        _context = context;
        _menuMapper = menuMapper;
    }

    /// <summary>
    /// Get menu in MRD-style format with efficient array indexing
    /// Example: GET /api/v2/menu/123
    ///
    /// Returns menu with:
    /// - options[] and extras[] arrays defined once at top level
    /// - variants reference them via option_indices and extra_indices
    /// - Single query, minimal data transfer, simple frontend logic
    /// </summary>
    [HttpGet("{catalogId}")]
    public async Task<ActionResult<MenuResponseDto>> GetMenu(int catalogId)
    {
        var catalog = await _context.Catalogs
            .Include(c => c.Categories)
                .ThenInclude(cat => cat.CatalogItemCategories)
                    .ThenInclude(cic => cic.CatalogItem)
                        .ThenInclude(item => item.Variants)
            .Include(c => c.Categories)
                .ThenInclude(cat => cat.CatalogItemCategories)
                    .ThenInclude(cic => cic.CatalogItem)
                        .ThenInclude(item => item.ExtraGroupLinks)
                            .ThenInclude(link => link.ExtraGroup)
                                .ThenInclude(group => group.GroupItems)
                                    .ThenInclude(gi => gi.Extra)
            .Include(c => c.Categories)
                .ThenInclude(cat => cat.CatalogItemCategories)
                    .ThenInclude(cic => cic.CatalogItem)
                        .ThenInclude(item => item.OptionGroupLinks)
                            .ThenInclude(link => link.OptionGroup)
                                .ThenInclude(group => group.GroupItems)
                                    .ThenInclude(gi => gi.Option)
            .FirstOrDefaultAsync(c => c.Id == catalogId);

        if (catalog == null)
            return NotFound(new { error = "Menu not found" });

        var menuDto = _menuMapper.MapCatalogToMenu(catalog);

        return Ok(menuDto);
    }

    /// <summary>
    /// Get menu by profile slug (public-facing URL)
    /// Example: GET /api/v2/menu/by-slug/my-restaurant
    /// </summary>
    [HttpGet("by-slug/{slug}")]
    public async Task<ActionResult<MenuResponseDto>> GetMenuBySlug(string slug)
    {
        var profile = await _context.Profiles
            .FirstOrDefaultAsync(p => p.Slug == slug && p.IsActive);

        if (profile == null)
            return NotFound(new { error = "Profile not found" });

        var catalog = await _context.Catalogs
            .Include(c => c.Categories)
                .ThenInclude(cat => cat.CatalogItemCategories)
                    .ThenInclude(cic => cic.CatalogItem)
                        .ThenInclude(item => item.Variants)
            .Include(c => c.Categories)
                .ThenInclude(cat => cat.CatalogItemCategories)
                    .ThenInclude(cic => cic.CatalogItem)
                        .ThenInclude(item => item.ExtraGroupLinks)
                            .ThenInclude(link => link.ExtraGroup)
                                .ThenInclude(group => group.GroupItems)
                                    .ThenInclude(gi => gi.Extra)
            .Include(c => c.Categories)
                .ThenInclude(cat => cat.CatalogItemCategories)
                    .ThenInclude(cic => cic.CatalogItem)
                        .ThenInclude(item => item.OptionGroupLinks)
                            .ThenInclude(link => link.OptionGroup)
                                .ThenInclude(group => group.GroupItems)
                                    .ThenInclude(gi => gi.Option)
            .Where(c => c.ProfileId == profile.Id && c.IsPublic)
            .OrderByDescending(c => c.CreatedAt)
            .FirstOrDefaultAsync();

        if (catalog == null)
            return NotFound(new { error = "No active menu found for this profile" });

        var menuDto = _menuMapper.MapCatalogToMenu(catalog);

        return Ok(menuDto);
    }

    /// <summary>
    /// Calculate cart total from selected items (validates selections against menu)
    /// Example POST body:
    /// {
    ///   "items": [
    ///     {
    ///       "variantId": 123,
    ///       "quantity": 2,
    ///       "selectedExtras": [
    ///         { "extraId": 456, "quantity": 1 }
    ///       ],
    ///       "selectedOptions": [
    ///         { "optionId": 789 }
    ///       ]
    ///     }
    ///   ],
    ///   "deliveryFee": 20
    /// }
    /// </summary>
    [HttpPost("calculate-cart")]
    public async Task<ActionResult<CartCalculationResponse>> CalculateCart(
        [FromBody] CartCalculationRequest request)
    {
        var response = new CartCalculationResponse { Items = new List<CartItemCalculation>() };
        decimal subtotal = 0;

        foreach (var cartItem in request.Items)
        {
            var variant = await _context.CatalogItemVariants
                .Include(v => v.CatalogItem)
                    .ThenInclude(i => i.ExtraGroupLinks)
                        .ThenInclude(l => l.ExtraGroup)
                            .ThenInclude(g => g.GroupItems)
                                .ThenInclude(gi => gi.Extra)
                .FirstOrDefaultAsync(v => v.Id == cartItem.VariantId);

            if (variant == null)
            {
                return BadRequest(new { error = $"Variant {cartItem.VariantId} not found" });
            }

            decimal itemTotal = variant.Price * cartItem.Quantity;
            var selectedExtras = new List<SelectedExtraDetail>();

            // Validate and calculate options cost
            foreach (var optionSelection in cartItem.SelectedOptions)
            {
                // Try dedicated options table first
                var option = await _context.CatalogItemOptions
                    .FirstOrDefaultAsync(o => o.Id == optionSelection.OptionId);

                if (option != null)
                {
                    // Options use PriceModifier (can be positive, negative, or zero)
                    itemTotal += option.PriceModifier * cartItem.Quantity;
                }
                else
                {
                    // Legacy: Check extras table (for old option groups with MinRequired >= 1)
                    var legacyOption = await _context.CatalogItemExtras
                        .FirstOrDefaultAsync(e => e.Id == optionSelection.OptionId);

                    if (legacyOption == null)
                        return BadRequest(new { error = $"Option {optionSelection.OptionId} not found" });

                    itemTotal += legacyOption.BasePrice * cartItem.Quantity;
                }
            }

            // Calculate extras cost
            foreach (var extraSelection in cartItem.SelectedExtras)
            {
                var extra = await _context.CatalogItemExtras
                    .FirstOrDefaultAsync(e => e.Id == extraSelection.ExtraId);

                if (extra == null)
                    continue;

                decimal extraPrice = extra.BasePrice * extraSelection.Quantity;
                itemTotal += extraPrice * cartItem.Quantity;

                selectedExtras.Add(new SelectedExtraDetail
                {
                    ExtraId = extra.Id,
                    Name = extra.Name,
                    Quantity = extraSelection.Quantity,
                    UnitPrice = extra.BasePrice,
                    TotalPrice = extraPrice
                });
            }

            subtotal += itemTotal;

            response.Items.Add(new CartItemCalculation
            {
                VariantId = variant.Id,
                ItemName = variant.CatalogItem.Name,
                VariantName = variant.Title,
                Quantity = cartItem.Quantity,
                BasePrice = variant.Price,
                SelectedExtras = selectedExtras,
                ItemTotal = itemTotal
            });
        }

        response.Subtotal = subtotal;
        response.DeliveryFee = request.DeliveryFee ?? 0;
        response.Total = subtotal + response.DeliveryFee;

        return Ok(response);
    }
}

// DTOs for cart calculation

public class CartCalculationRequest
{
    public List<CartItemRequest> Items { get; set; } = new();
    public decimal? DeliveryFee { get; set; }
}

public class CartItemRequest
{
    public int VariantId { get; set; }
    public int Quantity { get; set; }
    public List<SelectedExtra> SelectedExtras { get; set; } = new();
    public List<SelectedOption> SelectedOptions { get; set; } = new();
}

public class SelectedExtra
{
    public int ExtraId { get; set; }
    public int Quantity { get; set; } = 1;
}

public class SelectedOption
{
    public int OptionId { get; set; }
}

public class CartCalculationResponse
{
    public List<CartItemCalculation> Items { get; set; } = new();
    public decimal Subtotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal Total { get; set; }
}

public class CartItemCalculation
{
    public int VariantId { get; set; }
    public string ItemName { get; set; } = null!;
    public string VariantName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal BasePrice { get; set; }
    public List<SelectedExtraDetail> SelectedExtras { get; set; } = new();
    public decimal ItemTotal { get; set; }
}

public class SelectedExtraDetail
{
    public int ExtraId { get; set; }
    public string Name { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
