using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Data;
using SaraRose.Api.DTOs;

namespace SaraRose.Api.Controllers;

[ApiController]
[Route("api")]
public class EquipmentController(AppDbContext db) : ControllerBase
{
    [HttpGet("categories")]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        var items = await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoryDto(
                c.Id,
                c.Slug,
                c.Code,
                c.Name,
                c.ShortName,
                c.Summary,
                c.Equipment.Count))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("equipment")]
    public async Task<ActionResult<IEnumerable<EquipmentSummaryDto>>> GetEquipment(
        [FromQuery] string? category,
        CancellationToken cancellationToken)
    {
        var query = db.Equipment.AsNoTracking().Include(e => e.Category).AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(e => e.Category!.Slug == category);
        }

        var items = await query
            .OrderBy(e => e.SortOrder)
            .Select(e => new EquipmentSummaryDto(
                e.Id,
                e.Slug,
                e.Name,
                e.MachineType,
                e.Summary,
                e.Category!.Slug,
                e.Category.Name))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("equipment/{slug}")]
    public async Task<ActionResult<EquipmentDetailDto>> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var item = await db.Equipment
            .AsNoTracking()
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Slug == slug, cancellationToken);

        if (item is null || item.Category is null)
        {
            return NotFound();
        }

        var category = item.Category;
        return Ok(new EquipmentDetailDto(
            item.Id,
            item.Slug,
            item.Name,
            item.MachineType,
            item.Summary,
            item.Description,
            item.TypicalUse,
            item.AvailabilityNote,
            new CategoryDto(
                category.Id,
                category.Slug,
                category.Code,
                category.Name,
                category.ShortName,
                category.Summary,
                0)));
    }
}
