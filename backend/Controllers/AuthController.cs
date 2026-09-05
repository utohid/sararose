using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaraRose.Api.Data;
using SaraRose.Api.DTOs;
using SaraRose.Api.Security;

namespace SaraRose.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthUserDto>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var email = (request.Email ?? string.Empty).Trim().ToLowerInvariant();
        var password = request.Password ?? string.Empty;
        if (string.IsNullOrWhiteSpace(email) || password.Length < 8)
        {
            return Unauthorized(new { message = "Enter a valid email and a password of at least 8 characters." });
        }

        var user = await db.Registrations.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

        if (user is null || !PasswordUtility.Matches(password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Email or password is not in the database." });
        }

        return Ok(new AuthUserDto(
            user.Id,
            user.FullName,
            user.Email,
            user.Phone,
            user.Company,
            user.City,
            user.Role,
            user.UserType,
            user.CreatedAtUtc));
    }
}
