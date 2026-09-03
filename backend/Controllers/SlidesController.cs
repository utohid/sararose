using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Data;
using SaraRose.Api.DTOs;
using SaraRose.Api.Models;

namespace SaraRose.Api.Controllers;

[ApiController]
[Route("api/slides")]
public class SlidesController(AppDbContext db, IWebHostEnvironment env) : ControllerBase
{
    private static readonly HashSet<string> AllowedTypes =
    [
        "image/jpeg",
        "image/jpg",
        "image/png",
        "image/webp",
        "image/gif"
    ];

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SliderSlideDto>>> List(CancellationToken cancellationToken)
    {
        var rows = await db.SliderSlides
            .AsNoTracking()
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.Id)
            .ToListAsync(cancellationToken);

        return Ok(rows.Select(ToDto));
    }

    [HttpPost]
    [RequestSizeLimit(8_000_000)]
    public async Task<ActionResult<SliderSlideDto>> Upload(
        [FromForm] IFormFile file,
        [FromForm] string? alt,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { message = "Choose an image file." });
        }

        if (file.Length > 8_000_000)
        {
            return BadRequest(new { message = "Image must be 8 MB or smaller." });
        }

        var contentType = (file.ContentType ?? string.Empty).ToLowerInvariant();
        if (!AllowedTypes.Contains(contentType))
        {
            return BadRequest(new { message = "Use a JPG, PNG, WEBP or GIF image." });
        }

        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrWhiteSpace(ext) || ext.Length > 8)
        {
            ext = contentType.Contains("png") ? ".png" : contentType.Contains("webp") ? ".webp" : contentType.Contains("gif") ? ".gif" : ".jpg";
        }

        var folder = Path.Combine(env.ContentRootPath, "wwwroot", "uploads", "slides");
        Directory.CreateDirectory(folder);
        var storedName = $"{Guid.NewGuid():N}{ext.ToLowerInvariant()}";
        var path = Path.Combine(folder, storedName);

        await using (var stream = System.IO.File.Create(path))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        var nextOrder = await db.SliderSlides.Select(s => (int?)s.SortOrder).MaxAsync(cancellationToken) ?? 0;
        var slide = new SliderSlide
        {
            SortOrder = nextOrder + 1,
            Alt = string.IsNullOrWhiteSpace(alt) ? Path.GetFileNameWithoutExtension(file.FileName) : alt.Trim(),
            FileName = storedName,
            ContentType = contentType,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.SliderSlides.Add(slide);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(List), ToDto(slide));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<SliderSlideDto>> Update(
        int id,
        [FromBody] SliderUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var slide = await db.SliderSlides.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (slide is null)
        {
            return NotFound();
        }

        if (request.Alt is not null)
        {
            slide.Alt = request.Alt.Trim();
        }

        if (request.SortOrder is int order)
        {
            slide.SortOrder = order;
        }

        await db.SaveChangesAsync(cancellationToken);
        return Ok(ToDto(slide));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var slide = await db.SliderSlides.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (slide is null)
        {
            return NotFound();
        }

        var path = Path.Combine(env.ContentRootPath, "wwwroot", "uploads", "slides", slide.FileName);
        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        db.SliderSlides.Remove(slide);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static SliderSlideDto ToDto(SliderSlide slide) =>
        new(slide.Id, slide.SortOrder, slide.Alt, $"/uploads/slides/{slide.FileName}", slide.CreatedAtUtc);
}

public class SliderUpdateRequest
{
    public string? Alt { get; set; }
    public int? SortOrder { get; set; }
}
