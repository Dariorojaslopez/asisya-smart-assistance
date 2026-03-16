using Microsoft.EntityFrameworkCore;
using ProviderOptimizerService.Domain;

namespace ProviderOptimizerService.Infrastructure;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Provider> Providers { get; set; }

    /// <summary>
    /// Usuarios del sistema. La tabla Users se crea con la migración AddUsersTable
    /// (columnas: Id, Username, Email, PasswordHash, Role, CreatedAt).
    /// </summary>
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de la entidad User y tabla Users para Entity Framework.
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.HasKey(u => u.Id);
            e.Property(u => u.Username).IsRequired().HasMaxLength(100);
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Role).IsRequired().HasMaxLength(50);
            e.Property(u => u.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Provider>().HasData(
            new Provider
            {
                Id = "P1",
                Latitud = 4.65,
                Longitud = -74.05,
                Calificacion = 4.5,
                Disponible = true
            },
            new Provider
            {
                Id = "P2",
                Latitud = 4.66,
                Longitud = -74.04,
                Calificacion = 4.8,
                Disponible = true
            },
            new Provider
            {
                Id = "P3",
                Latitud = 4.64,
                Longitud = -74.06,
                Calificacion = 4.2,
                Disponible = false
            }
        );
    }
}
