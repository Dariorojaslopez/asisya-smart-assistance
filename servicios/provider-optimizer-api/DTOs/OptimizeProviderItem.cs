namespace ProviderOptimizerService.DTOs;

/// <summary>
/// Proveedor devuelto por la optimización con ETA calculada.
/// </summary>
public class OptimizeProviderItem
{
    public string Id { get; set; } = string.Empty;
    public double Latitud { get; set; }
    public double Longitud { get; set; }
    public double Calificacion { get; set; }
    public int EtaMinutes { get; set; }
}
