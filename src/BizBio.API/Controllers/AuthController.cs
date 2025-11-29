using Microsoft.AspNetCore.Mvc;
using BizBio.Core.Interfaces;
using BizBio.Core.DTOs;

namespace BizBio.API.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;
    private readonly ITelemetryService _telemetry;

    public AuthController(
        IAuthService authService,
        ILogger<AuthController> logger,
        ITelemetryService telemetry)
    {
        _authService = authService;
        _logger = logger;
        _telemetry = telemetry;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        _logger.LogInformation("Registration attempt for email: {Email}", dto.Email);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Registration failed - Invalid model state for email: {Email}", dto.Email);
            return BadRequest(ModelState);
        }

        var result = await _authService.RegisterAsync(dto);

        if (!result.Success)
        {
            _logger.LogWarning("Registration failed for email: {Email}. Reason: {Reason}", dto.Email, result.Message);
            _telemetry.TrackEvent("UserRegistrationFailed", new Dictionary<string, string>
            {
                { "Email", dto.Email },
                { "Reason", result.Message ?? "Unknown" }
            });
            return BadRequest(new { message = result.Message });
        }

        _logger.LogInformation("User registered successfully: {Email}", dto.Email);
        _telemetry.TrackEvent("UserRegistered", new Dictionary<string, string>
        {
            { "Email", dto.Email },
            { "FirstName", dto.FirstName },
            { "LastName", dto.LastName }
        });
        _telemetry.TrackBusinessMetric("NewUserRegistrations", 1);

        return Ok(new
        {
            message = "Registration successful! Please check your email to verify your account.",
            email = dto.Email
        });
    }

    /// <summary>
    /// Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        _logger.LogInformation("Login attempt for email: {Email}", dto.Email);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Login failed - Invalid model state for email: {Email}", dto.Email);
            return BadRequest(ModelState);
        }

        var result = await _authService.LoginAsync(dto);

        if (!result.Success)
        {
            _logger.LogWarning("Login failed for email: {Email}. Reason: {Reason}", dto.Email, result.Message);
            _telemetry.TrackEvent("UserLoginFailed", new Dictionary<string, string>
            {
                { "Email", dto.Email },
                { "Reason", result.Message ?? "Unknown" }
            });
            return Unauthorized(new { message = result.Message });
        }

        _logger.LogInformation("User logged in successfully: {Email}", dto.Email);
        _telemetry.TrackEvent("UserLoggedIn", new Dictionary<string, string>
        {
            { "UserId", result.User!.Id.ToString() },
            { "Email", dto.Email }
        });
        _telemetry.TrackUserAction(result.User!.Id.ToString(), "Login", new Dictionary<string, string>
        {
            { "Email", dto.Email }
        });

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

    /// <summary>
    /// Verify email address
    /// </summary>
    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        if (string.IsNullOrEmpty(token))
            return BadRequest(new { message = "Token is required" });

        var success = await _authService.VerifyEmailAsync(token);

        if (!success.Success)
            return BadRequest(new { message = "Invalid or expired token" });

        return Ok(new { message = "Email verified successfully! You can now login." });
    }

    /// <summary>
    /// Resend email verification
    /// </summary>
    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationDto dto)
    {
        if (string.IsNullOrEmpty(dto.Email))
            return BadRequest(new { message = "Email is required" });

        var result = await _authService.ResendVerificationEmailAsync(dto.Email);

        if (!result.Success)
            return BadRequest(new { message = result.Message });

        return Ok(new { message = "Verification email has been sent. Please check your inbox." });
    }

    /// <summary>
    /// Request password reset
    /// </summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        await _authService.RequestPasswordResetAsync(dto.Email);

        return Ok(new
        {
            message = "If that email exists, we've sent password reset instructions."
        });
    }

    /// <summary>
    /// Reset password
    /// </summary>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var success = await _authService.ResetPasswordAsync(dto.Token, dto.NewPassword);

        if (!success.Success)
            return BadRequest(new { message = "Invalid or expired token" });

        return Ok(new { message = "Password reset successfully! You can now login." });
    }
}
