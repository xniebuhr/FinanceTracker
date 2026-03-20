using DotNetEnv;
using finance_tracker.backend.Data;
using finance_tracker.backend.Models.Auth;
using finance_tracker.backend.Models.Users;
using finance_tracker.backend.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

// ============================
// Load configuration and secrets
// ============================

// Load secrets from .env for dev env
if (builder.Environment.IsDevelopment())
{
    Env.Load("secrets.env");
}

// And env variables to configuration
builder.Configuration.AddEnvironmentVariables();

// ============================
// Build database connection string
// ============================

string connectionString;

// Try to get connection string first from docker
connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// If it has placeholders, build from env vars (local dev)
if (!string.IsNullOrEmpty(connectionString) && connectionString.Contains("{SERVER_NAME}"))
{
    connectionString = connectionString
        .Replace("{SERVER_NAME}", Environment.GetEnvironmentVariable("SERVER_NAME"))
        .Replace("{DB_NAME}", Environment.GetEnvironmentVariable("DB_NAME"))
        .Replace("{DB_USER}", Environment.GetEnvironmentVariable(
            builder.Environment.IsDevelopment() ? "ADMIN_ID" : "GUEST_ID"))
        .Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable(
            builder.Environment.IsDevelopment() ? "ADMIN_PASSWORD" : "GUEST_PASSWORD"));
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Database connection string is not configured");
}


// ============================
// Register EF Core + Identity
// ============================

// Add EF Core DbContext with SQL Server provider
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)
    ));

// Add ASP.NET Core Identity with ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


// ============================
// Configure CORS policies
// ============================

// Get allowed origins from evironment
var corsOrigins = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS")
    ?? (builder.Environment.IsDevelopment()
        ? "http://localhost:4200"
        : "https://app-finance-tracker-6n8mk7w3.azurewebsites.net");

var allowedOrigins = corsOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy(builder.Environment.IsDevelopment() ? "Development" : "Production", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();

        if (!builder.Environment.IsDevelopment())
        {
            policy.AllowCredentials();
        }
    });
});


// ============================
// Configure JWT authentication
// ============================

// Bind JwtSettings from config 
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

// Make JwtSettings injectable as a singleton
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<JwtSettings>>().Value);

// Register TokenService for DI
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();

// Pull JWT key from environment first
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? builder.Configuration["Jwt:Key"];

if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("JWT signing key is not configured. Set JWT_KEY environment variable or Jwt:Key in appsettings.");
}

// Pull JWT values from config
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

// Configure authentication middleware
builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });


// ============================
// Add rate limiting
// ============================

// Configure rate limiting middleware
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ============================
// Add controllers, Swagger, and authorization
// ============================

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

// ============================
// Build the app
// ============================

var app = builder.Build();

// Trust Nginx proxy headers inside Docker network
var forwardedOptions = new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
};
forwardedOptions.KnownNetworks.Clear();
forwardedOptions.KnownProxies.Clear();

app.UseForwardedHeaders(forwardedOptions);

// Health endpoint
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" }));

// Database health endpoint
app.MapGet("/api/health/db", async (ApplicationDbContext dbContext) =>
{
    try
    {
        bool canConnect = await dbContext.Database.CanConnectAsync();

        if (canConnect)
        {
            return Results.Ok(new { status = "healthy", database = "connected" });
        }

        return Results.Problem("Database connection failed", statusCode: 500);
    }
    catch (Exception ex)
    {
        // Catching the exception is important so the whole app doesn't crash 
        // if the DB is completely unreachable
        return Results.Problem($"Database error: {ex.Message}", statusCode: 500);
    }
});

// ============================
// Configure middleware pipeline
// ============================

if (app.Environment.IsDevelopment())
{
    // Dev: Allow Angular dev server + Swagger UI
    app.UseCors("Development");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Prod: Enforce HSTS + production CORS
    app.UseHsts();
    app.UseCors("Production");
}

// Enable rate limiting
app.UseRateLimiter();

// Enable authentication & authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers to routes
app.MapControllers();

// Run the app
app.Run();