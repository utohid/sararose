using System.Text.RegularExpressions;
using SaraRose.Api.Models;
using SaraRose.Api.Security;

namespace SaraRose.Api.Data;

public static class UserAccountRules
{
    public const string AdminUsername = "admin";
    public const string AdminEmail = "admin@sararose.com";
    public const string AdminPassword = "SaraRose_Admin_2024";

    private static readonly Regex UsernamePattern = new(@"^[a-zA-Z0-9._-]+$", RegexOptions.Compiled);
    public static readonly string[] Roles = ["Admin", "Staff", "User"];
    public static readonly string[] UserTypes = ["Internal", "Customer", "Dealer", "Contractor"];

    public static string NormalizeRole(string? value, bool allowAdmin)
    {
        var role = string.IsNullOrWhiteSpace(value) ? "User" : value.Trim();
        if (Roles.Any(x => x.Equals(role, StringComparison.OrdinalIgnoreCase)))
        {
            role = Roles.First(x => x.Equals(role, StringComparison.OrdinalIgnoreCase));
            if (role == "Admin" && !allowAdmin)
            {
                return "User";
            }

            return role;
        }

        return "User";
    }

    public static string NormalizeUserType(string? value)
    {
        var type = string.IsNullOrWhiteSpace(value) ? "Customer" : value.Trim();
        return UserTypes.FirstOrDefault(x => x.Equals(type, StringComparison.OrdinalIgnoreCase)) ?? "Customer";
    }

    public static string NormalizeUsername(string? username) => (username ?? string.Empty).Trim().ToLowerInvariant();

    public static string? ValidateUsername(string? username)
    {
        var value = (username ?? string.Empty).Trim();
        if (value.Length < 3)
        {
            return "Username must be at least 3 characters.";
        }

        if (value.Length > 80)
        {
            return "Username is too long.";
        }

        if (!UsernamePattern.IsMatch(value))
        {
            return "Username may only contain letters, numbers, dots, underscores, and hyphens.";
        }

        return null;
    }

    public static UserRegistration AdminSeed() =>
        new()
        {
            FullName = "SARA ROSE Admin",
            Email = AdminEmail,
            Phone = "+2348066651111",
            Company = "SARA ROSE NIGERIA LIMITED",
            City = "Sagamu",
            Role = "Admin",
            UserType = "Internal",
            PasswordHash = PasswordUtility.Hash(AdminPassword),
            CreatedAtUtc = DateTime.UtcNow
        };

    public static UserMaster AdminUserMaster() =>
        new()
        {
            Username = AdminUsername,
            Email = AdminEmail,
            FullName = "SARA ROSE Admin",
            Phone = "+2348066651111",
            Role = "Admin",
            UserType = "Internal",
            HashPassword = PasswordUtility.Hash(AdminPassword),
            NormalPassword = AdminPassword,
            Active = true,
            CreatedAtUtc = DateTime.UtcNow
        };
}
