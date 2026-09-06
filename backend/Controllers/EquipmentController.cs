using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Data;
using SaraRose.Api.DTOs;
using SaraRose.Api.Models;

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
                c.Equipment.Count,
                c.SortOrder))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("categories/{id:int}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id, CancellationToken cancellationToken)
    {
        var row = await db.Categories
            .AsNoTracking()
            .Include(c => c.Equipment)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return row is null ? NotFound() : Ok(ToCategoryDto(row, row.Equipment.Count));
    }

    [HttpPost("categories")]
    public async Task<ActionResult<CategoryDto>> CreateCategory(
        [FromBody] CategoryRequest request,
        CancellationToken cancellationToken)
    {
        var name = CatalogText.Required(request.Name, 160);
        if (name.Length == 0)
        {
            return BadRequest(new { message = "Enter an equipment group name." });
        }

        var sortOrder = request.SortOrder
            ?? (await db.Categories.Select(x => (int?)x.SortOrder).MaxAsync(cancellationToken) ?? 0) + 1;
        var slug = await UniqueCategorySlug(request.Slug, name, null, cancellationToken);
        var shortName = CatalogText.Required(request.ShortName, 80, name);
        var row = new EquipmentCategory
        {
            Name = name,
            ShortName = shortName,
            Slug = slug,
            Code = CatalogText.Code(request.Code, sortOrder),
            Summary = CatalogText.Required(request.Summary, 800, name),
            SortOrder = sortOrder
        };

        db.Categories.Add(row);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetCategory), new { id = row.Id }, ToCategoryDto(row, 0));
    }

    [HttpPut("categories/{id:int}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(
        int id,
        [FromBody] CategoryRequest request,
        CancellationToken cancellationToken)
    {
        var row = await db.Categories.Include(c => c.Equipment).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (row is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            row.Name = CatalogText.Required(request.Name, 160);
        }

        if (request.ShortName is not null)
        {
            row.ShortName = CatalogText.Required(request.ShortName, 80, row.Name);
        }

        if (request.Summary is not null)
        {
            row.Summary = CatalogText.Required(request.Summary, 800, row.Name);
        }

        if (request.Code is not null)
        {
            row.Code = CatalogText.Code(request.Code, row.SortOrder);
        }

        if (request.SortOrder is int order)
        {
            row.SortOrder = order;
        }

        if (request.Slug is not null || !string.IsNullOrWhiteSpace(request.Name))
        {
            row.Slug = await UniqueCategorySlug(request.Slug, row.Name, row.Id, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
        return Ok(ToCategoryDto(row, row.Equipment.Count));
    }

    [HttpDelete("categories/{id:int}")]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
    {
        var row = await db.Categories.Include(c => c.Equipment).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (row is null)
        {
            return NotFound();
        }

        if (row.Equipment.Count > 0)
        {
            return Conflict(new { message = "Remove or move the machine types in this equipment group first." });
        }

        db.Categories.Remove(row);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
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
                e.Category.Name,
                e.CategoryId,
                e.SortOrder))
            .ToListAsync(cancellationToken);

        return Ok(items);
    }

    [HttpGet("equipment/item/{id:int}")]
    public async Task<ActionResult<EquipmentDetailDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var item = await db.Equipment
            .AsNoTracking()
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        return item is null || item.Category is null ? NotFound() : Ok(ToDetail(item));
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

        return Ok(ToDetail(item));
    }

    [HttpPost("equipment")]
    public async Task<ActionResult<EquipmentDetailDto>> CreateEquipment(
        [FromBody] EquipmentRequest request,
        CancellationToken cancellationToken)
    {
        var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);
        if (category is null)
        {
            return BadRequest(new { message = "Choose an equipment group." });
        }

        var name = CatalogText.Required(request.Name, 160);
        if (name.Length == 0)
        {
            return BadRequest(new { message = "Enter a machine type name." });
        }

        var sortOrder = request.SortOrder
            ?? (await db.Equipment.Select(x => (int?)x.SortOrder).MaxAsync(cancellationToken) ?? 0) + 1;
        var machineType = CatalogText.Required(request.MachineType, 80, name);
        var row = new EquipmentItem
        {
            CategoryId = category.Id,
            Name = name,
            MachineType = machineType,
            Slug = await UniqueEquipmentSlug(request.Slug, name, null, cancellationToken),
            Summary = CatalogText.Required(request.Summary, 400, name),
            Description = string.IsNullOrWhiteSpace(request.Description) ? name : request.Description.Trim(),
            TypicalUse = CatalogText.Required(request.TypicalUse, 400),
            AvailabilityNote = CatalogText.Required(request.AvailabilityNote, 400, CatalogText.DefaultAvailability),
            SortOrder = sortOrder
        };

        db.Equipment.Add(row);
        await db.SaveChangesAsync(cancellationToken);
        row.Category = category;
        return CreatedAtAction(nameof(GetById), new { id = row.Id }, ToDetail(row));
    }

    [HttpPut("equipment/{id:int}")]
    public async Task<ActionResult<EquipmentDetailDto>> UpdateEquipment(
        int id,
        [FromBody] EquipmentRequest request,
        CancellationToken cancellationToken)
    {
        var row = await db.Equipment.Include(e => e.Category).FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (row is null)
        {
            return NotFound();
        }

        if (request.CategoryId > 0 && request.CategoryId != row.CategoryId)
        {
            var category = await db.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);
            if (category is null)
            {
                return BadRequest(new { message = "Choose an equipment group." });
            }

            row.CategoryId = category.Id;
            row.Category = category;
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            row.Name = CatalogText.Required(request.Name, 160);
        }

        if (request.MachineType is not null)
        {
            row.MachineType = CatalogText.Required(request.MachineType, 80, row.Name);
        }

        if (request.Summary is not null)
        {
            row.Summary = CatalogText.Required(request.Summary, 400, row.Name);
        }

        if (request.Description is not null)
        {
            row.Description = string.IsNullOrWhiteSpace(request.Description) ? row.Name : request.Description.Trim();
        }

        if (request.TypicalUse is not null)
        {
            row.TypicalUse = CatalogText.Required(request.TypicalUse, 400);
        }

        if (request.AvailabilityNote is not null)
        {
            row.AvailabilityNote = CatalogText.Required(request.AvailabilityNote, 400, CatalogText.DefaultAvailability);
        }

        if (request.SortOrder is int order)
        {
            row.SortOrder = order;
        }

        if (request.Slug is not null || !string.IsNullOrWhiteSpace(request.Name))
        {
            row.Slug = await UniqueEquipmentSlug(request.Slug, row.Name, row.Id, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
        if (row.Category is null)
        {
            row.Category = await db.Categories.FirstAsync(c => c.Id == row.CategoryId, cancellationToken);
        }

        return Ok(ToDetail(row));
    }

    [HttpDelete("equipment/{id:int}")]
    public async Task<IActionResult> DeleteEquipment(int id, CancellationToken cancellationToken)
    {
        var row = await db.Equipment.FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
        if (row is null)
        {
            return NotFound();
        }

        db.Equipment.Remove(row);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private async Task<string> UniqueCategorySlug(string? requested, string name, int? exceptId, CancellationToken cancellationToken)
    {
        var baseSlug = CatalogText.Slug(string.IsNullOrWhiteSpace(requested) ? name : requested);
        return await UniqueSlug(baseSlug, exceptId, (slug, id) =>
            db.Categories.AnyAsync(x => x.Slug == slug && (id == null || x.Id != id), cancellationToken));
    }

    private async Task<string> UniqueEquipmentSlug(string? requested, string name, int? exceptId, CancellationToken cancellationToken)
    {
        var baseSlug = CatalogText.Slug(string.IsNullOrWhiteSpace(requested) ? name : requested);
        return await UniqueSlug(baseSlug, exceptId, (slug, id) =>
            db.Equipment.AnyAsync(x => x.Slug == slug && (id == null || x.Id != id), cancellationToken));
    }

    private static async Task<string> UniqueSlug(string baseSlug, int? exceptId, Func<string, int?, Task<bool>> taken)
    {
        var slug = baseSlug;
        var n = 2;
        while (await taken(slug, exceptId))
        {
            slug = $"{baseSlug}-{n++}";
        }

        return slug;
    }

    private static CategoryDto ToCategoryDto(EquipmentCategory row, int count) =>
        new(row.Id, row.Slug, row.Code, row.Name, row.ShortName, row.Summary, count, row.SortOrder);

    private static EquipmentDetailDto ToDetail(EquipmentItem item)
    {
        var category = item.Category!;
        return new EquipmentDetailDto(
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
                0,
                category.SortOrder));
    }
}
