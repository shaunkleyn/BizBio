using System.Net;
using System.Security.Cryptography;
using System.Text;
using BizBio.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BizBio.Infrastructure.Services;

public class PayFastService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly IUserSubscriptionRepository _subscriptionRepository;
    private readonly string _merchantId;
    private readonly string _merchantKey;
    private readonly string _passPhrase;
    private readonly string _payFastUrl;
    private readonly bool _isSandbox;

    public PayFastService(
        IConfiguration configuration,
        IUserSubscriptionRepository subscriptionRepository)
    {
        _configuration = configuration;
        _subscriptionRepository = subscriptionRepository;

        _merchantId = configuration["PayFast:MerchantId"] ?? "";
        _merchantKey = configuration["PayFast:MerchantKey"] ?? "";
        _passPhrase = configuration["PayFast:PassPhrase"] ?? "";
        _payFastUrl = configuration["PayFast:Url"] ?? "https://sandbox.payfast.co.za";
        _isSandbox = _payFastUrl.Contains("sandbox");
    }

    public async Task<PaymentResult> GeneratePaymentUrlAsync(PaymentRequest request)
    {
        try
        {
            // Generate unique payment identifier
            var paymentId = Guid.NewGuid().ToString();

            // Build PayFast data dictionary
            var payFastData = new Dictionary<string, string>
            {
                { "merchant_id", _merchantId },
                { "merchant_key", _merchantKey },
                { "return_url", request.ReturnUrl },
                { "cancel_url", request.CancelUrl },
                { "notify_url", request.NotifyUrl },
                { "name_first", "BizBio" },
                { "name_last", "User" },
                { "email_address", "user@bizbio.co.za" },
                { "m_payment_id", paymentId },
                { "amount", request.Amount.ToString("F2") },
                { "item_name", request.ItemName },
                { "item_description", request.ItemDescription },
                { "custom_int1", request.UserId.ToString() },
                { "custom_int2", request.SubscriptionTierId.ToString() },
                { "custom_str1", request.BillingCycle }
            };

            // Add subscription-specific fields if applicable
            if (request.BillingCycle == "monthly")
            {
                payFastData.Add("subscription_type", "1"); // Regular subscription
                payFastData.Add("billing_date", DateTime.Now.ToString("yyyy-MM-dd"));
                payFastData.Add("recurring_amount", request.Amount.ToString("F2"));
                payFastData.Add("frequency", "3"); // Monthly
                payFastData.Add("cycles", "0"); // Indefinite
            }
            else if (request.BillingCycle == "annual")
            {
                payFastData.Add("subscription_type", "1");
                payFastData.Add("billing_date", DateTime.Now.ToString("yyyy-MM-dd"));
                payFastData.Add("recurring_amount", request.Amount.ToString("F2"));
                payFastData.Add("frequency", "6"); // Annual
                payFastData.Add("cycles", "0"); // Indefinite
            }

            // Generate signature
            var signature = GenerateSignature(payFastData);
            payFastData.Add("signature", signature);

            // Build query string
            var queryString = string.Join("&", payFastData.Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

            var paymentUrl = $"{_payFastUrl}/eng/process?{queryString}";

            return await Task.FromResult(PaymentResult.SuccessResult(paymentUrl, paymentId));
        }
        catch (Exception ex)
        {
            return PaymentResult.FailureResult($"Failed to generate payment URL: {ex.Message}");
        }
    }

    public async Task<bool> ValidatePaymentNotificationAsync(Dictionary<string, string> postData)
    {
        try
        {
            // 1. Verify the signature
            if (!postData.ContainsKey("signature"))
                return false;

            var receivedSignature = postData["signature"];
            var dataToVerify = new Dictionary<string, string>(postData);
            dataToVerify.Remove("signature");

            var calculatedSignature = GenerateSignature(dataToVerify);

            if (receivedSignature != calculatedSignature)
            {
                Console.WriteLine("PayFast signature validation failed");
                return false;
            }

            // 2. Verify payment status
            if (!postData.ContainsKey("payment_status"))
                return false;

            var paymentStatus = postData["payment_status"];
            if (paymentStatus != "COMPLETE")
            {
                Console.WriteLine($"PayFast payment not complete. Status: {paymentStatus}");
                return false;
            }

            // 3. Verify payment amounts match (if stored in database)
            if (postData.ContainsKey("amount_gross"))
            {
                // Additional validation can be added here
                Console.WriteLine($"Payment validated successfully. Amount: {postData["amount_gross"]}");
            }

            return await Task.FromResult(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"PayFast validation error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ProcessPaymentAsync(string paymentId, Dictionary<string, string> paymentData)
    {
        try
        {
            // Extract custom fields
            if (!paymentData.ContainsKey("custom_int1") ||
                !paymentData.ContainsKey("custom_int2") ||
                !paymentData.ContainsKey("custom_str1"))
            {
                Console.WriteLine("Missing custom payment data");
                return false;
            }

            var userId = int.Parse(paymentData["custom_int1"]);
            var subscriptionTierId = int.Parse(paymentData["custom_int2"]);
            var billingCycle = paymentData["custom_str1"];

            // Check if subscription already exists and is active
            var existingSubscriptions = await _subscriptionRepository.GetByUserIdAsync(userId);
            var activeSubscription = existingSubscriptions
                .FirstOrDefault(s => s.StatusId == (int)Core.Enums.SubscriptionStatus.Active);

            if (activeSubscription != null)
            {
                // Cancel old subscription
                activeSubscription.StatusId = (int)Core.Enums.SubscriptionStatus.Cancelled;
                activeSubscription.CancelledAt = DateTime.UtcNow;
                activeSubscription.UpdatedAt = DateTime.UtcNow;
                await _subscriptionRepository.UpdateAsync(activeSubscription);
            }

            // Determine price from payment data
            decimal price = 0;
            if (paymentData.ContainsKey("amount_gross"))
            {
                decimal.TryParse(paymentData["amount_gross"], out price);
            }

            // Create or update subscription
            var subscription = new Core.Entities.UserSubscription
            {
                UserId = userId,
                TierId = subscriptionTierId,
                StatusId = (int)Core.Enums.SubscriptionStatus.Active,
                BillingCycleId = billingCycle == "monthly"
                    ? (int)Core.Enums.BillingCycle.Monthly
                    : (int)Core.Enums.BillingCycle.Annual,
                StartDate = DateTime.UtcNow,
                NextBillingDate = billingCycle == "monthly"
                    ? DateTime.UtcNow.AddMonths(1)
                    : DateTime.UtcNow.AddYears(1),
                Price = price,
                Currency = "ZAR",
                PaymentMethod = "PayFast",
                LastPaymentDate = DateTime.UtcNow,
                LastPaymentAmount = price,
                AutoRenew = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _subscriptionRepository.AddAsync(subscription);
            await _subscriptionRepository.SaveChangesAsync();

            Console.WriteLine($"Subscription created successfully for user {userId}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error processing payment: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Generates MD5 signature for PayFast integration
    /// </summary>
    private string GenerateSignature(Dictionary<string, string> data)
    {
        // Sort the data alphabetically by key
        var sortedData = data.OrderBy(x => x.Key);

        // Create parameter string
        var paramString = string.Join("&", sortedData
            .Where(kvp => !string.IsNullOrEmpty(kvp.Value))
            .Select(kvp => $"{kvp.Key}={WebUtility.UrlEncode(kvp.Value)}"));

        // Append passphrase if not in sandbox mode or if configured
        if (!string.IsNullOrEmpty(_passPhrase))
        {
            paramString += $"&passphrase={WebUtility.UrlEncode(_passPhrase)}";
        }

        // Calculate MD5 hash
        using (var md5 = MD5.Create())
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(paramString));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }
    }
}
