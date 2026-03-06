using CarRental.Domain.Enums;

namespace CarRental.Infrastructure.Persistence.Seeding.Models;

public sealed record SeedUserModel(
    string UserName,
    string Email,
    string Password,
    string FirstName,
    string LastName,
    string Address,
    string Phone,
    RoleTypes RoleType);
