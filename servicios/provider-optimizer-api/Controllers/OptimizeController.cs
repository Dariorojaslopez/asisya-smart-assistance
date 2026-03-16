using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderOptimizerService.DTOs;
using ProviderOptimizerService.Domain;
using ProviderOptimizerService.Infrastructure;
using ProviderOptimizerService.Services;

namespace ProviderOptimizerService.Controllers;

/// <summary>
/// Controller encargado de optimizar la selección de proveedores
/// de asistencia basado en ubicación del cliente y disponibilidad.
/// </summary>
[ApiController]
[Route("optimize")]
[Authorize]
public class OptimizeController : ControllerBase
{
    private readonly global::ProviderOptimizerService.Services.ProviderOptimizerService _optimizerService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<OptimizeController> _logger;

    /// <summary>
    /// Inicializa el controller con el servicio de optimización, el contexto de base de datos y el logger.
    /// </summary>
    public OptimizeController(
        global::ProviderOptimizerService.Services.ProviderOptimizerService optimizerService,
        ApplicationDbContext context,
        ILogger<OptimizeController> logger)
    {
        _optimizerService = optimizerService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Recibe ubicación (latitude, longitude) y tipo de asistencia (código).
    /// Devuelve lista de proveedores ordenados por distancia con ETA calculada (minutos).
    /// ETA se estima a ~30 km/h desde la distancia al cliente.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Optimize([FromBody] OptimizeRequestApi? request)
    {
        _logger.LogInformation("Solicitud de optimización recibida.");

        if (request == null || string.IsNullOrWhiteSpace(request.AssistanceType))
        {
            _logger.LogWarning("Solicitud de optimización rechazada: request nulo o sin tipo.");
            return BadRequest("La solicitud es inválida. Incluya latitude, longitude y assistanceType.");
        }

        if (!Catalogs.AssistanceTypeCodeToId.TryGetValue(request.AssistanceType.Trim(), out var tipoId))
        {
            _logger.LogWarning("Tipo de asistencia inválido: {AssistanceType}", request.AssistanceType);
            return BadRequest($"Tipo de asistencia inválido: {request.AssistanceType}. Consulte GET /catalogs/assistance-types.");
        }

        var proveedores = await _context.Providers
            .Where(p => p.Disponible)
            .ToListAsync();

        var ordered = _optimizerService.GetOrderedProvidersWithDistance(
            proveedores,
            request.Latitude,
            request.Longitude
        );

        // ETA en minutos: supuesto 30 km/h -> 2 min por km
        const double minutesPerKm = 2.0;
        var result = ordered.Select(x =>
        {
            var etaMinutes = (int)Math.Max(1, Math.Round(x.DistanceKm * minutesPerKm));
            return new OptimizeProviderItem
            {
                Id = x.Provider.Id,
                Latitud = x.Provider.Latitud,
                Longitud = x.Provider.Longitud,
                Calificacion = x.Provider.Calificacion,
                EtaMinutes = etaMinutes
            };
        }).ToList();

        _logger.LogInformation("Optimización: {Count} proveedores devueltos.", result.Count);
        return Ok(result);
    }
}
