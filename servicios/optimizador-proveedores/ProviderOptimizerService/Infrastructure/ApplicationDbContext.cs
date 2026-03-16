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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
