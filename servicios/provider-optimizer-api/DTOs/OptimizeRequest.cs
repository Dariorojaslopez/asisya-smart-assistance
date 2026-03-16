namespace ProviderOptimizerService.DTOs;

/// <summary>
/// DTO que representa la solicitud enviada por el cliente
/// para optimizar la asignación de un proveedor de asistencia.
/// Contiene la ubicación del cliente y el tipo de servicio requerido.
/// </summary>
public class OptimizeRequest
{
    /// <summary>
    /// Latitud geográfica del cliente que solicita la asistencia.
    /// Se utiliza para calcular la distancia (Haversine) entre el cliente y los proveedores disponibles.
    /// </summary>
    public double Lat { get; set; }

    /// <summary>
    /// Longitud geográfica del cliente que solicita la asistencia.
    /// Junto con Lat se utiliza para calcular la distancia a cada proveedor.
    /// </summary>
    public double Lng { get; set; }

    /// <summary>
    /// Identificador del tipo de asistencia solicitada.
    /// Los valores válidos se obtienen del endpoint GET /catalogos/tipos-asistencia.
    /// Ejemplos: 1=Grua, 2=Bateria, 3=Combustible, 4=Cerrajeria, 5=Cambio de llanta, 6=Mecanica ligera.
    /// </summary>
    public int TipoAsistencia { get; set; }
}
