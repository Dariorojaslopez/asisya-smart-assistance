using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ProviderOptimizerService.Infrastructure;
using ProviderOptimizerService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddScoped<global::ProviderOptimizerService.Services.ProviderOptimizerService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Provider Optimizer Service",
        Version = "v1"
    });
});

var app = builder.Build();

// Aplicar migraciones pendientes al arranque (permite que Docker funcione sin ejecutar migraciones a mano)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>
/// Entry point exposed for integration tests (WebApplicationFactory).
/// </summary>
public partial class Program { }
