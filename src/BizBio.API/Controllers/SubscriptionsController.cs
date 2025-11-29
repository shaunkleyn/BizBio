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

        return Ok(new
        {
            success = true,
            data = new
            {
                tiers = tiers,
                count = tiers.Count()
            }
        });
    }
}
