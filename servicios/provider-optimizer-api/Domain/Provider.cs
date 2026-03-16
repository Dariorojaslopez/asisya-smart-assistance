namespace ProviderOptimizerService.Domain;

/// <summary>
/// Representa un proveedor de servicios de asistencia.
/// Este modelo pertenece a la capa de dominio y encapsula
/// la información relevante para la selección y optimización
/// de proveedores.
/// </summary>
public class Provider
{
    /// <summary>
    /// Identificador único del proveedor.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Latitud geográfica del proveedor.
    /// Se utiliza para calcular la distancia con el cliente.
    /// </summary>
    public double Latitud { get; set; }

    /// <summary>
    /// Longitud geográfica del proveedor.
    /// Se utiliza junto con la latitud para calcular la proximidad.
    /// </summary>
    public double Longitud { get; set; }

    /// <summary>
    /// Calificación del proveedor basada en la calidad del servicio.
    /// Se utiliza como parte del algoritmo de optimización.
    /// </summary>
    public double Calificacion { get; set; }

    /// <summary>
    /// Indica si el proveedor está disponible para atender solicitudes.
    /// Solo los proveedores disponibles participan en el proceso de optimización.
    /// </summary>
    public bool Disponible { get; set; }
}
