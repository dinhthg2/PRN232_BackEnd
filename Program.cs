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
        policy.WithOrigins("http://localhost:3000") // Next.js default port
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

// Đảm bảo database được tạo và có dữ liệu mẫu (cho môi trường development)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    try 
    {
        // Chỉ tạo database và các bảng nếu chưa tồn tại
        db.Database.EnsureCreated();
        Console.WriteLine("Database đã được tạo hoặc đã tồn tại");
        
        SeedData.EnsureSeedData(db);
        Console.WriteLine("Quá trình khởi tạo dữ liệu hoàn tất");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Lỗi khi khởi tạo database: {ex.Message}");
        // Tiếp tục chạy ứng dụng ngay cả khi có lỗi database
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Sử dụng CORS
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
