using CarRental.Domain.Enums;

namespace CarRental.Infrastructure.Persistence.Seeding.Models;

public sealed record SeedCarModel(
    string LicensePlate,
    string Brand,
    string Model,
    int DistanceKm,
    int DailyPrice,
    CarStatus Status);
