using BizBio.API.Filters;
using BizBio.API.Telemetry;
using BizBio.Core.Interfaces;
using BizBio.Infrastructure.Data;
using BizBio.Infrastructure.Repositories;
using BizBio.Infrastructure.Services;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Application Insights Logging Integration
builder.Logging.AddApplicationInsights(
    configureTelemetryConfiguration: (config) =>
        config.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"],
    configureApplicationInsightsLoggerOptions: (options) => { }
);

// Add services to the container
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApplicationInsightsExceptionFilter>();
});

// Database Configuration
// AutoDetect works with both MySQL and MariaDB
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mysqlOptions => mysqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("JWT");
var secretKey = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
        .WithOrigins(
            "http://localhost:3000",
            "http://localhost:5173",
            "http://localhost:5000",
            "https://localhost:5443",
            "https://bizbio.co.za",
            "https://www.bizbio.co.za",
            "https://api.bizbio.co.za",
            "https://ui.bizbio.co.za"
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

// Repository Registration
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISubscriptionTierRepository, SubscriptionTierRepository>();
builder.Services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();
builder.Services.AddScoped<IRestaurantTableRepository, RestaurantTableRepository>();

// Service Registration
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPaymentService, PayFastService>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();

// Session Configuration (for NFC scan tracking)
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Swagger/OpenAPI Configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BizBio API",
        Version = "v1",
        Description = "BizBio Platform API - Digital Business Cards, Menus & Catalogs",
        Contact = new OpenApiContact
        {
            Name = "BizBio Support",
            Email = "support@bizbio.co.za"
        }
        
    });

    //options.AddServer(new OpenApiServer
    //{
    //    Url = "https://api.bizbio.co.za"
    //});

    // JWT Bearer Authentication in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Application Insights Telemetry
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
    options.EnableAdaptiveSampling = builder.Configuration.GetValue<bool>("ApplicationInsights:EnableAdaptiveSampling", true);
    options.EnablePerformanceCounterCollectionModule = builder.Configuration.GetValue<bool>("ApplicationInsights:EnablePerformanceCounterCollectionModule", true);
    options.EnableDependencyTrackingTelemetryModule = builder.Configuration.GetValue<bool>("ApplicationInsights:EnableDependencyTrackingTelemetryModule", true);
    options.EnableEventCounterCollectionModule = builder.Configuration.GetValue<bool>("ApplicationInsights:EnableEventCounterCollectionModule", true);
    options.EnableQuickPulseMetricStream = builder.Configuration.GetValue<bool>("ApplicationInsights:EnableQuickPulseMetricStream", true);
    options.EnableHeartbeat = builder.Configuration.GetValue<bool>("ApplicationInsights:EnableHeartbeat", true);
    options.AddAutoCollectedMetricExtractor = builder.Configuration.GetValue<bool>("ApplicationInsights:AddAutoCollectedMetricExtractor", true);
});

// Register custom telemetry initializer
builder.Services.AddSingleton<ITelemetryInitializer, BizBioTelemetryInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "BizBio API v1");
        options.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
//}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseCors("AllowFrontend");

//app.UseHttpsRedirection();

// Serve static files from the uploads directory
var uploadPath = builder.Configuration["FileUpload:Path"] ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadPath),
    RequestPath = "/uploads"
});

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Apply any pending migrations
        await context.Database.MigrateAsync();

        // Seed the database
        await DbSeeder.SeedAsync(context);

        Console.WriteLine("Database seeded successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database");
    }
}

app.Run();
