using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PRN232.Backend.Data;


var builder = WebApplication.CreateBuilder(args);

// Configuration
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Thêm CORS để Frontend có thể gọi API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        // Get allowed origins from configuration or environment
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
                            ?? new[] { "http://localhost:3000" };
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// DbContext - Sử dụng NeonConnection thay vì DefaultConnection
var connectionString = builder.Configuration.GetConnectionString("NeonConnection") 
                      ?? builder.Configuration.GetConnectionString("DefaultConnection")
                      ?? "Host=localhost;Database=prn232_db;Username=postgres;Password=postgres;";
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

// (BCrypt password hashing used directly)

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "super_secret_key_123!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "prn232";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

var app = builder.Build();

// Initialize database with better error handling
try 
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    logger.LogInformation("Starting database initialization...");
    
    // Test database connection first
    await db.Database.CanConnectAsync();
    logger.LogInformation("Database connection successful");
    
    // Ensure database is created
    db.Database.EnsureCreated();
    logger.LogInformation("Database schema ensured");
    
    // Seed initial data
    SeedData.EnsureSeedData(db);
    logger.LogInformation("Database seeding completed");
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Database initialization failed: {Message}", ex.Message);
    
    // In production, we might want to fail fast if DB is not available
    if (app.Environment.IsProduction())
    {
        logger.LogCritical("Database connection failed in production. Exiting...");
        throw;
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Sử dụng CORS
app.UseCors("AllowFrontend");

// Configure for production deployment (Render.com)
if (app.Environment.IsProduction())
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "80";
    app.Urls.Add($"http://0.0.0.0:{port}");
    
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Production mode: Listening on port {Port}", port);
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Add health check endpoint for deployment verification
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow });

app.Run();
