using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TopScorers.API.DTOs;

namespace TopScorers.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IConfiguration configuration, ILogger<AuthController> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }


    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        _logger.LogInformation($"Login attempt for user: {request.Username}");

        if (request.Username == "admin" && request.Password == "password123")
        {
            var token = GenerateJwtToken(request.Username);
            _logger.LogInformation($"Token generated for user: {request.Username}");
            return Ok(new { Token = token, Username = request.Username });
        }

        _logger.LogWarning($"Failed login attempt for user: {request.Username}");
        return Unauthorized(new { Message = "Invalid username or password" });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] LoginRequest request)
    {
        _logger.LogInformation($"Registration attempt for user: {request.Username}");

        if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { Message = "Username and password are required" });
        }

        if (request.Username.Length < 3 || request.Password.Length < 6)
        {
            return BadRequest(new { Message = "Username must be at least 3 characters and password at least 6 characters" });
        }

        // In production, hash the password and save to database
        var token = GenerateJwtToken(request.Username);
        return Ok(new
        {
            Token = token,
            Username = request.Username,
            Message = "User registered successfully"
        });
    }

    private string GenerateJwtToken(string username)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "YourSuperSecretKeyForJWTAuthentication12345!";
        var issuer = _configuration["Jwt:Issuer"] ?? "TopScorersAPI";
        var audience = _configuration["Jwt:Audience"] ?? "TopScorersClient";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create claims - will be included in the token
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "User"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        // Create the token
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),  // Token expires in 1 hour
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}