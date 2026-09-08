using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SaraRose.Api.DTOs;

namespace SaraRose.Api.Security;

public class JwtTokenService(IConfiguration configuration)
{
    public const string RoleClaim = "role";
    public const string NameClaim = "name";
    public const string FullNameClaim = "fullName";
    public const string UserTypeClaim = "userType";
    public const string PhoneClaim = "phone";
    public const string CompanyClaim = "company";
    public const string CityClaim = "city";
    public const string UserIdClaim = "userId";

    public (string Token, DateTime ExpiresAtUtc) Create(AuthUserDto user)
    {
        var issuer = configuration["Jwt:Issuer"] ?? "SaraRose.Api";
        var audience = configuration["Jwt:Audience"] ?? "SaraRose.Web";
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key is missing.");
        var minutes = int.TryParse(configuration["Jwt:ExpiresMinutes"], out var value) ? value : 480;
        var expires = DateTime.UtcNow.AddMinutes(minutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(UserIdClaim, user.Id.ToString()),
            new(NameClaim, user.Username),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(FullNameClaim, user.FullName),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Email, user.Email),
            new(RoleClaim, user.Role),
            new(ClaimTypes.Role, user.Role),
            new(UserTypeClaim, user.UserType),
            new(PhoneClaim, user.Phone ?? string.Empty),
            new(CompanyClaim, user.Company ?? string.Empty),
            new(CityClaim, user.City ?? string.Empty)
        };

        var signing = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            DateTime.UtcNow.AddMinutes(-1),
            expires,
            signing);

        return (new JwtSecurityTokenHandler().WriteToken(token), expires);
    }

    public static AuthUserDto FromPrincipal(ClaimsPrincipal user)
    {
        var idValue = user.FindFirstValue(UserIdClaim)
            ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub)
            ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? "0";
        _ = int.TryParse(idValue, out var id);

        return new AuthUserDto(
            id,
            user.FindFirstValue(NameClaim)
                ?? user.FindFirstValue(JwtRegisteredClaimNames.UniqueName)
                ?? user.Identity?.Name
                ?? string.Empty,
            user.FindFirstValue(FullNameClaim) ?? string.Empty,
            user.FindFirstValue(JwtRegisteredClaimNames.Email)
                ?? user.FindFirstValue(ClaimTypes.Email)
                ?? string.Empty,
            user.FindFirstValue(PhoneClaim) ?? string.Empty,
            EmptyToNull(user.FindFirstValue(CompanyClaim)),
            EmptyToNull(user.FindFirstValue(CityClaim)),
            user.FindFirstValue(RoleClaim) ?? user.FindFirstValue(ClaimTypes.Role) ?? "User",
            user.FindFirstValue(UserTypeClaim) ?? "Customer",
            DateTime.UtcNow);
    }

    private static string? EmptyToNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
