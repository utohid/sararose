using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Data;
using SaraRose.Api.DTOs;
using SaraRose.Api.Models;
using SaraRose.Api.Security;

namespace SaraRose.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/user-masters")]
public class UserMastersController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserMasterDto>>> List(CancellationToken cancellationToken)
    {
        var rows = await db.UserMasters
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return Ok(rows.Select(ToDto));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserMasterDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var row = await db.UserMasters.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return row is null ? NotFound() : Ok(ToDto(row));
    }

    [HttpPost]
    public async Task<ActionResult<UserMasterDto>> Create(
        [FromBody] CreateUserMasterRequest request,
        CancellationToken cancellationToken)
    {
        var usernameError = UserAccountRules.ValidateUsername(request.Username);
        if (usernameError is not null)
        {
            return BadRequest(new { message = usernameError });
        }

        var username = UserAccountRules.NormalizeUsername(request.Username);
        var email = request.Email.Trim().ToLowerInvariant();

        if (await db.UserMasters.AnyAsync(x => x.Username == username, cancellationToken))
        {
            return Conflict(new { message = "That username is already in UserMaster." });
        }

        if (await db.UserMasters.AnyAsync(x => x.Email == email, cancellationToken))
        {
            return Conflict(new { message = "That email is already in UserMaster." });
        }

        var row = new UserMaster
        {
            Username = username,
            Email = email,
            FullName = request.FullName.Trim(),
            Phone = request.Phone.Trim(),
            Role = UserAccountRules.NormalizeRole(request.Role, allowAdmin: true),
            UserType = UserAccountRules.NormalizeUserType(request.UserType),
            HashPassword = PasswordUtility.Hash(request.Password),
            NormalPassword = request.Password,
            Active = request.Active,
            CreatedAtUtc = DateTime.UtcNow
        };

        db.UserMasters.Add(row);
        await db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = row.Id }, ToDto(row));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserMasterDto>> Update(
        int id,
        [FromBody] UpdateUserMasterRequest request,
        CancellationToken cancellationToken)
    {
        var row = await db.UserMasters.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.Username))
        {
            var usernameError = UserAccountRules.ValidateUsername(request.Username);
            if (usernameError is not null)
            {
                return BadRequest(new { message = usernameError });
            }

            var username = UserAccountRules.NormalizeUsername(request.Username);
            if (await db.UserMasters.AnyAsync(x => x.Id != id && x.Username == username, cancellationToken))
            {
                return Conflict(new { message = "That username is already in UserMaster." });
            }

            row.Username = username;
        }

        if (!string.IsNullOrWhiteSpace(request.Email))
        {
            var email = request.Email.Trim().ToLowerInvariant();
            if (await db.UserMasters.AnyAsync(x => x.Id != id && x.Email == email, cancellationToken))
            {
                return Conflict(new { message = "That email is already in UserMaster." });
            }

            row.Email = email;
        }

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            row.FullName = request.FullName.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Phone))
        {
            row.Phone = request.Phone.Trim();
        }

        if (request.Role is not null)
        {
            row.Role = UserAccountRules.NormalizeRole(request.Role, allowAdmin: true);
        }

        if (request.UserType is not null)
        {
            row.UserType = UserAccountRules.NormalizeUserType(request.UserType);
        }

        if (request.Active is not null)
        {
            if (row.Role == "Admin" && request.Active == false)
            {
                var otherAdmins = await db.UserMasters.CountAsync(
                    x => x.Id != id && x.Role == "Admin" && x.Active,
                    cancellationToken);
                if (otherAdmins == 0)
                {
                    return BadRequest(new { message = "Keep at least one active Admin in UserMaster." });
                }
            }

            row.Active = request.Active.Value;
        }

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            if (request.Password.Length < 8)
            {
                return BadRequest(new { message = "Password must be at least 8 characters." });
            }

            row.HashPassword = PasswordUtility.Hash(request.Password);
            row.NormalPassword = request.Password;
        }

        await db.SaveChangesAsync(cancellationToken);
        return Ok(ToDto(row));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var row = await db.UserMasters.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row is null)
        {
            return NotFound();
        }

        if (row.Role == "Admin" && row.Active)
        {
            var otherAdmins = await db.UserMasters.CountAsync(
                x => x.Id != id && x.Role == "Admin" && x.Active,
                cancellationToken);
            if (otherAdmins == 0)
            {
                return BadRequest(new { message = "Keep at least one active Admin in UserMaster." });
            }
        }

        db.UserMasters.Remove(row);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    private static UserMasterDto ToDto(UserMaster row) =>
        new(row.Id, row.Username, row.FullName, row.Email, row.Phone, row.Role, row.UserType, row.Active, row.CreatedAtUtc);
}
