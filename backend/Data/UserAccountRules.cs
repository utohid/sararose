using SaraRose.Api.Models;
using SaraRose.Api.Security;

namespace SaraRose.Api.Data;

public static class UserAccountRules
{
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

    public static UserRegistration AdminSeed() =>
        new()
        {
            FullName = "SARA ROSE Admin",
            Email = "admin@sararose.com",
            Phone = "+2348066651111",
            Company = "SARA ROSE NIGERIA LIMITED",
            City = "Sagamu",
            Role = "Admin",
            UserType = "Internal",
            PasswordHash = PasswordUtility.Hash("SaraRose_Admin_2024"),
            CreatedAtUtc = DateTime.UtcNow
        };
}
