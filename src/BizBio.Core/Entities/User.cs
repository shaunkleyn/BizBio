namespace BizBio.Core.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class User
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = null!;

    [Required]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = null!;

    public bool EmailVerified { get; set; } = false;

    public bool IsActive { get; set; } = true;

    [MaxLength(255)]
    public string? EmailVerificationToken { get; set; }

    public DateTime? EmailVerificationTokenExpiry { get; set; }

    [MaxLength(255)]
    public string? PasswordResetToken { get; set; }

    public DateTime? PasswordResetTokenExpiry { get; set; }

    public DateTime? LastPasswordChangeAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public int FailedLoginAttempts { get; set; } = 0;

    public DateTime? LockoutEnd { get; set; }

    [MaxLength(3)]
    public string Currency { get; set; } = "ZAR";

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    // Navigation properties
    public virtual ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();

    public virtual ICollection<Profile> Profiles { get; set; } = new List<Profile>();
}
