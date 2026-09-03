using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Data;
using SaraRose.Api.DTOs;
using SaraRose.Api.Models;

namespace SaraRose.Api.Controllers;

[ApiController]
[Route("api/header-links")]
public class HeaderLinksController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HeaderLinkDto>>> List(CancellationToken cancellationToken)
    {
        var rows = await db.HeaderLinks
            .AsNoTracking()
            .OrderBy(x => x.SortOrder)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        return Ok(rows.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<HeaderLinkDto>> Get(int id, CancellationToken cancellationToken)
    {
        var row = await db.HeaderLinks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return row is null ? NotFound() : Ok(ToDto(row));
    }

    [HttpPost]
    public async Task<ActionResult<HeaderLinkDto>> Create(
        [FromBody] HeaderLinkRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryNormalize(request, out var label, out var path, out var error))
        {
            return BadRequest(new { message = error });
        }

        var nextOrder = request.SortOrder
            ?? (await db.HeaderLinks.Select(x => (int?)x.SortOrder).MaxAsync(cancellationToken) ?? 0) + 1;

        var row = new HeaderLink
        {
            Label = label,
            Path = path,
            SortOrder = nextOrder,
            Visible = request.Visible ?? true,
            IsCta = request.IsCta ?? false,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.HeaderLinks.Add(row);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = row.Id }, ToDto(row));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<HeaderLinkDto>> Update(
        int id,
        [FromBody] HeaderLinkRequest request,
        CancellationToken cancellationToken)
    {
        var row = await db.HeaderLinks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row is null)
        {
            return NotFound();
        }

        if (request.Label is not null || request.Path is not null)
        {
            var label = request.Label ?? row.Label;
            var path = request.Path ?? row.Path;
            if (!TryNormalize(new HeaderLinkRequest { Label = label, Path = path }, out label, out path, out var error))
            {
                return BadRequest(new { message = error });
            }

            row.Label = label;
            row.Path = path;
        }

        if (request.SortOrder is int order)
        {
            row.SortOrder = order;
        }

        if (request.Visible is bool visible)
        {
            row.Visible = visible;
        }

        if (request.IsCta is bool cta)
        {
            row.IsCta = cta;
        }

        await db.SaveChangesAsync(cancellationToken);
        return Ok(ToDto(row));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var row = await db.HeaderLinks.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row is null)
        {
            return NotFound();
        }

        db.HeaderLinks.Remove(row);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static bool TryNormalize(HeaderLinkRequest request, out string label, out string path, out string? error)
    {
        label = (request.Label ?? string.Empty).Trim();
        path = (request.Path ?? string.Empty).Trim();
        error = null;

        if (label.Length is 0 or > 80)
        {
            error = "Enter a label of 1 to 80 characters.";
            return false;
        }

        if (path.Length is 0 or > 240)
        {
            error = "Enter a path such as /about or a full https:// URL.";
            return false;
        }

        var ok = path.StartsWith('/') || path.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
            || path.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        if (!ok)
        {
            error = "Path must start with / or http.";
            return false;
        }

        return true;
    }

    private static HeaderLinkDto ToDto(HeaderLink row) =>
        new(row.Id, row.Label, row.Path, row.SortOrder, row.Visible, row.IsCta, row.CreatedAtUtc);
}
