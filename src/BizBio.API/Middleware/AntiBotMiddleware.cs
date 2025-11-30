using System.Text.RegularExpressions;

namespace BizBio.API.Middleware;

public class AntiBotMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AntiBotMiddleware> _logger;
    private static readonly Regex BotPattern = new Regex(
        @"(bot|crawler|spider|crawling|slurp|mediapartners|bingpreview|googlebot|baiduspider|yandex|duckduck|yahoo|seznambot|exabot|facebot|facebookexternalhit|ia_archiver|twitterbot|whatsapp|telegrambot|applebot|semrush|ahrefs|mj12bot|dotbot|petalbot|screaming frog|sitebulb|linkcheck|wget|curl|python-requests|go-http-client|scrapy|headless|phantom|selenium|webdriver)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled
    );

    public AntiBotMiddleware(RequestDelegate next, ILogger<AntiBotMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add headers to prevent indexing
        context.Response.Headers.Add("X-Robots-Tag", "noindex, nofollow, noarchive, nosnippet");

        var userAgent = context.Request.Headers.UserAgent.ToString();

        // Check if the request is from a known bot/crawler
        if (!string.IsNullOrEmpty(userAgent) && BotPattern.IsMatch(userAgent))
        {
            _logger.LogWarning("Bot detected and blocked: {UserAgent} | IP: {IpAddress} | Path: {Path}",
                userAgent,
                context.Connection.RemoteIpAddress,
                context.Request.Path);

            // Option 1: Block completely (uncomment to enable)
            // context.Response.StatusCode = 403;
            // await context.Response.WriteAsync("Access forbidden");
            // return;

            // Option 2: Just log and allow (current behavior - good bots like Googlebot might need API access for certain features)
            // The request continues but won't be indexed due to X-Robots-Tag header
        }

        await _next(context);
    }
}
