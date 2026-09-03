namespace SaraRose.Api.Models;

public class HeaderLink
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public bool Visible { get; set; } = true;
    public bool IsCta { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
