using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DotNetEnv;
using finance_tracker.backend.Data;
using finance_tracker.backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Load secrets from .env
Env.Load("secrets.env");

// Get current environment
var environment = builder.Environment.EnvironmentName;


var dbUser = "";
var dbPassword = "";
string connectionString = "";

// Set up server and database names for connection string
var dbServer = Environment.GetEnvironmentVariable("SERVER_NAME");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");

if (environment == "Development")
{
    dbUser = Environment.GetEnvironmentVariable("ADMIN_ID");
    dbPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD");

    connectionString = $"Server=tcp:{dbServer}.database.windows.net,1433;Initial Catalog={dbName};Persist Security Info=False;User ID={dbUser};Password={dbPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
}
else
{
    dbUser = Environment.GetEnvironmentVariable("GUEST_ID");
    dbPassword = Environment.GetEnvironmentVariable("GUEST_PASSWORD");

    connectionString = $"Server=tcp:{dbServer}.database.windows.net,1433;Initial Catalog={dbName};Persist Security Info=False;User ID={dbUser};Password={dbPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
}

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
        policy.WithOrigins("DOMAIN.com")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.UseCors("Production");
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
