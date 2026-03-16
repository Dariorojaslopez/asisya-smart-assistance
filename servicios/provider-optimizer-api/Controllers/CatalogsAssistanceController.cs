using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProviderOptimizerService.Domain;
using ProviderOptimizerService.DTOs;

namespace ProviderOptimizerService.Controllers;

/// <summary>
/// Expone el catálogo de tipos de asistencia en formato code/name para la API.
/// </summary>
[ApiController]
[Route("catalogs")]
[Authorize]
public class CatalogsAssistanceController : ControllerBase
{
    /// <summary>
    /// Obtiene el catálogo de tipos de asistencia (code, name) para dropdowns.
    /// </summary>
    [HttpGet("assistance-types")]
    public IActionResult GetAssistanceTypes()
    {
        var items = Catalogs.AssistanceTypeCodeToId
            .OrderBy(x => x.Value)
            .Select(x => new AssistanceTypeItem
            {
                Code = x.Key,
                Name = Catalogs.TiposAsistencia[x.Value]
            })
            .ToList();
        return Ok(items);
    }
}
