namespace SaraRose.Api.Models;

public class EquipmentItem
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string MachineType { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TypicalUse { get; set; } = string.Empty;
    public string AvailabilityNote { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public EquipmentCategory? Category { get; set; }
}
