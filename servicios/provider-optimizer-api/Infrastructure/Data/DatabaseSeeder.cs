using Microsoft.EntityFrameworkCore;
using ProviderOptimizerService.Domain;

namespace ProviderOptimizerService.Infrastructure.Data;

/// <summary>
/// Seeder de base de datos. Crea el usuario administrador por defecto
/// únicamente cuando la tabla Users está vacía (primera ejecución o entorno nuevo).
/// Las credenciales por defecto no están en controladores ni servicios, solo aquí.
/// </summary>
public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;

    public DatabaseSeeder(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Ejecuta el seed: si no hay usuarios, inserta el administrador por defecto
    /// con contraseña hasheada con BCrypt. Idempotente y seguro para Docker/PostgreSQL.
    /// </summary>
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Comprobar si ya existe al menos un usuario (evita duplicados en reinicios).
        var hasUsers = await _context.Users.AnyAsync(cancellationToken);
        if (hasUsers)
            return;

        // Crear el único usuario inicial: administrador por defecto.
        // La contraseña se almacena hasheada con BCrypt, nunca en claro.
        var defaultAdmin = new User
        {
            Username = "admin",
            Email = "admin@system.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"),
            Role = "Admin",
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(defaultAdmin);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
