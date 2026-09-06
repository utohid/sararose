using System.Text;
using System.Text.RegularExpressions;

namespace SaraRose.Api.Data;

public static class CatalogText
{
    public const string DefaultAvailability =
        "Brands, models, technical specifications, capacities and availability are confirmed on a per-enquiry basis.";

    public static string Slug(string? value)
    {
        var source = (value ?? string.Empty).Trim().ToLowerInvariant();
        var builder = new StringBuilder();
        var dash = false;
        foreach (var c in source)
        {
            if (char.IsAsciiLetterOrDigit(c))
            {
                builder.Append(c);
                dash = false;
            }
            else if (builder.Length > 0 && !dash)
            {
                builder.Append('-');
                dash = true;
            }
        }

        var slug = builder.ToString().Trim('-');
        return slug.Length == 0 ? "item" : slug;
    }

    public static string Code(string? value, int sortOrder)
    {
        var code = (value ?? string.Empty).Trim();
        if (code.Length > 0)
        {
            return code.Length > 16 ? code[..16] : code;
        }

        return sortOrder.ToString("00");
    }

    public static string Required(string? value, int max, string fallback = "")
    {
        var text = Regex.Replace((value ?? string.Empty).Trim(), @"\s+", " ");
        if (text.Length == 0)
        {
            text = fallback;
        }

        return text.Length > max ? text[..max] : text;
    }
}
