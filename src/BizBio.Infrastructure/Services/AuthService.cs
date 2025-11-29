using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;
using BizBio.Core.DTOs;
using BizBio.Core.Entities;
using BizBio.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BizBio.Infrastructure.Services;

/// <summary>
/// Implementation of the authentication service handling user registration, login,
/// email verification, and password reset operations.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of the AuthService class.
    /// </summary>
    /// <param name="userRepository">Repository for user data access</param>
    /// <param name="configuration">Application configuration</param>
    /// <param name="emailService">Email service for sending notifications</param>
    public AuthService(IUserRepository userRepository, IConfiguration configuration, IEmailService emailService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    /// <summary>
    /// Registers a new user with validation and email verification token generation.
    /// </summary>
    /// <param name="dto">Registration details</param>
    /// <returns>AuthResult with success status and user information</returns>
    public async Task<AuthResult> RegisterAsync(RegisterDto dto)
    {
        if (dto == null)
            return AuthResult.FailureResult("Registration data cannot be null");

        // Check if email already exists
        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null)
            return AuthResult.FailureResult("Email is already registered");

        try
        {
            // Hash the password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Generate email verification token
            var verificationToken = GenerateSecureToken();
            var verificationTokenExpiry = DateTime.UtcNow.AddHours(24);

            // Create new user
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                EmailVerified = false,
                IsActive = true,
                EmailVerificationToken = verificationToken,
                EmailVerificationTokenExpiry = verificationTokenExpiry,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                FailedLoginAttempts = 0
            };

            // Save user to repository
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Send verification email
            Console.WriteLine("[AuthService] About to send verification email...");
            try
            {
                await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, verificationToken);
                Console.WriteLine("[AuthService] ✅ Verification email sent successfully");
            }
            catch (Exception emailEx)
            {
                // Log email error but don't fail registration
                Console.WriteLine($"[AuthService] ❌ Failed to send verification email: {emailEx.Message}");
                Console.WriteLine($"[AuthService] Exception Type: {emailEx.GetType().Name}");
                Console.WriteLine($"[AuthService] Stack Trace: {emailEx.StackTrace}");
                if (emailEx.InnerException != null)
                {
                    Console.WriteLine($"[AuthService] Inner Exception: {emailEx.InnerException.Message}");
                    Console.WriteLine($"[AuthService] Inner Stack Trace: {emailEx.InnerException.StackTrace}");
                }
            }

            return AuthResult.SuccessResult(user);
        }
        catch (Exception ex)
        {
            return AuthResult.FailureResult($"Registration failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Authenticates a user with email and password, returning a JWT token if successful.
    /// Implements account lockout after 5 failed attempts for 30 minutes.
    /// </summary>
    /// <param name="dto">Login credentials</param>
    /// <returns>AuthResult with JWT token if authentication successful</returns>
    public async Task<AuthResult> LoginAsync(LoginDto dto)
    {
        if (dto == null)
            return AuthResult.FailureResult("Login credentials cannot be null");

        try
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null)
                return AuthResult.FailureResult("Invalid email or password");

            // Check if account is locked
            if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow)
            {
                var remainingTime = user.LockoutEnd.Value - DateTime.UtcNow;
                return AuthResult.FailureResult($"Account is locked. Try again in {remainingTime.Minutes} minutes");
            }

            // Check if user is active
            if (!user.IsActive)
                return AuthResult.FailureResult("User account is not active");

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                // Increment failed login attempts
                user.FailedLoginAttempts++;

                // Lock account if 5 failed attempts
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(30);
                    await _userRepository.UpdateAsync(user);
                    await _userRepository.SaveChangesAsync();
                    return AuthResult.FailureResult("Too many failed login attempts. Account locked for 30 minutes");
                }

                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                return AuthResult.FailureResult("Invalid email or password");
            }

            // Check if email is verified
            if (!user.EmailVerified)
                return AuthResult.FailureResult("Email not verified. Please verify your email before logging in");

            // Reset failed login attempts and clear lockout
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Generate JWT token
            var token = GenerateJwtToken(user);

            return AuthResult.SuccessResult(user, token);
        }
        catch (Exception ex)
        {
            return AuthResult.FailureResult($"Login failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Resends the email verification link to the specified email address.
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>AuthResult with success status</returns>
    public async Task<AuthResult> ResendVerificationEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return AuthResult.FailureResult("Email is required");

        try
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                // Return success even if user not found for security reasons
                return AuthResult.SuccessResult(new User(), null);

            // Check if email is already verified
            if (user.EmailVerified)
                return AuthResult.FailureResult("Email is already verified");

            // Generate new verification token
            var verificationToken = GenerateSecureToken();
            var verificationTokenExpiry = DateTime.UtcNow.AddHours(24);

            user.EmailVerificationToken = verificationToken;
            user.EmailVerificationTokenExpiry = verificationTokenExpiry;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Send verification email
            Console.WriteLine("[AuthService] About to resend verification email...");
            try
            {
                await _emailService.SendVerificationEmailAsync(user.Email, user.FullName, verificationToken);
                Console.WriteLine("[AuthService] ✅ Verification email resent successfully");
            }
            catch (Exception emailEx)
            {
                // Log email error but don't fail the request
                Console.WriteLine($"[AuthService] ❌ Failed to resend verification email: {emailEx.Message}");
                Console.WriteLine($"[AuthService] Exception Type: {emailEx.GetType().Name}");
                Console.WriteLine($"[AuthService] Stack Trace: {emailEx.StackTrace}");
                if (emailEx.InnerException != null)
                {
                    Console.WriteLine($"[AuthService] Inner Exception: {emailEx.InnerException.Message}");
                    Console.WriteLine($"[AuthService] Inner Stack Trace: {emailEx.InnerException.StackTrace}");
                }
            }

            return AuthResult.SuccessResult(user);
        }
        catch (Exception ex)
        {
            return AuthResult.FailureResult($"Failed to resend verification email: {ex.Message}");
        }
    }

    /// <summary>
    /// Verifies a user's email address using a verification token.
    /// </summary>
    /// <param name="token">Email verification token</param>
    /// <returns>AuthResult with success status and user information</returns>
    public async Task<AuthResult> VerifyEmailAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return AuthResult.FailureResult("Verification token is required");

        try
        {
            // Find user by verification token
            var user = await _userRepository.GetByEmailVerificationTokenAsync(token);
            if (user == null)
                return AuthResult.FailureResult("Invalid verification token");

            // Check if token has expired
            if (user.EmailVerificationTokenExpiry < DateTime.UtcNow)
                return AuthResult.FailureResult("Verification token has expired");

            // Mark email as verified
            user.EmailVerified = true;
            user.EmailVerificationToken = null;
            user.EmailVerificationTokenExpiry = null;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Send welcome email
            Console.WriteLine("[AuthService] About to send welcome email...");
            try
            {
                await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName);
                Console.WriteLine("[AuthService] ✅ Welcome email sent successfully");
            }
            catch (Exception emailEx)
            {
                // Log email error but don't fail verification
                Console.WriteLine($"[AuthService] ❌ Failed to send welcome email: {emailEx.Message}");
                Console.WriteLine($"[AuthService] Exception Type: {emailEx.GetType().Name}");
                Console.WriteLine($"[AuthService] Stack Trace: {emailEx.StackTrace}");
                if (emailEx.InnerException != null)
                {
                    Console.WriteLine($"[AuthService] Inner Exception: {emailEx.InnerException.Message}");
                    Console.WriteLine($"[AuthService] Inner Stack Trace: {emailEx.InnerException.StackTrace}");
                }
            }

            return AuthResult.SuccessResult(user);
        }
        catch (Exception ex)
        {
            return AuthResult.FailureResult($"Email verification failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Initiates a password reset process by generating a reset token for the user.
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <returns>AuthResult with success status</returns>
    public async Task<AuthResult> RequestPasswordResetAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return AuthResult.FailureResult("Email is required");

        try
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                // Return success even if user not found for security reasons
                return AuthResult.SuccessResult(new User(), null);

            // Generate password reset token
            var resetToken = GenerateSecureToken();
            var resetTokenExpiry = DateTime.UtcNow.AddHours(1);

            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = resetTokenExpiry;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Send password reset email
            Console.WriteLine("[AuthService] About to send password reset email...");
            try
            {
                await _emailService.SendPasswordResetEmailAsync(user.Email, user.FullName, resetToken);
                Console.WriteLine("[AuthService] ✅ Password reset email sent successfully");
            }
            catch (Exception emailEx)
            {
                // Log email error but don't fail password reset request
                Console.WriteLine($"[AuthService] ❌ Failed to send password reset email: {emailEx.Message}");
                Console.WriteLine($"[AuthService] Exception Type: {emailEx.GetType().Name}");
                Console.WriteLine($"[AuthService] Stack Trace: {emailEx.StackTrace}");
                if (emailEx.InnerException != null)
                {
                    Console.WriteLine($"[AuthService] Inner Exception: {emailEx.InnerException.Message}");
                    Console.WriteLine($"[AuthService] Inner Stack Trace: {emailEx.InnerException.StackTrace}");
                }
            }

            return AuthResult.SuccessResult(user);
        }
        catch (Exception ex)
        {
            return AuthResult.FailureResult($"Password reset request failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Resets a user's password using a reset token.
    /// </summary>
    /// <param name="token">Password reset token</param>
    /// <param name="newPassword">New password</param>
    /// <returns>AuthResult with success status</returns>
    public async Task<AuthResult> ResetPasswordAsync(string token, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(token))
            return AuthResult.FailureResult("Reset token is required");

        if (string.IsNullOrWhiteSpace(newPassword))
            return AuthResult.FailureResult("New password is required");

        try
        {
            // Find user by reset token
            var user = await _userRepository.GetByPasswordResetTokenAsync(token);
            if (user == null)
                return AuthResult.FailureResult("Invalid reset token");

            // Check if token has expired
            if (!user.PasswordResetTokenExpiry.HasValue || user.PasswordResetTokenExpiry < DateTime.UtcNow)
                return AuthResult.FailureResult("Password reset token has expired");

            // Hash new password
            var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            // Update user password
            user.PasswordHash = newPasswordHash;
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;
            user.LastPasswordChangeAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            user.FailedLoginAttempts = 0;
            user.LockoutEnd = null;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return AuthResult.SuccessResult(user);
        }
        catch (Exception ex)
        {
            return AuthResult.FailureResult($"Password reset failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates a JWT authentication token for the specified user.
    /// </summary>
    /// <param name="user">The user to generate token for</param>
    /// <returns>JWT token string</returns>
    private string GenerateJwtToken(User user)
    {
        var jwtSecret = _configuration["JWT:Secret"];
        var jwtIssuer = _configuration["JWT:Issuer"];
        var jwtAudience = _configuration["JWT:Audience"];
        var jwtExpiryMinutes = int.TryParse(_configuration["JWT:ExpiryMinutes"], out int jwtExpiry) ? jwtExpiry : 60;

        if (string.IsNullOrEmpty(jwtSecret))
            throw new InvalidOperationException("JWT:Secret is not configured");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
        };

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a cryptographically secure random token for email verification
    /// or password reset operations.
    /// </summary>
    /// <returns>Base64 encoded secure token</returns>
    private static string GenerateSecureToken()
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var tokenData = new byte[32];
            rng.GetBytes(tokenData);
            return Convert.ToBase64String(tokenData);
        }
    }
}
