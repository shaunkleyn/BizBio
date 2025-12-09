using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using BizBio.Core.Interfaces;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BizBio.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly TelemetryClient _telemetryClient;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;

    public EmailService(
        IConfiguration configuration,
        ILogger<EmailService> logger,
        TelemetryClient telemetryClient)
    {
        _configuration = configuration;
        _logger = logger;
        _telemetryClient = telemetryClient;
        _smtpHost = configuration["Email:SmtpHost"] ?? "smtp.domains.co.za";
        _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
        _smtpUsername = configuration["Email:SmtpUsername"] ?? "";
        _smtpPassword = configuration["Email:SmtpPassword"] ?? "";
        _fromEmail = configuration["Email:FromEmail"] ?? "";
        _fromName = configuration["Email:FromName"] ?? "BizBio";
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Attempting to send email to {Email} with subject: {Subject}", toEmail, subject);
            _logger.LogDebug("SMTP Host: {Host}:{Port}, User: {User}", _smtpHost, _smtpPort, _smtpUsername);

            // Track dependency for SMTP call
            var dependency = new DependencyTelemetry
            {
                Name = "SMTP Email Send",
                Type = "SMTP",
                Target = $"{_smtpHost}:{_smtpPort}",
                Data = subject
            };

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                Timeout = 30000, // 30 seconds
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false
            };

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_fromEmail, _fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);

            _logger.LogInformation("Email sent successfully to {Email}", toEmail);

            dependency.Success = true;
            dependency.Duration = stopwatch.Elapsed;
            _telemetryClient.TrackDependency(dependency);

            _telemetryClient.TrackEvent("EmailSent", new Dictionary<string, string>
            {
                { "To", toEmail },
                { "Subject", subject },
                { "SmtpHost", _smtpHost }
            });
        }
        catch (SmtpException smtpEx)
        {
            _logger.LogError(smtpEx, "SMTP error sending email to {Email}. Status: {Status}", toEmail, smtpEx.StatusCode);

            var dependency = new DependencyTelemetry
            {
                Name = "SMTP Email Send",
                Type = "SMTP",
                Target = $"{_smtpHost}:{_smtpPort}",
                Data = subject,
                Success = false,
                Duration = stopwatch.Elapsed
            };
            _telemetryClient.TrackDependency(dependency);

            _telemetryClient.TrackException(smtpEx, new Dictionary<string, string>
            {
                { "Operation", "SendEmail" },
                { "To", toEmail },
                { "Subject", subject },
                { "SmtpStatusCode", smtpEx.StatusCode.ToString() }
            });
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Email}", toEmail);

            _telemetryClient.TrackException(ex, new Dictionary<string, string>
            {
                { "Operation", "SendEmail" },
                { "To", toEmail },
                { "Subject", subject }
            });
            throw;
        }
    }

    public async Task SendVerificationEmailAsync(string toEmail, string userName, string verificationToken)
    {
        var subject = "Verify Your BizBio Account";

        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px; }}
        .button {{ display: inline-block; background: #667eea; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Welcome to BizBio!</h1>
        </div>
        <div class='content'>
            <h2>Hi {userName},</h2>
            <p>Thank you for creating your BizBio account. We're excited to have you on board!</p>
            <p>To complete your registration and verify your email address, please click the button below:</p>
            <div style='text-align: center;'>
                <a href='https://bizbio.co.za/verify-email?token={verificationToken}' class='button'>Verify Email Address</a>
            </div>
            <p>Or copy and paste this link into your browser:</p>
            <p style='background: white; padding: 15px; border-radius: 5px; word-break: break-all;'>
                https://bizbio.co.za/verify-email?token={verificationToken}
            </p>
            <p><strong>This link will expire in 24 hours.</strong></p>
            <p>If you didn't create a BizBio account, you can safely ignore this email.</p>
            <hr style='border: none; border-top: 1px solid #e0e0e0; margin: 20px 0;'>
            <p style='font-size: 12px; color: #666;'>
                <strong>&#128161; Tip:</strong> Can't find this email? Check your spam or junk folder and mark it as ""Not Spam"" to ensure you receive future emails from us.
            </p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 BizBio. All rights reserved.</p>
            <p>BizBio - Digital Business Cards &amp; Smart Menus</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, htmlBody);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken)
    {
        var subject = "Reset Your BizBio Password";

        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px; }}
        .button {{ display: inline-block; background: #f5576c; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
        .warning {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Password Reset Request</h1>
        </div>
        <div class='content'>
            <h2>Hi {userName},</h2>
            <p>We received a request to reset your BizBio account password.</p>
            <p>Click the button below to reset your password:</p>
            <div style='text-align: center;'>
                <a href='https://bizbio.co.za/reset-password?token={resetToken}' class='button'>Reset Password</a>
            </div>
            <p>Or copy and paste this link into your browser:</p>
            <p style='background: white; padding: 15px; border-radius: 5px; word-break: break-all;'>
                https://bizbio.co.za/reset-password?token={resetToken}
            </p>
            <div class='warning'>
                <strong>⚠️ Security Notice:</strong>
                <ul style='margin: 10px 0 0 0;'>
                    <li>This link will expire in 1 hour</li>
                    <li>If you didn't request this reset, please ignore this email</li>
                    <li>Your password won't change unless you click the link above</li>
                </ul>
            </div>
            <hr style='border: none; border-top: 1px solid #e0e0e0; margin: 20px 0;'>
            <p style='font-size: 12px; color: #666;'>
                <strong>&#128161; Tip:</strong> Can't find this email? Check your spam or junk folder and mark it as ""Not Spam"" to ensure you receive future emails from us.
            </p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 BizBio. All rights reserved.</p>
            <p>If you have any questions, contact us at support@bizbio.co.za</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, htmlBody);
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string userName)
    {
        var subject = "Welcome to BizBio - Let's Get Started!";

        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 8px 8px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 8px 8px; }}
        .button {{ display: inline-block; background: #667eea; color: white; padding: 12px 30px; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .feature-box {{ background: white; padding: 20px; margin: 15px 0; border-radius: 5px; border-left: 4px solid #667eea; }}
        .footer {{ text-align: center; margin-top: 30px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Welcome to BizBio!</h1>
        </div>
        <div class='content'>
            <h2>Hi {userName},</h2>
            <p>Your email has been verified successfully! You're now ready to start building your digital presence.</p>

            <h3>What You Can Do Now:</h3>

            <div class='feature-box'>
                <h4>📇 Create Your Digital Business Card</h4>
                <p>Design a professional digital business card that you can share instantly.</p>
            </div>

            <div class='feature-box'>
                <h4>🍽️ Build Your Smart Menu</h4>
                <p>Create an interactive menu for your restaurant with NFC table integration.</p>
            </div>

            <div class='feature-box'>
                <h4>🏪 Showcase Your Catalog</h4>
                <p>Display your products and services in an elegant digital catalog.</p>
            </div>

            <div style='text-align: center;'>
                <a href='https://bizbio.co.za/dashboard' class='button'>Go to Dashboard</a>
            </div>

            <p style='margin-top: 30px;'>Need help getting started? Check out our <a href='https://bizbio.co.za/help'>Help Center</a> or reply to this email.</p>
        </div>
        <div class='footer'>
            <p>&copy; 2025 BizBio. All rights reserved.</p>
            <p>Elevate your business with digital innovation</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, htmlBody);
    }
}
