using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Persistence;
using CarRental.Infrastructure.Persistence.Seeding.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure.Persistence.Seeding;

public sealed class RentalSeedStep : IAppSeedStep
{
    private const string CustomerEmail = "customer@carrental.local";
    private const string OfficerEmail = "officer@carrental.local";

    private readonly CarRentalDbContext _db;
    private readonly ILogger<RentalSeedStep> _logger;

    private static readonly SeedRentalModel[] SeedRentals =
    [
        new("CAR-001", CustomerEmail, null, null, null, new DateTime(2026, 4, 1), new DateTime(2026, 4, 5), CarRentStatus.Requested),
        new("CAR-002", null, "Guest One", "guest.one@carrental.local", "+3612345001", new DateTime(2026, 3, 20), new DateTime(2026, 3, 24), CarRentStatus.Approved),
        new("CAR-003", null, "Guest Two", "guest.two@carrental.local", "+3612345002", new DateTime(2026, 3, 12), new DateTime(2026, 3, 14), CarRentStatus.Rejected),
        new("CAR-004", CustomerEmail, null, null, null, new DateTime(2026, 3, 10), new DateTime(2026, 3, 13), CarRentStatus.Handed),
        new("CAR-005", CustomerEmail, null, null, null, new DateTime(2026, 2, 1), new DateTime(2026, 2, 5), CarRentStatus.Returned)
    ];

    public RentalSeedStep(CarRentalDbContext db, ILogger<RentalSeedStep> logger)
    {
        _db = db;
        _logger = logger;
    }

    public int Order => 40;
    public string Name => "Rentals";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var customerUser = await _db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Email == CustomerEmail, cancellationToken);

        var officerUser = await _db.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user => user.Email == OfficerEmail, cancellationToken);

        if (customerUser is null || officerUser is null)
        {
            _logger.LogWarning("Rental seed skipped because required users are missing.");
            return;
        }

        var carLookup = (await _db.Cars
                .AsNoTracking()
                .ToListAsync(cancellationToken))
            .ToDictionary(car => car.LicensePlate, StringComparer.OrdinalIgnoreCase);

        var createdCount = 0;

        foreach (var seedRental in SeedRentals)
        {
            if (!carLookup.TryGetValue(seedRental.CarLicensePlate, out var car))
            {
                _logger.LogWarning("Rental seed skipped one item because car {LicensePlate} does not exist.", seedRental.CarLicensePlate);
                continue;
            }

            var userId = seedRental.UserEmail is null ? (int?)null : customerUser.Id;

            var exists = await _db.Rentals.AnyAsync(
                rental => rental.CarId == car.Id
                    && rental.StartDate == seedRental.StartDate
                    && rental.EndDate == seedRental.EndDate
                    && rental.UserId == userId
                    && rental.GuestEmail == seedRental.GuestEmail,
                cancellationToken);

            if (exists)
            {
                continue;
            }

            var rental = new Rental
            {
                CarId = car.Id,
                UserId = userId,
                GuestName = seedRental.GuestName,
                GuestEmail = seedRental.GuestEmail,
                GuestPhone = seedRental.GuestPhone,
                StartDate = seedRental.StartDate,
                EndDate = seedRental.EndDate,
                Status = seedRental.Status,
                ApprovedByUserId = NeedsApproval(seedRental.Status) ? officerUser.Id : null,
                HandedOverAt = seedRental.Status is CarRentStatus.Handed or CarRentStatus.Returned
                    ? seedRental.StartDate.AddHours(9)
                    : null,
                ClosedAt = seedRental.Status == CarRentStatus.Returned
                    ? seedRental.EndDate.AddHours(18)
                    : null
            };

            _db.Rentals.Add(rental);
            createdCount++;
        }

        if (createdCount > 0)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Rental seed finished. Created: {CreatedCount}, Skipped: {SkippedCount}.", createdCount, SeedRentals.Length - createdCount);
    }

    private static bool NeedsApproval(CarRentStatus status) =>
        status is CarRentStatus.Approved or CarRentStatus.Rejected or CarRentStatus.Handed or CarRentStatus.Returned;

}
