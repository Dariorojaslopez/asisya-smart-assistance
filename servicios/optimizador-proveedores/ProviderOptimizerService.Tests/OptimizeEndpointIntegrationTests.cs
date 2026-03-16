using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using ProviderOptimizerService.Domain;
using Xunit;

namespace ProviderOptimizerService.Tests;

public class OptimizeEndpointIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OptimizeEndpointIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
        }).CreateClient();
    }

    [Fact]
    public async Task PostOptimize_Returns200_And_Provider_P1_or_P2()
    {
        // Arrange
        var request = new
        {
            lat = 4.65,
            lng = -74.05,
            tipoAsistencia = 1
        };

        // Act
        var response = await _client.PostAsJsonAsync("/optimize", request);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var provider = await response.Content.ReadFromJsonAsync<Provider>();
        Assert.NotNull(provider);
        Assert.NotNull(provider.Id);
        Assert.True(provider.Disponible);
        // Verify response returns provider P1 or P2 (seeded available providers)
        Assert.True(provider.Id == "P1" || provider.Id == "P2", $"Expected P1 or P2 but got {provider.Id}");
    }
}
