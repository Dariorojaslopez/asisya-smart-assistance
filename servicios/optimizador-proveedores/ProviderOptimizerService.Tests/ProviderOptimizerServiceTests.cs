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
    public void Should_return_the_closest_provider_when_multiple_providers_exist()
    {
        // Arrange
        var proveedores = CreateSampleProviders();
        var service = new global::ProviderOptimizerService.Services.ProviderOptimizerService();
        double latCliente = 4.65;
        double lngCliente = -74.05;

        // Act
        var result = service.GetBestProvider(proveedores, latCliente, lngCliente);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("P1", result.Id);
        Assert.Equal(4.65, result.Latitud);
        Assert.Equal(-74.05, result.Longitud);
    }

    [Fact]
    public void Should_return_null_when_no_providers_are_available()
    {
        // Arrange
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

    [Fact]
    public void Should_select_provider_with_highest_rating_when_distances_are_equal()
    {
        // Arrange: two available providers at the same location (same distance to client)
        var proveedores = new List<Provider>
        {
            new Provider
            {
                Id = "LowRating",
                Latitud = 4.65,
                Longitud = -74.05,
                Calificacion = 3.0,
                Disponible = true
            },
            new Provider
            {
                Id = "HighRating",
                Latitud = 4.65,
                Longitud = -74.05,
                Calificacion = 5.0,
                Disponible = true
            }
        };
        var service = new global::ProviderOptimizerService.Services.ProviderOptimizerService();
        double latCliente = 4.65;
        double lngCliente = -74.05;

        // Act
        var result = service.GetBestProvider(proveedores, latCliente, lngCliente);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("HighRating", result.Id);
        Assert.Equal(5.0, result.Calificacion);
    }
}
