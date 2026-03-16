using Microsoft.AspNetCore.Mvc;
using ProviderOptimizerService.Domain;

namespace ProviderOptimizerService.Controllers;

/// <summary>
/// Controller que expone los catálogos del sistema (tipos de asistencia, etc.).
/// </summary>
[ApiController]
[Route("catalogos")]
public class CatalogsController : ControllerBase
{
    /// <summary>
    /// Obtiene el catálogo de tipos de asistencia.
    /// Retorna un diccionario donde la clave es el identificador numérico
    /// y el valor es el nombre del tipo de asistencia.
    /// </summary>
    /// <returns>Dictionary con los tipos de asistencia (id -> nombre).</returns>
    [HttpGet("tipos-asistencia")]
    public IActionResult GetAssistanceTypes()
    {
        return Ok(Catalogs.TiposAsistencia);
    }
}
