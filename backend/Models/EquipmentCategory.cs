namespace SaraRose.Api.Models;

public class EquipmentCategory
{
    public int Id { get; set; }
    public string Slug { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ShortName { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public int SortOrder { get; set; }

    public ICollection<EquipmentItem> Equipment { get; set; } = new List<EquipmentItem>();
}
