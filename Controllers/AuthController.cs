using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PRN232.Backend.Data;
using PRN232.Backend.Models;

namespace PRN232.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest req)
    {
        try
        {
            Console.WriteLine($"Registration attempt for email: {req.Email}");
            
            // Validate input
            if (string.IsNullOrEmpty(req.Email) || string.IsNullOrEmpty(req.Password) || string.IsNullOrEmpty(req.Name))
            {
                Console.WriteLine("Validation failed: Missing required fields");
                return BadRequest("All fields are required");
            }

            // Check if email already exists
            var existingUser = _db.Users.FirstOrDefault(u => u.Email == req.Email);
            if (existingUser != null)
            {
                Console.WriteLine($"Email already exists: {req.Email}");
                return BadRequest("Email already in use");
            }

            // Create new user
            var user = new User 
            { 
                Email = req.Email, 
                Name = req.Name, 
                CreatedAt = DateTime.UtcNow 
            };
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password);
            
            Console.WriteLine($"Adding user to database: {user.Email}");
            _db.Users.Add(user);
            
            Console.WriteLine("Saving changes to database...");
            var saveResult = _db.SaveChanges();
            Console.WriteLine($"Save result: {saveResult} rows affected");
            
            Console.WriteLine($"User created successfully with ID: {user.Id}");
            return Ok(new { Id = user.Id, Email = user.Email, Name = user.Name });
        }
        catch (Exception ex)
        {
            // Log the error for debugging
            Console.WriteLine($"Registration error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, $"Internal server error during registration: {ex.Message}");
        }
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest req)
    {
        var user = _db.Users.SingleOrDefault(u => u.Email == req.Email);
        if (user == null) return Unauthorized();
        var ok = BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash);
        if (!ok) return Unauthorized();

        var token = GenerateToken(user);
        return Ok(new { token });
    }

    private string GenerateToken(User user)
    {
        var key = _config["Jwt:Key"] ?? "super_secret_key_123!";
        var issuer = _config["Jwt:Issuer"] ?? "prn232";
        var claims = new[] { 
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer, issuer, claims, expires: DateTime.UtcNow.AddDays(7), signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record RegisterRequest(string Email, string Password, string Name);
public record LoginRequest(string Email, string Password);
