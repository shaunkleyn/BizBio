# Bot and Search Engine Protection

This document describes the multi-layered approach to preventing bots and search engines from crawling and indexing the BizBio API.

## Table of Contents

- [Overview](#overview)
- [Protection Layers](#protection-layers)
- [Configuration](#configuration)
- [Testing](#testing)
- [Customization](#customization)

## Overview

The API implements multiple layers of protection to prevent:
- Search engine indexing (Google, Bing, etc.)
- Bot crawling and scraping
- Automated attacks and abuse
- Excessive API usage

## Protection Layers

### 1. robots.txt File

**Location**: `wwwroot/robots.txt`

**Purpose**: Standard file that tells search engines not to crawl or index the API.

**Content**:
```
User-agent: *
Disallow: /
```

**Access**: Available at `https://api.bizbio.co.za/robots.txt`

**Effectiveness**: Prevents well-behaved search engines (Google, Bing, etc.) from crawling.

### 2. X-Robots-Tag Header

**Implementation**: `AntiBotMiddleware.cs`

**Purpose**: HTTP header that instructs search engines not to index any API responses.

**Header Added**:
```
X-Robots-Tag: noindex, nofollow, noarchive, nosnippet
```

**What it does**:
- `noindex`: Don't show this page in search results
- `nofollow`: Don't follow links on this page
- `noarchive`: Don't show cached version
- `nosnippet`: Don't show snippets in search results

**Effectiveness**: Provides an additional layer of protection beyond robots.txt. Works even if robots.txt is ignored.

### 3. Bot Detection and Blocking

**Implementation**: `AntiBotMiddleware.cs`

**Purpose**: Detects and optionally blocks known bots and crawlers based on User-Agent.

**Detected Bots**:
- Googlebot, Bingbot, Yahoo Slurp
- Social media crawlers (Facebook, Twitter, WhatsApp)
- SEO tools (SEMrush, Ahrefs, Screaming Frog)
- Generic crawlers and scrapers
- Headless browsers (Selenium, Puppeteer, PhantomJS)
- HTTP clients (wget, curl, Python requests)

**Current Behavior**:
- Logs bot detection
- Adds X-Robots-Tag header
- **Allows request to continue** (configurable)

**To Block Bots Completely**:

Edit `AntiBotMiddleware.cs` and uncomment these lines:

```csharp
// Option 1: Block completely (uncomment to enable)
context.Response.StatusCode = 403;
await context.Response.WriteAsync("Access forbidden");
return;
```

**Why Not Blocking by Default**:
- Some legitimate services may use bot-like user agents
- Social media preview features need to fetch metadata
- Monitoring tools may need access
- The X-Robots-Tag prevents indexing anyway

### 4. Rate Limiting

**Implementation**: AspNetCoreRateLimit package

**Purpose**: Prevent API abuse by limiting the number of requests per IP address.

**Configuration** (`appsettings.json`):

**Production Limits**:
```json
{
  "IpRateLimiting": {
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 60        // 60 requests per minute per IP
      },
      {
        "Endpoint": "*",
        "Period": "1h",
        "Limit": 1000      // 1000 requests per hour per IP
      },
      {
        "Endpoint": "*/auth/login",
        "Period": "1m",
        "Limit": 5         // 5 login attempts per minute
      },
      {
        "Endpoint": "*/auth/register",
        "Period": "1h",
        "Limit": 3         // 3 registrations per hour
      }
    ]
  }
}
```

**Development Limits** (`appsettings.Development.json`):
- 300 requests per minute
- 5000 requests per hour
- No specific endpoint limits

**What Happens When Limit Exceeded**:
- HTTP 429 (Too Many Requests) response
- Standard retry-after header
- Request is logged

**Benefits**:
- Prevents brute force attacks
- Stops aggressive scrapers
- Protects against DDoS
- Controls API costs

## Configuration

### Adjusting Rate Limits

Edit `appsettings.json`:

```json
{
  "Endpoint": "*/api/v1/subscriptions/tiers",
  "Period": "1m",
  "Limit": 10
}
```

**Period Options**:
- `1s` - 1 second
- `1m` - 1 minute
- `1h` - 1 hour
- `1d` - 1 day

### Whitelisting IPs

To allow unlimited access from specific IPs, add to `appsettings.json`:

```json
"IpRateLimiting": {
  "IpWhitelist": [
    "127.0.0.1",
    "::1",
    "your-monitoring-server-ip"
  ]
}
```

### Blocking Specific User Agents

Edit `AntiBotMiddleware.cs` to add patterns:

```csharp
private static readonly Regex BotPattern = new Regex(
    @"(bot|crawler|your-pattern-here)",
    RegexOptions.IgnoreCase | RegexOptions.Compiled
);
```

### Enabling Complete Bot Blocking

In `AntiBotMiddleware.cs`, uncomment:

```csharp
context.Response.StatusCode = 403;
await context.Response.WriteAsync("Access forbidden");
return;
```

## Testing

### 1. Test robots.txt

```bash
curl https://api.bizbio.co.za/robots.txt
```

Expected output:
```
User-agent: *
Disallow: /
```

### 2. Test X-Robots-Tag Header

```bash
curl -I https://api.bizbio.co.za/api/v1/subscriptions/tiers
```

Expected headers:
```
X-Robots-Tag: noindex, nofollow, noarchive, nosnippet
```

### 3. Test Bot Detection

```bash
curl -H "User-Agent: Googlebot/2.1" https://api.bizbio.co.za/api/v1/subscriptions/tiers
```

Check logs for:
```
Bot detected and blocked: Googlebot/2.1 | IP: xxx.xxx.xxx.xxx
```

### 4. Test Rate Limiting

Run multiple requests rapidly:

```bash
for i in {1..70}; do
  curl https://api.bizbio.co.za/api/v1/subscriptions/tiers
done
```

After 60 requests in a minute, you should get:
```json
{
  "statusCode": 429,
  "message": "API calls quota exceeded! Maximum allowed: 60 per 1m."
}
```

### 5. Test Login Rate Limiting

Try 6 login attempts in one minute:

```bash
for i in {1..6}; do
  curl -X POST https://api.bizbio.co.za/api/v1/auth/login \
    -H "Content-Type: application/json" \
    -d '{"email":"test@example.com","password":"test"}'
done
```

The 6th request should return 429.

## Customization

### Allow Specific Bots

To allow specific bots (e.g., for social media previews), modify `AntiBotMiddleware.cs`:

```csharp
// List of allowed bots
private static readonly string[] AllowedBots = new[]
{
    "facebookexternalhit",
    "twitterbot",
    "linkedinbot"
};

public async Task InvokeAsync(HttpContext context)
{
    context.Response.Headers.Add("X-Robots-Tag", "noindex, nofollow, noarchive, nosnippet");

    var userAgent = context.Request.Headers.UserAgent.ToString();

    // Check if it's an allowed bot
    if (!string.IsNullOrEmpty(userAgent) &&
        AllowedBots.Any(bot => userAgent.Contains(bot, StringComparison.OrdinalIgnoreCase)))
    {
        await _next(context);
        return;
    }

    // Continue with regular bot detection...
}
```

### Custom Rate Limit Response

Create a custom response in Program.cs:

```csharp
builder.Services.Configure<IpRateLimitOptions>(options =>
{
    options.QuotaExceededResponse = new QuotaExceededResponse
    {
        Content = "{{ \"error\": \"Too many requests. Please slow down.\" }}",
        ContentType = "application/json",
        StatusCode = 429
    };
});
```

### Endpoint-Specific Configuration

For more granular control:

```json
"IpRateLimiting": {
  "EndpointWhitelist": [
    "get:/api/v1/health"
  ],
  "GeneralRules": [
    {
      "Endpoint": "post:/api/v1/auth/*",
      "Period": "1m",
      "Limit": 3
    }
  ]
}
```

## Monitoring

### Application Insights Queries

**Bot Detection Events**:
```kusto
traces
| where message contains "Bot detected"
| summarize count() by tostring(customDimensions.UserAgent)
| order by count_ desc
```

**Rate Limit Violations**:
```kusto
requests
| where resultCode == 429
| summarize count() by bin(timestamp, 1h), client_IP
| render timechart
```

**Top User Agents**:
```kusto
requests
| summarize count() by client_Browser
| order by count_ desc
| take 20
```

## Security Best Practices

1. **Keep Bot Patterns Updated**: Regularly update the bot detection regex with new patterns

2. **Monitor Logs**: Watch for suspicious patterns in bot detection logs

3. **Adjust Rate Limits**: Start conservative and relax as needed based on legitimate usage

4. **Use HTTPS Only**: Ensure all traffic is encrypted

5. **Implement Authentication**: Rate limit should be combined with proper authentication

6. **IP Whitelisting**: Whitelist your own monitoring and testing IPs

7. **Regular Reviews**: Periodically review blocked requests to ensure no false positives

## Common Issues

### False Positives

**Issue**: Legitimate requests being blocked as bots

**Solution**:
1. Check the User-Agent in logs
2. Add pattern to allowed list
3. Or adjust the bot detection regex

### Rate Limit Too Restrictive

**Issue**: Legitimate users hitting rate limits

**Solution**:
1. Increase limits in `appsettings.json`
2. Consider per-user limits instead of IP-based
3. Implement API keys for higher limits

### Social Media Previews Not Working

**Issue**: Link previews not showing on social media

**Solution**:
1. Whitelist social media bots in `AntiBotMiddleware.cs`
2. Create specific endpoints for Open Graph metadata
3. Exclude preview endpoints from bot blocking

## Additional Resources

- [AspNetCoreRateLimit Documentation](https://github.com/stefanprodan/AspNetCoreRateLimit)
- [robots.txt Specification](https://www.robotstxt.org/)
- [X-Robots-Tag Documentation](https://developers.google.com/search/docs/crawling-indexing/robots-meta-tag)
- [OWASP API Security](https://owasp.org/www-project-api-security/)
