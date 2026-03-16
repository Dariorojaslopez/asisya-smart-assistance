namespace ProviderOptimizerService.Domain;

/// <summary>
/// Entidad de usuario para autenticación y gestión.
/// Persistida en la tabla Users mediante Entity Framework.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
