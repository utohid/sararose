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
    [HttpPost]
    public async Task<ActionResult<RegistrationDto>> Create(
        [FromBody] CreateRegistrationRequest request,
        CancellationToken cancellationToken)
    {
        var usernameError = UserAccountRules.ValidateUsername(request.Username);
        if (usernameError is not null)
        {
            return BadRequest(new { message = usernameError });
        }

        var username = UserAccountRules.NormalizeUsername(request.Username);
        var email = request.Email.Trim().ToLowerInvariant();
        var takenEmail = await db.Registrations.AnyAsync(x => x.Email == email, cancellationToken);
        if (takenEmail)
        {
            return Conflict(new { message = "That email is already registered." });
        }

        var takenUsername = await db.UserMasters.AnyAsync(x => x.Username == username, cancellationToken);
        if (takenUsername)
        {
            return Conflict(new { message = "That username is already taken." });
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
            PasswordHash = hash,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.Registrations.Add(row);
        await db.SaveChangesAsync(cancellationToken);

        db.UserMasters.Add(new UserMaster
        {
            Username = username,
            Email = email,
            FullName = row.FullName,
            Phone = row.Phone,
            Role = row.Role,
            UserType = row.UserType,
            HashPassword = hash,
            NormalPassword = request.Password,
            RegistrationId = row.Id,
            Active = true,
            CreatedAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = row.Id }, ToDto(row, username));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RegistrationDto>>> List(CancellationToken cancellationToken)
    {
        var rows = await db.Registrations
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
        var masters = await db.UserMasters.AsNoTracking().ToListAsync(cancellationToken);
        var usernames = masters
            .GroupBy(x => x.Email, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Username, StringComparer.OrdinalIgnoreCase);

        return Ok(rows.Select(row => ToDto(row, usernames.GetValueOrDefault(row.Email, string.Empty))));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<RegistrationDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var row = await db.Registrations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row is null)
        {
            return NotFound();
        }

        var username = await db.UserMasters.AsNoTracking()
            .Where(x => x.RegistrationId == row.Id || x.Email == row.Email)
            .Select(x => x.Username)
            .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;

        return Ok(ToDto(row, username));
    }

    private static RegistrationDto ToDto(UserRegistration row, string username) =>
        new(row.Id, username, row.FullName, row.Email, row.Phone, row.Company, row.City, row.Role, row.UserType, row.CreatedAtUtc);
}
