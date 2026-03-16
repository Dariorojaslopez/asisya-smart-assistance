namespace ProviderOptimizerService.Domain;

/// <summary>
/// Catálogos utilizados por el sistema.
/// Se centralizan aquí para evitar duplicación de datos.
/// </summary>
public static class Catalogs
{
    /// <summary>
    /// Catálogo de tipos de asistencia disponibles (id -> nombre).
    /// </summary>
    public static readonly Dictionary<int, string> TiposAsistencia = new()
    {
        { 1, "Grúa" },
        { 2, "Paso de corriente" },
        { 3, "Combustible" },
        { 4, "Cerrajería" },
        { 5, "Cambio de llanta" },
        { 6, "Mecánica ligera" }
    };

    /// <summary>
    /// Códigos de tipo de asistencia para la API (código -> id).
    /// </summary>
    public static readonly Dictionary<string, int> AssistanceTypeCodeToId = new(StringComparer.OrdinalIgnoreCase)
    {
        { "GRUA", 1 },
        { "BATERIA", 2 },
        { "COMBUSTIBLE", 3 },
        { "CERRAJERIA", 4 },
        { "LLANTA", 5 },
        { "MECANICA", 6 }
    };
}
