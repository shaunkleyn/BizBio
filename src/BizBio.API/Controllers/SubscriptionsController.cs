using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;

namespace BizBio.API.Controllers;

[Route("api/v1/subscriptions")]
[ApiController]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionTierRepository _tierRepo;
    private readonly ILogger<SubscriptionsController> _logger;
    private readonly ITelemetryService _telemetry;

    public SubscriptionsController(
        ISubscriptionTierRepository tierRepo,
        ILogger<SubscriptionsController> logger,
        ITelemetryService telemetry)
    {
        _tierRepo = tierRepo;
        _logger = logger;
        _telemetry = telemetry;
    }

    /// <summary>
    /// Get all active subscription tiers
    /// Publicly accessible endpoint for viewing pricing and tier information
    /// </summary>
    /// <param name="productLine">Optional filter by product line</param>
    [HttpGet("tiers")]
    public async Task<IActionResult> GetTiers([FromQuery] string? productLine = null)
    {
        _logger.LogInformation("Fetching subscription tiers. ProductLine filter: {ProductLine}", productLine ?? "None");

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("GetTiers failed - Invalid model state");
            return BadRequest(ModelState);
        }

        var tiers = await _tierRepo.GetAllActiveAsync();

        if (!string.IsNullOrEmpty(productLine))
        {
            tiers = tiers.Where(t => t.ProductLine.Name.ToString().Equals(productLine, StringComparison.OrdinalIgnoreCase)).ToList();
            _logger.LogInformation("Filtered tiers by product line: {ProductLine}. Count: {Count}", productLine, tiers.Count());
        }

        _logger.LogInformation("Successfully retrieved {Count} subscription tiers", tiers.Count());
        _telemetry.TrackEvent("SubscriptionTiersViewed", new Dictionary<string, string>
        {
            { "ProductLineFilter", productLine ?? "None" },
            { "TiersCount", tiers.Count().ToString() }
        });
        _telemetry.TrackMetric("SubscriptionTiersRetrieved", tiers.Count());

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
