namespace SaraRose.Api.Models;

public class Enquiry
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Company { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? MachineType { get; set; }
    public string? SiteLocation { get; set; }
    public string Requirement { get; set; } = string.Empty;
    public string Status { get; set; } = "New";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public EquipmentCategory? Category { get; set; }
}
