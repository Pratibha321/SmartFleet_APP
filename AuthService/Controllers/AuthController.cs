using AuthService.Data;
using AuthService.Models;
using AuthService.Endpoints;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AuthDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { error = "username and password required" });

        // Demo: plaintext check. Replace with hashed password check in prod.
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username == req.Username && u.Password == req.Password);
        if (user == null) return Unauthorized(new { error = "invalid credentials" });

        var jwtKey = _config["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key not set");
        var jwtIssuer = _config["Jwt:Issuer"] ?? "SmartFleet";

        var token = JwtTokenGenerator.GenerateToken(user, jwtKey, jwtIssuer, expireMinutes: 240);

        return Ok(new
        {
            token,
            role = user.Role,
            expiresAt = DateTime.UtcNow.AddMinutes(240)
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
    {
        if (req == null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { error = "username and password required" });

        if (await _db.Users.AnyAsync(u => u.Username == req.Username))
            return Conflict(new { error = "user already exists" });

        var user = new ApplicationUser
        {
            Username = req.Username,
            Password = req.Password,
            Role = string.IsNullOrWhiteSpace(req.Role) ? "Driver" : req.Role,
            FullName = req.FullName ?? string.Empty
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new { id = user.Id, username = user.Username, role = user.Role });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var u = await _db.Users.FindAsync(id);
        if (u == null) return NotFound();
        return Ok(new { u.Id, u.Username, u.Role, u.FullName });
    }

    // Simple request DTOs
    public class LoginRequest { public string Username { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; }
    public class RegisterRequest { public string Username { get; set; } = string.Empty; public string Password { get; set; } = string.Empty; public string Role { get; set; } = "Driver"; public string FullName { get; set; } = string.Empty; }
}
