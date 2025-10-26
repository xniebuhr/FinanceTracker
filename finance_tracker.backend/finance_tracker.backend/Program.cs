using DotNetEnv;
using finance_tracker.backend.Data;
using finance_tracker.backend.Models.Auth;
using finance_tracker.backend.Models.Users;
using finance_tracker.backend.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Load secrets from .env
Env.Load("secrets.env");

// Include environment vars in config
builder.Configuration.AddEnvironmentVariables();

// Build database connection
var rawConnection = builder.Configuration.GetConnectionString("DefaultConnection");

var connectionString = rawConnection
    .Replace("{SERVER_NAME}", Environment.GetEnvironmentVariable("SERVER_NAME"))
    .Replace("{DB_NAME}", Environment.GetEnvironmentVariable("DB_NAME"))
    .Replace("{DB_USER}", Environment.GetEnvironmentVariable(
        builder.Environment.IsDevelopment() ? "ADMIN_ID" : "GUEST_ID"))
    .Replace("{DB_PASSWORD}", Environment.GetEnvironmentVariable(
        builder.Environment.IsDevelopment() ? "ADMIN_PASSWORD" : "GUEST_PASSWORD"));

// Add EF Core & Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("Development", policy =>
    {    
        policy.WithOrigins("http://localhost:4200") // Angular dev server
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins("https://DOMAIN.com")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Bind JwtSettings from config 
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton(resolver =>
    resolver.GetRequiredService<IOptions<JwtSettings>>().Value);

// Register TokenService for DI
builder.Services.AddSingleton<TokenService>();

// Get JWT settings from config
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(
    options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

var app = builder.Build();

// Use appropriate environment
if (app.Environment.IsDevelopment())
{
    app.UseCors("Development");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
    app.UseCors("Production");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();