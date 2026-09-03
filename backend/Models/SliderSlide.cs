namespace SaraRose.Api.Models;

public class SliderSlide
{
    public int Id { get; set; }
    public int SortOrder { get; set; }
    public string Alt { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "image/jpeg";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
