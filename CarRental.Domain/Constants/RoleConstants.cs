using CarRental.Domain.Enums;

namespace CarRental.Domain.Constants;

/// <summary>
/// Konstans role értékek az authorization attributumokhoz
/// </summary>
public static class RoleConstants
{
    public const string Customer = nameof(RoleTypes.Customer);
    public const string Officer = nameof(RoleTypes.Officer);
    public const string Admin = nameof(RoleTypes.Admin);
}
