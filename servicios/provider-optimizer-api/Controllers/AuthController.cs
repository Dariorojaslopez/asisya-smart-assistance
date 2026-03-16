using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProviderOptimizerService.Domain;
using ProviderOptimizerService.DTOs;
using ProviderOptimizerService.Infrastructure;

namespace ProviderOptimizerService.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    /// <summary>
    /// Indica si se puede registrar el primer usuario (no hay ninguno en la BD).
    /// Permite a la UI mostrar el flujo de "crear primera cuenta" sin credenciales hardcodeadas.
    /// </summary>
    [HttpGet("setup")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> Setup(CancellationToken cancellationToken)
    {
        var canRegister = !await _context.Users.AnyAsync(cancellationToken);
        return Ok(new { canRegister });
    }

    /// <summary>
    /// Autentica un usuario y devuelve un JWT.
    /// </summary>
    [HttpPost("login")]
    [Microsoft.AspNetCore.Authorization.AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("Username and password are required.");

        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return StatusCode(401, new { message = "Invalid username or password." });

        var secret = _configuration["JWT_SECRET"];
        if (string.IsNullOrEmpty(secret))
            return StatusCode(500, new { message = "JWT configuration is missing." });

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("userId", user.Id.ToString()),
            new Claim("username", user.Username),
            new Claim("role", user.Role ?? "User"),
        };

        var token = new JwtSecurityToken(
            claims: claims.AsEnumerable(),
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new LoginResponse { Token = tokenString });
    }
}
