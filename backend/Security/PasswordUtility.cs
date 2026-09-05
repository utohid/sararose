using System.Security.Cryptography;
using System.Text;

namespace SaraRose.Api.Security;

public static class PasswordUtility
{
    public static string Hash(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    public static bool Matches(string password, string storedHash) =>
        string.Equals(Hash(password), storedHash, StringComparison.OrdinalIgnoreCase);
}
