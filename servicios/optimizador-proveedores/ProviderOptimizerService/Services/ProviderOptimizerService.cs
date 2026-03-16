using ProviderOptimizerService.Domain;

namespace ProviderOptimizerService.Services;

/// <summary>
/// Servicio que selecciona el mejor proveedor de asistencia según
/// disponibilidad, distancia al cliente (Haversine) y calificación.
/// </summary>
public class ProviderOptimizerService
{
    /// <summary>
    /// Obtiene el mejor proveedor disponible: filtra por Disponible == true,
    /// ordena por menor distancia al cliente y, a igual distancia, por mayor calificación.
    /// </summary>
    /// <param name="proveedores">Lista de proveedores a evaluar.</param>
    /// <param name="latCliente">Latitud del cliente.</param>
    /// <param name="lngCliente">Longitud del cliente.</param>
    /// <returns>El mejor proveedor o null si no hay ninguno disponible.</returns>
    public Provider? GetBestProvider(List<Provider> proveedores, double latCliente, double lngCliente)
    {
        // 1. Filtrar solo proveedores disponibles
        var proveedoresDisponibles = proveedores.Where(p => p.Disponible).ToList();
        if (proveedoresDisponibles.Count == 0)
            return null;

        // 2. Calcular distancia al cliente y ordenar por distancia ascendente,
        //    luego por calificación descendente (a igual distancia, mejor rating)
        var mejorProveedor = proveedoresDisponibles
            .Select(p => new
            {
                Proveedor = p,
                Distancia = CalculateDistance(latCliente, lngCliente, p.Latitud, p.Longitud)
            })
            .OrderBy(x => x.Distancia)
            .ThenByDescending(x => x.Proveedor.Calificacion)
            .FirstOrDefault();

        return mejorProveedor?.Proveedor;
    }

    /// <summary>
    /// Calcula la distancia en kilómetros entre dos puntos geográficos
    /// usando la fórmula de Haversine.
    /// </summary>
    /// <param name="lat1">Latitud del primer punto.</param>
    /// <param name="lon1">Longitud del primer punto.</param>
    /// <param name="lat2">Latitud del segundo punto.</param>
    /// <param name="lon2">Longitud del segundo punto.</param>
    /// <returns>Distancia en kilómetros.</returns>
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double radioTierra = 6371; // km

        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;

        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(lat1 * Math.PI / 180) *
            Math.Cos(lat2 * Math.PI / 180) *
            Math.Sin(dLon / 2) *
            Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return radioTierra * c;
    }
}
