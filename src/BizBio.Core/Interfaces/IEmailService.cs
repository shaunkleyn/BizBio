namespace BizBio.Core.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    Task SendVerificationEmailAsync(string toEmail, string userName, string verificationToken, string verificationCode);
    Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken);
    Task SendWelcomeEmailAsync(string toEmail, string userName);
}
