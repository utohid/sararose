using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Data;
using SaraRose.Api.DTOs;
using SaraRose.Api.Models;
using SaraRose.Api.Security;

namespace SaraRose.Api.Controllers;

[ApiController]
[Route("api/registrations")]
public class RegistrationsController(AppDbContext db) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<RegistrationDto>> Create(
        [FromBody] CreateRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var equipmentType = request.EquipmentType.Trim();
        var machineType = request.MachineType.Trim();
        if (equipmentType.Length == 0 || machineType.Length == 0)
        {
            return BadRequest(new { message = "Select an equipment type and a machine type." });
        }

        var takenEmail = await db.Registrations.AnyAsync(x => x.Email == email, cancellationToken);
        if (takenEmail)
        {
            return Conflict(new { message = "That email is already registered." });
        }

        var hash = PasswordUtility.Hash(request.Password);
        var row = new UserRegistration
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = request.Phone.Trim(),
            Company = string.IsNullOrWhiteSpace(request.Company) ? null : request.Company.Trim(),
            City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
            Role = UserAccountRules.NormalizeRole(request.Role, allowAdmin: false),
            UserType = UserAccountRules.NormalizeUserType(request.UserType),
            EquipmentType = equipmentType,
            MachineType = machineType,
            PasswordHash = hash,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Registrations.Add(row);
        await db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = row.Id }, ToDto(row));
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegistrationDto>>> List(CancellationToken cancellationToken)
    {
        var rows = await db.Registrations
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return Ok(rows.Select(ToDto));
    }

    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<RegistrationDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var row = await db.Registrations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row is null)
        {
            return NotFound();
        }

        return Ok(ToDto(row));
    }

    private static RegistrationDto ToDto(UserRegistration row) =>
        new(
            row.Id,
            row.FullName,
            row.Email,
            row.Phone,
            row.Company,
            row.City,
            row.Role,
            row.UserType,
            row.EquipmentType,
            row.MachineType,
            row.CreatedAtUtc);
}
