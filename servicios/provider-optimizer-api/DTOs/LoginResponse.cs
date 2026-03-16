namespace ProviderOptimizerService.DTOs;

/// <summary>
/// Respuesta del login con el JWT generado.
/// </summary>
public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
}
