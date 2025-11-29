namespace BizBio.Core.Interfaces;

public interface IPaymentService
{
    /// <summary>
    /// Generates a PayFast payment URL with the provided parameters
    /// </summary>
    Task<PaymentResult> GeneratePaymentUrlAsync(PaymentRequest request);

    /// <summary>
    /// Validates PayFast ITN (Instant Transaction Notification) callback
    /// </summary>
    Task<bool> ValidatePaymentNotificationAsync(Dictionary<string, string> postData);

    /// <summary>
    /// Processes a successful payment notification
    /// </summary>
    Task<bool> ProcessPaymentAsync(string paymentId, Dictionary<string, string> paymentData);
}

public class PaymentRequest
{
    public int UserId { get; set; }
    public int SubscriptionTierId { get; set; }
    public decimal Amount { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string ItemDescription { get; set; } = string.Empty;
    public string BillingCycle { get; set; } = "monthly"; // or "annual"
    public string ReturnUrl { get; set; } = string.Empty;
    public string CancelUrl { get; set; } = string.Empty;
    public string NotifyUrl { get; set; } = string.Empty;
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string? PaymentUrl { get; set; }
    public string? PaymentId { get; set; }
    public string? ErrorMessage { get; set; }

    public static PaymentResult SuccessResult(string paymentUrl, string paymentId)
    {
        return new PaymentResult
        {
            Success = true,
            PaymentUrl = paymentUrl,
            PaymentId = paymentId
        };
    }

    public static PaymentResult FailureResult(string errorMessage)
    {
        return new PaymentResult
        {
            Success = false,
            ErrorMessage = errorMessage
        };
    }
}
