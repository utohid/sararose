namespace SaraRose.Api.Models;

public class UserRegistration
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string? City { get; set; }
    public string Role { get; set; } = "User";
    public string UserType { get; set; } = "Customer";
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
