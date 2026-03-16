namespace ProviderOptimizerService.DTOs;

/// <summary>
/// DTO para la actualización parcial de un usuario.
/// </summary>
public class UpdateUserRequest
{
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string? Role { get; set; }
}
