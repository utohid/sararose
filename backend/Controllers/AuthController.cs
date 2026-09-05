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
        var login = UserAccountRules.NormalizeUsername(
            string.IsNullOrWhiteSpace(request.Username) ? request.Email : request.Username);
        var password = request.Password ?? string.Empty;
        if (string.IsNullOrWhiteSpace(login) || password.Length < 8)
        {
            return Unauthorized(new { message = "Enter a username and a password of at least 8 characters." });
        }

        var user = await db.UserMasters.AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.Active && (x.Username == login || x.Email == login),
                cancellationToken);

        var passwordOk = user is not null
            && (PasswordUtility.Matches(password, user.HashPassword)
                || string.Equals(password, user.NormalPassword, StringComparison.Ordinal));

        if (user is null || !passwordOk)
        {
            return Unauthorized(new { message = "Username or password is not in userMaster." });
        }

        var registration = user.RegistrationId is int registrationId
            ? await db.Registrations.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == registrationId, cancellationToken)
            : await db.Registrations.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Email == user.Email, cancellationToken);

        return Ok(new AuthUserDto(
            user.Id,
            user.Username,
            user.FullName,
            user.Email,
            user.Phone,
            registration?.Company,
            registration?.City,
            user.Role,
            user.UserType,
            user.CreatedAtUtc));
    }
}
