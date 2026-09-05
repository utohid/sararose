using System.ComponentModel.DataAnnotations;

namespace SaraRose.Api.DTOs;

public record CategoryDto(
    int Id,
    string Slug,
    string Code,
    string Name,
    string ShortName,
    string Summary,
    int EquipmentCount);

public record EquipmentSummaryDto(
    int Id,
    string Slug,
    string Name,
    string MachineType,
    string Summary,
    string CategorySlug,
    string CategoryName);

public record EquipmentDetailDto(
    int Id,
    string Slug,
    string Name,
    string MachineType,
    string Summary,
    string Description,
    string TypicalUse,
    string AvailabilityNote,
    CategoryDto Category);

public record CompanyDto(
    string Name,
    int YearEstablished,
    string BusinessType,
    string Industry,
    string HeadOffice,
    string OperatingLocation,
    string ContactPerson,
    string Telephone,
    string Email,
    string WebsiteNote,
    string About,
    string HowWeWork,
    string Vision,
    string Mission,
    IReadOnlyList<string> Sectors,
    IReadOnlyList<ReasonDto> Reasons,
    IReadOnlyList<ValueDto> Values);

public record ReasonDto(string Title, string Body);
public record ValueDto(string Title, string Body);

public class CreateEnquiryRequest
{
    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(160)]
    public string? Company { get; set; }

    [Required, StringLength(40)]
    public string Phone { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(160)]
    public string Email { get; set; } = string.Empty;

    public int? CategoryId { get; set; }

    [StringLength(80)]
    public string? MachineType { get; set; }

    [StringLength(160)]
    public string? SiteLocation { get; set; }

    [Required, StringLength(2000, MinimumLength = 12)]
    public string Requirement { get; set; } = string.Empty;
}

public record EnquiryDto(
    int Id,
    string FullName,
    string? Company,
    string Phone,
    string Email,
    int? CategoryId,
    string? CategoryName,
    string? MachineType,
    string? SiteLocation,
    string Requirement,
    string Status,
    DateTime CreatedAtUtc);

public record SliderSlideDto(
    int Id,
    int SortOrder,
    string Alt,
    string Url,
    DateTime CreatedAtUtc);

public record HeaderLinkDto(
    int Id,
    string Label,
    string Path,
    int SortOrder,
    bool Visible,
    bool IsCta,
    DateTime CreatedAtUtc);

public class HeaderLinkRequest
{
    [StringLength(80)]
    public string? Label { get; set; }

    [StringLength(240)]
    public string? Path { get; set; }

    public int? SortOrder { get; set; }

    public bool? Visible { get; set; }

    public bool? IsCta { get; set; }
}

public class CreateRegistrationRequest
{
    [Required, StringLength(80, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(160)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string Phone { get; set; } = string.Empty;

    [StringLength(160)]
    public string? Company { get; set; }

    [StringLength(120)]
    public string? City { get; set; }

    [Required, StringLength(120, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [StringLength(40)]
    public string? Role { get; set; }

    [StringLength(40)]
    public string? UserType { get; set; }
}

public record RegistrationDto(
    int Id,
    string Username,
    string FullName,
    string Email,
    string Phone,
    string? Company,
    string? City,
    string Role,
    string UserType,
    DateTime CreatedAtUtc);

public class LoginRequest
{
    [StringLength(160)]
    public string Username { get; set; } = string.Empty;

    [StringLength(160)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(120, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}

public record AuthUserDto(
    int Id,
    string Username,
    string FullName,
    string Email,
    string Phone,
    string? Company,
    string? City,
    string Role,
    string UserType,
    DateTime CreatedAtUtc);
