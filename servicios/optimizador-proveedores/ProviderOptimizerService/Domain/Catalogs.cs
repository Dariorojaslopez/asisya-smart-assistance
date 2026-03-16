namespace ProviderOptimizerService.Domain;

/// <summary>
/// Catálogos utilizados por el sistema.
/// Se centralizan aquí para evitar duplicación de datos.
/// </summary>
public static class Catalogs
{
    /// <summary>
    /// Catálogo de tipos de asistencia disponibles.
    /// </summary>
    public static readonly Dictionary<int, string> TiposAsistencia = new()
    {
        {1, "Grua"},
        {2, "Bateria"},
        {3, "Combustible"},
        {4, "Cerrajeria"},
        {5, "Cambio de llanta"},
        {6, "Mecanica ligera"}
    };
}
