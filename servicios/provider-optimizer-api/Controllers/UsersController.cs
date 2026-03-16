using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderOptimizerService.Domain;
using ProviderOptimizerService.DTOs;
using ProviderOptimizerService.Infrastructure;

namespace ProviderOptimizerService.Controllers;

/// <summary>
/// CRUD de usuarios. Sin usuarios hardcodeados; todos se gestionan en PostgreSQL.
/// El primer usuario se puede crear sin JWT (configuración inicial).
/// </summary>
[ApiController]
[Route("users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UsersController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Lista todos los usuarios.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var list = await _context.Users
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .Select(u => ToResponse(u))
            .ToListAsync(cancellationToken);
        return Ok(list);
    }

    /// <summary>
    /// Obtiene un usuario por ID.
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null)
            return NotFound();
        return Ok(ToResponse(user));
    }

    /// <summary>
    /// Crea un usuario. Si no existe ningún usuario en el sistema, permite crear el primero sin JWT (rol Admin).
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        var hasUsers = await _context.Users.AnyAsync(cancellationToken);
        if (hasUsers && (User.Identity?.IsAuthenticated != true))
            return Unauthorized(new { message = "Requiere autenticación para crear usuarios." });

        if (string.IsNullOrWhiteSpace(request.Username))
            return BadRequest(new { message = "Username es requerido." });
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = "Email es requerido." });
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            return BadRequest(new { message = "La contraseña debe tener al menos 6 caracteres." });

        if (await _context.Users.AnyAsync(u => u.Username == request.Username.Trim(), cancellationToken))
            return Conflict(new { message = "El nombre de usuario ya existe." });
        if (await _context.Users.AnyAsync(u => u.Email == request.Email.Trim(), cancellationToken))
            return Conflict(new { message = "El email ya está registrado." });

        var role = (hasUsers ? request.Role : "Admin")?.Trim() ?? "User";
        if (string.IsNullOrEmpty(role)) role = "User";

        var user = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = role,
            CreatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, ToResponse(user));
    }

    /// <summary>
    /// Actualiza un usuario existente.
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponse>> Update(int id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null)
            return NotFound();

        if (request.Username != null)
        {
            var username = request.Username.Trim();
            if (string.IsNullOrEmpty(username))
                return BadRequest(new { message = "Username no puede estar vacío." });
            if (await _context.Users.AnyAsync(u => u.Username == username && u.Id != id, cancellationToken))
                return Conflict(new { message = "El nombre de usuario ya existe." });
            user.Username = username;
        }
        if (request.Email != null)
        {
            var email = request.Email.Trim();
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { message = "Email no puede estar vacío." });
            if (await _context.Users.AnyAsync(u => u.Email == email && u.Id != id, cancellationToken))
                return Conflict(new { message = "El email ya está registrado." });
            user.Email = email;
        }
        if (!string.IsNullOrEmpty(request.Password))
        {
            if (request.Password.Length < 6)
                return BadRequest(new { message = "La contraseña debe tener al menos 6 caracteres." });
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }
        if (request.Role != null)
            user.Role = request.Role.Trim().Length > 0 ? request.Role.Trim() : user.Role;

        await _context.SaveChangesAsync(cancellationToken);
        return Ok(ToResponse(user));
    }

    /// <summary>
    /// Elimina un usuario.
    /// </summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        if (user == null)
            return NotFound();
        _context.Users.Remove(user);
        await _context.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static UserResponse ToResponse(User u)
    {
        return new UserResponse
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            Role = u.Role,
            CreatedAt = u.CreatedAt
        };
    }
}
