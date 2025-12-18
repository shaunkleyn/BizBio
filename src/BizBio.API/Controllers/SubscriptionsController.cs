using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;

namespace BizBio.API.Controllers;

[Route("api/v1/subscriptions")]
[ApiController]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionTierRepository _tierRepo;

    public SubscriptionsController(ISubscriptionTierRepository tierRepo)
    {
        _tierRepo = tierRepo;
    }

    /// <summary>
    /// Get all active subscription tiers
    /// Publicly accessible endpoint for viewing pricing and tier information
    /// </summary>
    /// <param name="productLine">Optional filter by product line</param>
    [HttpGet("tiers")]
    public async Task<IActionResult> GetTiers([FromQuery] string? productLine = null)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var tiers = await _tierRepo.GetAllActiveAsync();

        if (!string.IsNullOrEmpty(productLine))
        {
            tiers = tiers.Where(t => t.ProductLine.ToString().Equals(productLine, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        // Map to anonymous objects to avoid circular reference
        var tierDtos = tiers.Select(t => new
        {
            t.Id,
            t.ProductLineId,
            t.TierName,
            t.TierCode,
            t.DisplayName,
            t.Description,
            t.MonthlyPrice,
            t.AnnualPrice,
            t.AnnualDiscountPercent,
            t.MaxProfiles,
            t.MaxCatalogItems,
            t.MaxLocations,
            t.MaxTeamMembers,
            t.MaxDocuments,
            t.MaxDocumentSizeMB,
            t.MaxImagesPerItem,
            t.CustomBranding,
            t.RemoveBranding,
            t.Analytics,
            t.AdvancedAnalytics,
            t.ApiAccess,
            t.WhiteLabel,
            t.CustomDomain,
            t.CustomSubdomain,
            t.PrioritySupport,
            t.PhoneSupport,
            t.DedicatedManager,
            t.ItemVariants,
            t.ItemAddons,
            t.AllergenInfo,
            t.DietaryTags,
            t.MenuScheduling,
            t.MultiLanguage,
            t.NutritionalInfo,
            t.Bundles,
            t.InventoryTracking,
            t.MultiLocationInventory,
            t.BulkOperations,
            t.BarcodeGeneration,
            t.B2BFeatures,
            t.OrganizationalHierarchy,
            t.TeamManagement,
            t.SSOIntegration,
            t.DisplayOrder,
            t.IsActive,
            t.IsPopular,
            t.RecommendedFor,
            t.CreatedAt,
            t.UpdatedAt,
            ProductLine = t.ProductLine != null ? new
            {
                t.ProductLine.Id,
                t.ProductLine.Name,
                t.ProductLine.Description
            } : null
        }).ToList();

        return Ok(new
        {
            success = true,
            data = new
            {
                tiers = tierDtos,
                count = tierDtos.Count
            }
        });
    }
}
