using CarRental.Domain.Enums;

namespace CarRental.Infrastructure.Persistence.Seeding.Models;

public sealed record SeedRentalModel(
    string CarLicensePlate,
    string? UserEmail,
    string? GuestName,
    string? GuestEmail,
    string? GuestPhone,
    DateTime StartDate,
    DateTime EndDate,
    CarRentStatus Status);
