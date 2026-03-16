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
    /// Recibe la ubicación del cliente y el tipo de asistencia y retorna
    /// el proveedor óptimo (disponible, más cercano y con mejor calificación).
    /// </summary>
    /// <param name="request">Datos de ubicación (Lat, Lng) y TipoAsistencia. No puede ser null.</param>
    /// <returns>200 con el proveedor seleccionado, 400 si el tipo de asistencia no es válido, 404 si no hay proveedor disponible.</returns>
    [HttpPost]
    public async Task<IActionResult> Optimize([FromBody] OptimizeRequest? request)
    {
        _logger.LogInformation("Solicitud de optimización recibida.");

        if (request == null)
        {
            _logger.LogWarning("Solicitud de optimización rechazada: request nulo.");
            return BadRequest("La solicitud es inválida.");
        }

        if (!Catalogs.TiposAsistencia.ContainsKey(request.TipoAsistencia))
        {
            _logger.LogWarning("Tipo de asistencia inválido: {TipoAsistencia}", request.TipoAsistencia);
            return BadRequest($"Tipo de asistencia inválido: {request.TipoAsistencia}. Consulte GET /catalogos/tipos-asistencia para valores válidos.");
        }

        var proveedores = await _context.Providers
            .Where(p => p.Disponible)
            .ToListAsync();

        var mejorProveedor = _optimizerService.GetBestProvider(
            proveedores,
            request.Lat,
            request.Lng
        );

        if (mejorProveedor == null)
        {
            _logger.LogWarning("No se encontró ningún proveedor disponible para la solicitud.");
            return NotFound("No se encontró un proveedor disponible.");
        }

        _logger.LogInformation("Proveedor seleccionado: {ProveedorId} (Calificación: {Calificacion})", mejorProveedor.Id, mejorProveedor.Calificacion);
        return Ok(mejorProveedor);
    }
}
