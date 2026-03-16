using ProviderOptimizerService.Domain;
using ProviderOptimizerService.Services;
using Xunit;

namespace ProviderOptimizerService.Tests;

public class ProviderOptimizerServiceTests
{
    private static List<Provider> CreateSampleProviders()
    {
        return new List<Provider>
        {
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
        };
    }

    [Fact]
    public void Should_return_best_provider_based_on_rating_and_distance()
    {
        // Arrange: P1 is closest to client (4.65, -74.05); P2 is slightly farther but has higher rating
        var proveedores = CreateSampleProviders();
        var service = new global::ProviderOptimizerService.Services.ProviderOptimizerService();
        double latCliente = 4.65;
        double lngCliente = -74.05;

        // Act
        var result = service.GetBestProvider(proveedores, latCliente, lngCliente);

        // Assert: closest available provider wins (P1 same location as client)
        Assert.NotNull(result);
        Assert.Equal("P1", result.Id);
        Assert.True(result.Disponible);
        Assert.True(result.Calificacion >= 0);
    }

    [Fact]
    public void Should_ignore_providers_that_are_not_available()
    {
        // Arrange: P3 has best location but Disponible = false; only P1 and P2 are available
        var proveedores = CreateSampleProviders();
        var service = new global::ProviderOptimizerService.Services.ProviderOptimizerService();
        double latCliente = 4.64;
        double lngCliente = -74.06;

        // Act: client near P3 (unavailable) — result must be P1 or P2, never P3
        var result = service.GetBestProvider(proveedores, latCliente, lngCliente);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual("P3", result.Id);
        Assert.True(result.Disponible);
        Assert.True(result.Id == "P1" || result.Id == "P2");
    }

    [Fact]
    public void Should_return_null_when_no_providers_are_available()
    {
        // Arrange: all providers marked unavailable
        var proveedores = CreateSampleProviders();
        foreach (var p in proveedores)
            p.Disponible = false;
        var service = new global::ProviderOptimizerService.Services.ProviderOptimizerService();
        double latCliente = 4.65;
        double lngCliente = -74.05;

        // Act
        var result = service.GetBestProvider(proveedores, latCliente, lngCliente);

        // Assert
        Assert.Null(result);
    }
}
