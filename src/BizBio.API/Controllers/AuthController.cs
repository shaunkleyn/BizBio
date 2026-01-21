using Microsoft.AspNetCore.Mvc;
using Microsoft.ApplicationInsights;
using BizBio.Core.Interfaces;
using BizBio.Core.DTOs;

namespace BizBio.API.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly TelemetryClient _telemetryClient;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger,
        TelemetryClient telemetryClient)
    {
        _authService = authService;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Registration failed: Invalid model state for {Email}", dto.Email);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Registration attempt for {Email}", dto.Email);

            var result = await _authService.RegisterAsync(dto);

            if (!result.Success)
            {
                _logger.LogWarning("Registration failed for {Email}: {Message}", dto.Email, result.Message);
                return BadRequest(new { message = result.Message });
            }

            _logger.LogInformation("Registration successful for {Email}", dto.Email);

            return Ok(new
            {
                message = "Registration successful! Please check your email to verify your account.",
                email = dto.Email
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during registration for {Email}", dto.Email);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "AuthController" },
                { "Action", "Register" },
                { "Email", dto.Email }
            });
            return StatusCode(500, new { message = "An error occurred during registration. Please try again later." });
        }
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Login failed: Invalid model state for {Email}", dto.Email);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Login attempt for {Email}", dto.Email);

            var result = await _authService.LoginAsync(dto);

            if (!result.Success)
            {
                _logger.LogWarning("Login failed for {Email}: {Message}", dto.Email, result.Message);
                _telemetryClient.TrackEvent("LoginFailed", new Dictionary<string, string>
                {
                    { "Email", dto.Email },
                    { "Reason", result.Message ?? "Unknown" }
                });
                return Unauthorized(new { message = result.Message });
            }

            _logger.LogInformation("Login successful for {Email}", dto.Email);

            return Ok(new
            {
                token = result.Token,
                user = new
                {
                    id = result.User!.Id,
                    email = result.User.Email,
                    firstName = result.User.FirstName,
                    lastName = result.User.LastName
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for {Email}", dto.Email);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "AuthController" },
                { "Action", "Login" },
                { "Email", dto.Email }
            });
            return StatusCode(500, new { message = "An error occurred during login. Please try again later." });
        }
    }

    /// <summary>
    /// Verify email address using token
    /// </summary>
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("Email verification failed: Token is required");
                return BadRequest(new { message = "Token is required" });
            }

            _logger.LogInformation("Email verification attempt with token");

            var success = await _authService.VerifyEmailAsync(token);

            if (!success.Success)
            {
                _logger.LogWarning("Email verification failed: Invalid or expired token");
                return BadRequest(new { message = success.Message });
            }

            _logger.LogInformation("Email verified successfully");

            return Ok(new { message = "Email verified successfully! You can now login." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during email verification");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "AuthController" },
                { "Action", "VerifyEmail" }
            });
            return StatusCode(500, new { message = "An error occurred during email verification. Please try again later." });
        }
    }

    /// <summary>
    /// Verify email address using 6-digit code
    /// </summary>
    [HttpPost("verify-email-code")]
    public async Task<IActionResult> VerifyEmailWithCode([FromBody] VerifyEmailCodeDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Email verification with code failed: Invalid model state for {Email}", dto.Email);
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Email verification attempt with code for {Email}", dto.Email);

            var result = await _authService.VerifyEmailWithCodeAsync(dto.Email, dto.Code);

            if (!result.Success)
            {
                _logger.LogWarning("Email verification with code failed for {Email}: {Message}", dto.Email, result.Message);
                return BadRequest(new { message = result.Message });
            }

            _logger.LogInformation("Email verified successfully with code for {Email}", dto.Email);

            return Ok(new { message = "Email verified successfully! You can now login." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during email verification with code for {Email}", dto.Email);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "AuthController" },
                { "Action", "VerifyEmailWithCode" },
                { "Email", dto.Email }
            });
            return StatusCode(500, new { message = "An error occurred during email verification. Please try again later." });
        }
    }

    /// <summary>
    /// Development-only: Verify email by email address (bypasses token requirement)
    /// </summary>
    [HttpPost("dev-verify-email")]
    public async Task<IActionResult> DevVerifyEmail([FromBody] DevVerifyEmailDto dto)
    {
        // Only allow in Development environment
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment != "Development")
        {
            return NotFound();
        }

        try
        {
            if (string.IsNullOrEmpty(dto.Email))
            {
                return BadRequest(new { message = "Email is required" });
            }

            _logger.LogWarning("DEV ONLY: Manual email verification for {Email}", dto.Email);

            // Get user by email and manually verify
            var user = await _authService.GetUserByEmailAsync(dto.Email);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            if (user.EmailVerified)
            {
                return Ok(new { message = "Email already verified" });
            }

            await _authService.ManuallyVerifyEmailAsync(dto.Email);

            _logger.LogInformation("DEV ONLY: Email verified successfully for {Email}", dto.Email);

            return Ok(new { message = "Email verified successfully! (Development bypass)" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DEV ONLY: Error during manual email verification for {Email}", dto.Email);
            return StatusCode(500, new { message = ex.Message });
        }
    }

    /// <summary>
    /// Resend email verification
    /// </summary>
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto)
    {
        try
        {
            if (string.IsNullOrEmpty(dto.Email))
            {
                _logger.LogWarning("Resend verification failed: Email is required");
                return BadRequest(new { message = "Email is required" });
            }

            _logger.LogInformation("Resend verification request for {Email}", dto.Email);

            var result = await _authService.ResendVerificationEmailAsync(dto.Email);

            if (!result.Success)
            {
                _logger.LogWarning("Resend verification failed for {Email}: {Message}", dto.Email, result.Message);
                return BadRequest(new { message = result.Message });
            }

            _logger.LogInformation("Verification email resent to {Email}", dto.Email);

            return Ok(new { message = "Verification email has been sent. Please check your inbox." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error resending verification for {Email}", dto.Email);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "AuthController" },
                { "Action", "ResendVerification" },
                { "Email", dto.Email }
            });
            return StatusCode(500, new { message = "An error occurred. Please try again later." });
        }
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        try
        {
            _logger.LogInformation("Password reset request for {Email}", dto.Email);

            await _authService.RequestPasswordResetAsync(dto.Email);

            return Ok(new
            {
                message = "If that email exists, we've sent password reset instructions."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during password reset request for {Email}", dto.Email);
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "AuthController" },
                { "Action", "ForgotPassword" },
                { "Email", dto.Email }
            });
            return StatusCode(500, new { message = "An error occurred. Please try again later." });
        }
    }

    /// <summary>
    /// Reset password
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Password reset failed: Invalid model state");
                return BadRequest(ModelState);
            }

            _logger.LogInformation("Password reset attempt");

            var success = await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);

            if (!success.Success)
            {
                _logger.LogWarning("Password reset failed: Invalid or expired token");
                return BadRequest(new { message = "Invalid or expired token" });
            }

            _logger.LogInformation("Password reset successfully");
            _telemetryClient.TrackEvent("PasswordReset");

            return Ok(new { message = "Password reset successfully! You can now login." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during password reset");
            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Controller", "AuthController" },
                { "Action", "ResetPassword" }
            });
            return StatusCode(500, new { message = "An error occurred. Please try again later." });
        }
    }
}
