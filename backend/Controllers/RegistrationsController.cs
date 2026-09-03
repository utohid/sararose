using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Data;
using SaraRose.Api.DTOs;
using SaraRose.Api.Models;

namespace SaraRose.Api.Controllers;

[ApiController]
[Route("api/registrations")]
public class RegistrationsController(AppDbContext db) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<RegistrationDto>> Create(
        [FromBody] CreateRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var taken = await db.Registrations.AnyAsync(x => x.Email == email, cancellationToken);
        if (taken)
        {
            return Conflict(new { message = "That email is already registered." });
        }

        var row = new UserRegistration
        {
            FullName = request.FullName.Trim(),
            Email = email,
            Phone = request.Phone.Trim(),
            Company = string.IsNullOrWhiteSpace(request.Company) ? null : request.Company.Trim(),
            City = string.IsNullOrWhiteSpace(request.City) ? null : request.City.Trim(),
            PasswordHash = HashPassword(request.Password),
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Registrations.Add(row);
        await db.SaveChangesAsync(cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = row.Id }, ToDto(row));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegistrationDto>>> List(CancellationToken cancellationToken)
    {
        var rows = await db.Registrations
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return Ok(rows.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RegistrationDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var row = await db.Registrations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return row is null ? NotFound() : Ok(ToDto(row));
    }

    private static RegistrationDto ToDto(UserRegistration row) =>
        new(row.Id, row.FullName, row.Email, row.Phone, row.Company, row.City, row.CreatedAtUtc);

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }
}
