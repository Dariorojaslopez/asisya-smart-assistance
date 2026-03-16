using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProviderOptimizerService.Infrastructure;
using ProviderOptimizerService.Infrastructure.Data;
using ProviderOptimizerService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services. JSON en camelCase para consistencia con el frontend.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddScoped<global::ProviderOptimizerService.Services.ProviderOptimizerService>();
builder.Services.AddScoped<DatabaseSeeder>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var jwtSecret = builder.Configuration["JWT_SECRET"]
    ?? (builder.Environment.IsDevelopment() ? "dev-secret-key-min-32-chars-long-for-jwt" : null)
    ?? throw new InvalidOperationException("JWT_SECRET must be set.");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });

// CORS: el frontend React (http://localhost:3000) y la API (http://localhost:8080) tienen orígenes distintos.
// El navegador bloquea peticiones cross-origin por defecto; esta política permite que el frontend consuma la API.
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

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

// Al arrancar la aplicación (incluido en Docker) se aplican las migraciones pendientes.
// Migrate() actualiza la base de datos con la tabla Users (Id, Username, Email, PasswordHash, Role, CreatedAt)
// y el resto de tablas definidas en el modelo, sin intervención manual.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();

    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync();
}

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// CORS debe ejecutarse antes de autenticación y autorización para que las preflight (OPTIONS) respondan correctamente.
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

/// <summary>
/// Entry point exposed for integration tests (WebApplicationFactory).
/// </summary>
public partial class Program { }
