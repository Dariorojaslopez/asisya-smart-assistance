namespace ProviderOptimizerService.DTOs;

/// <summary>
/// Solicitud de optimización desde la API (latitude, longitude, assistanceType string).
/// </summary>
public class OptimizeRequestApi
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string AssistanceType { get; set; } = string.Empty;
}
