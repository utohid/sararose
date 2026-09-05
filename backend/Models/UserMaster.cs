using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaraRose.Api.Models;

[Table("userMaster")]
public class UserMaster
{
    public int Id { get; set; }

    [MaxLength(80)]
    public string Username { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(40)]
    public string Phone { get; set; } = string.Empty;

    [MaxLength(32)]
    public string Role { get; set; } = "User";

    [MaxLength(32)]
    public string UserType { get; set; } = "Customer";

    [MaxLength(128)]
    public string HashPassword { get; set; } = string.Empty;

    [MaxLength(200)]
    public string NormalPassword { get; set; } = string.Empty;

    public int? RegistrationId { get; set; }

    public bool Active { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
