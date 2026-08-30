using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Data;
using SaraRose.Api.DTOs;
using SaraRose.Api.Models;

namespace SaraRose.Api.Controllers;

[ApiController]
[Route("api/enquiries")]
public class EnquiriesController(AppDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<EnquiryDto>> Create(
        [FromBody] CreateEnquiryRequest request,
        CancellationToken cancellationToken)
    {
        if (request.CategoryId is int categoryId)
        {
            var exists = await db.Categories.AnyAsync(c => c.Id == categoryId, cancellationToken);
            if (!exists)
            {
                return BadRequest(new { message = "Unknown equipment category." });
            }
        }

        var enquiry = new Enquiry
        {
            FullName = request.FullName.Trim(),
            Company = string.IsNullOrWhiteSpace(request.Company) ? null : request.Company.Trim(),
            Phone = request.Phone.Trim(),
            Email = request.Email.Trim(),
            CategoryId = request.CategoryId,
            MachineType = string.IsNullOrWhiteSpace(request.MachineType) ? null : request.MachineType.Trim(),
            SiteLocation = string.IsNullOrWhiteSpace(request.SiteLocation) ? null : request.SiteLocation.Trim(),
            Requirement = request.Requirement.Trim(),
            Status = "New",
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Enquiries.Add(enquiry);
        await db.SaveChangesAsync(cancellationToken);

        var categoryName = enquiry.CategoryId is null
            ? null
            : await db.Categories.Where(c => c.Id == enquiry.CategoryId).Select(c => c.Name).FirstAsync(cancellationToken);

        var dto = ToDto(enquiry, categoryName);
        return CreatedAtAction(nameof(GetById), new { id = enquiry.Id }, dto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EnquiryDto>>> List(CancellationToken cancellationToken)
    {
        var items = await db.Enquiries
            .AsNoTracking()
            .Include(e => e.Category)
            .OrderByDescending(e => e.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return Ok(items.Select(e => ToDto(e, e.Category?.Name)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EnquiryDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var enquiry = await db.Enquiries
            .AsNoTracking()
            .Include(e => e.Category)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

        if (enquiry is null)
        {
            return NotFound();
        }

        return Ok(ToDto(enquiry, enquiry.Category?.Name));
    }

    private static EnquiryDto ToDto(Enquiry enquiry, string? categoryName) =>
        new(
            enquiry.Id,
            enquiry.FullName,
            enquiry.Company,
            enquiry.Phone,
            enquiry.Email,
            enquiry.CategoryId,
            categoryName,
            enquiry.MachineType,
            enquiry.SiteLocation,
            enquiry.Requirement,
            enquiry.Status,
            enquiry.CreatedAtUtc);
}
