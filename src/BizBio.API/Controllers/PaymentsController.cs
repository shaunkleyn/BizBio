using BizBio.Core.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BizBio.API.Controllers;

/// <summary>
/// Handles payment processing and subscription purchases via PayFast
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly ISubscriptionTierRepository _tierRepository;
    private readonly ILogger<PaymentsController> _logger;
    private readonly TelemetryClient _telemetryClient;

    public PaymentsController(
        IPaymentService paymentService,
        ISubscriptionTierRepository tierRepository,
        ILogger<PaymentsController> logger,
        TelemetryClient telemetryClient)
    {
        _paymentService = paymentService;
        _tierRepository = tierRepository;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    /// <summary>
    /// Initiates a subscription purchase
    /// </summary>
    [HttpPost("subscribe")]
    [Authorize]
    public async Task<IActionResult> InitiateSubscription([FromBody] SubscribeRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                _logger.LogWarning("Subscription initiation failed: User not authenticated");
                return Unauthorized(new { message = "User not authenticated" });
            }

            _logger.LogInformation("User {UserId} initiating subscription for tier {TierId}", userId, request.SubscriptionTierId);

            // Get subscription tier details
            var tier = await _tierRepository.GetByIdAsync(request.SubscriptionTierId);
            if (tier == null)
            {
                _logger.LogWarning("Subscription tier {TierId} not found for user {UserId}", request.SubscriptionTierId, userId);
                return NotFound(new { message = "Subscription tier not found" });
            }

            if (!tier.IsActive)
            {
                _logger.LogWarning("Subscription tier {TierId} is not active for user {UserId}", tier.Id, userId);
                return BadRequest(new { message = "This subscription tier is not available" });
            }

            // Calculate amount based on billing cycle
            var amount = request.BillingCycle == "annual" ? tier.AnnualPrice : tier.MonthlyPrice;
            if (amount == 0)
                return BadRequest(new { message = "This subscription tier is free. No payment required." });

            // Create payment request
            var paymentRequest = new PaymentRequest
            {
                UserId = userId,
                SubscriptionTierId = tier.Id,
                Amount = amount,
                ItemName = $"{tier.DisplayName} - {tier.ProductLine}",
                ItemDescription = $"{tier.Description} - {request.BillingCycle} subscription",
                BillingCycle = request.BillingCycle,
                ReturnUrl = $"{Request.Scheme}://{Request.Host}/payment/success",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/payment/cancel",
                NotifyUrl = $"{Request.Scheme}://{Request.Host}/api/payments/notify"
            };

            var result = await _paymentService.GeneratePaymentUrlAsync(paymentRequest);

            if (!result.Success)
            {
                _logger.LogError("Failed to generate payment URL for user {UserId}: {Error}", userId, result.ErrorMessage);
                return BadRequest(new { message = result.ErrorMessage });
            }

            _logger.LogInformation("Payment URL generated successfully for user {UserId}, PaymentId: {PaymentId}", userId, result.PaymentId);

            return Ok(new
            {
                paymentUrl = result.PaymentUrl,
                paymentId = result.PaymentId,
                amount = amount,
                currency = "ZAR"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error initiating payment for user {UserId}", int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0"));
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "PaymentsController" },
                { "Action", "InitiateSubscription" },
                { "SubscriptionTierId", request.SubscriptionTierId.ToString() }
            });
            return StatusCode(500, new { message = $"Error initiating payment: {ex.Message}" });
        }
    }

    /// <summary>
    /// PayFast ITN (Instant Transaction Notification) webhook endpoint
    /// This endpoint receives payment notifications from PayFast
    /// </summary>
    [HttpPost("notify")]
    [AllowAnonymous]
    public async Task<IActionResult> PayFastNotification()
    {
        try
        {
            // Read the POST data from PayFast
            var postData = new Dictionary<string, string>();
            foreach (var key in Request.Form.Keys)
            {
                postData[key] = Request.Form[key].ToString();
            }

            if (postData.Count == 0)
            {
                _logger.LogWarning("No POST data received from PayFast");
                return BadRequest("No data received");
            }

            _logger.LogInformation("PayFast notification received with {Count} fields", postData.Count);

            // Validate the notification
            var isValid = await _paymentService.ValidatePaymentNotificationAsync(postData);
            if (!isValid)
            {
                _logger.LogWarning("PayFast notification validation failed");
                _telemetryClient.TrackEvent("PayFastValidationFailed");
                return BadRequest("Invalid notification");
            }

            // Process the payment
            var paymentId = postData.ContainsKey("m_payment_id")
                ? postData["m_payment_id"]
                : Guid.NewGuid().ToString();

            var processed = await _paymentService.ProcessPaymentAsync(paymentId, postData);

            if (!processed)
            {
                _logger.LogError("Failed to process payment {PaymentId}", paymentId);
                _telemetryClient.TrackEvent("PaymentProcessingFailed", new Dictionary<string, string>
                {
                    { "PaymentId", paymentId }
                });
                return StatusCode(500, "Failed to process payment");
            }

            _logger.LogInformation("Payment {PaymentId} processed successfully", paymentId);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PayFast notification");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "PaymentsController" },
                { "Action", "PayFastNotification" }
            });
            return StatusCode(500, $"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Cancels an active subscription
    /// </summary>
    [HttpPost("cancel")]
    [Authorize]
    public async Task<IActionResult> CancelSubscription()
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                _logger.LogWarning("Subscription cancellation failed: User not authenticated");
                return Unauthorized(new { message = "User not authenticated" });
            }

            _logger.LogInformation("User {UserId} requested subscription cancellation", userId);

            // Implementation for subscription cancellation
            // This would typically involve:
            // 1. Finding the user's active subscription
            // 2. Marking it as canceled
            // 3. Optionally canceling the subscription with PayFast

            _telemetryClient.TrackEvent("SubscriptionCancellationRequested", new Dictionary<string, string>
            {
                { "UserId", userId.ToString() }
            });

            return Ok(new { message = "Subscription cancellation requested. It will remain active until the end of the current billing period." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error canceling subscription");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "PaymentsController" },
                { "Action", "CancelSubscription" }
            });
            return StatusCode(500, new { message = $"Error canceling subscription: {ex.Message}" });
        }
    }
}

public class SubscribeRequest
{
    public int SubscriptionTierId { get; set; }
    public string BillingCycle { get; set; } = "monthly"; // "monthly" or "annual"
}
