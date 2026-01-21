using System.ComponentModel.DataAnnotations;

namespace BizBio.Core.DTOs;

/// <summary>
/// DTO for development-only email verification bypass
/// </summary>
public class DevVerifyEmailDto
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
}
