using BizBio.Core.DTOs;
using BizBio.Core.Entities;

namespace BizBio.Core.Interfaces;

/// <summary>
/// Service interface for authentication operations including registration, login,
/// email verification, and password reset functionality.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Registers a new user with the provided registration details.
    /// </summary>
    Task<AuthResult> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// Authenticates a user with email and password credentials.
    /// </summary>
    Task<AuthResult> LoginAsync(LoginDto dto);

    /// <summary>
    /// Verifies a user's email address using a verification token.
    /// </summary>
    Task<AuthResult> VerifyEmailAsync(string token);

    /// <summary>
    /// Resends the email verification link to the specified email address.
    /// </summary>
    Task<AuthResult> ResendVerificationEmailAsync(string email);

    /// <summary>
    /// Initiates a password reset process for the specified email address.
    /// </summary>
    Task<AuthResult> RequestPasswordResetAsync(string email);

    /// <summary>
    /// Resets a user's password using a reset token and new password.
    /// </summary>
    Task<AuthResult> ResetPasswordAsync(string token, string newPassword);
}

/// <summary>
/// Represents the result of an authentication operation.
/// </summary>
public class AuthResult
{
    /// <summary>
    /// Gets or sets a value indicating whether the operation was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Gets or sets an optional message providing details about the result.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Gets or sets the authenticated user object, if applicable.
    /// </summary>
    public User? User { get; set; }

    /// <summary>
    /// Gets or sets the JWT authentication token, if applicable.
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Creates a successful authentication result with optional token.
    /// </summary>
    /// <param name="user">The authenticated user</param>
    /// <param name="token">The JWT token (optional)</param>
    /// <returns>A successful AuthResult</returns>
    public static AuthResult SuccessResult(User user, string? token = null)
    {
        return new AuthResult
        {
            Success = true,
            User = user,
            Token = token,
            Message = "Operation completed successfully"
        };
    }

    /// <summary>
    /// Creates a failed authentication result with an error message.
    /// </summary>
    /// <param name="message">The error message</param>
    /// <returns>A failed AuthResult</returns>
    public static AuthResult FailureResult(string message)
    {
        return new AuthResult
        {
            Success = false,
            Message = message
        };
    }
}
