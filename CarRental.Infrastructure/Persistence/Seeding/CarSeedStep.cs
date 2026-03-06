using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Persistence;
using CarRental.Infrastructure.Persistence.Seeding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure.Persistence.Seeding;

public sealed class CarSeedStep : IAppSeedStep
{
    private readonly CarRentalDbContext _db;
    private readonly ILogger<CarSeedStep> _logger;

    private static readonly SeedCarModel[] SeedCars =
    [
        new("CAR-001", "Toyota", "Corolla", 82450, 18000, CarStatus.Available),
        new("CAR-002", "Skoda", "Octavia", 54210, 22000, CarStatus.Available),
        new("CAR-003", "Ford", "Focus", 112350, 15000, CarStatus.Available),
        new("CAR-004", "Volkswagen", "Golf", 74520, 19500, CarStatus.Rented),
        new("CAR-005", "BMW", "320d", 132880, 32000, CarStatus.Available),
        new("CAR-006", "Nissan", "Leaf", 43890, 21000, CarStatus.Unavailable)
    ];

    public CarSeedStep(CarRentalDbContext db, ILogger<CarSeedStep> logger)
    {
        _db = db;
        _logger = logger;
    }

    public int Order => 30;
    public string Name => "Cars";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existingPlates = (await _db.Cars
                .AsNoTracking()
                .Select(car => car.LicensePlate)
                .ToListAsync(cancellationToken))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var carsToInsert = SeedCars
            .Where(seedCar => !existingPlates.Contains(seedCar.LicensePlate))
            .Select(seedCar => new Car
            {
                LicensePlate = seedCar.LicensePlate,
                Brand = seedCar.Brand,
                Model = seedCar.Model,
                DistanceKm = seedCar.DistanceKm,
                DailyPrice = seedCar.DailyPrice,
                Status = seedCar.Status
            })
            .ToList();

        if (carsToInsert.Count == 0)
        {
            _logger.LogInformation("Car seed skipped. All seed cars already exist.");
            return;
        }

        _db.Cars.AddRange(carsToInsert);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Car seed created {Count} cars.", carsToInsert.Count);
    }

}
