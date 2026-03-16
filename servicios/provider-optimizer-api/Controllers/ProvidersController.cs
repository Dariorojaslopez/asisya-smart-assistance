using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderOptimizerService.Domain;
using ProviderOptimizerService.Infrastructure;

namespace ProviderOptimizerService.Controllers;

/// <summary>
/// Controller que expone los proveedores del sistema (disponibles, listado, etc.).
/// </summary>
[ApiController]
[Route("providers")]
[Authorize]
public class ProvidersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Inicializa el controller con el contexto de base de datos.
    /// </summary>
    public ProvidersController(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene la lista de proveedores disponibles en el sistema.
    /// Solo se incluyen proveedores con Disponible == true.
    /// </summary>
    /// <returns>200 OK con la lista de proveedores disponibles (List&lt;Provider&gt;).</returns>
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableProviders()
    {
        var providers = await _context.Providers
            .Where(p => p.Disponible)
            .ToListAsync();

        return Ok(providers);
    }
}
