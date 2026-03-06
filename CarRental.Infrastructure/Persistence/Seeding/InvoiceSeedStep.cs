using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure.Persistence.Seeding;

public sealed class InvoiceSeedStep : IAppSeedStep
{
    private readonly CarRentalDbContext _db;
    private readonly ILogger<InvoiceSeedStep> _logger;

    public InvoiceSeedStep(CarRentalDbContext db, ILogger<InvoiceSeedStep> logger)
    {
        _db = db;
        _logger = logger;
    }

    public int Order => 50;
    public string Name => "Invoices";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var returnedRentals = await _db.Rentals
            .AsNoTracking()
            .Include(rental => rental.Car)
            .Where(rental => rental.Status == CarRentStatus.Returned)
            .OrderBy(rental => rental.Id)
            .ToListAsync(cancellationToken);

        if (returnedRentals.Count == 0)
        {
            _logger.LogInformation("Invoice seed skipped because no returned rentals were found.");
            return;
        }

        var existingInvoiceRentalIds = (await _db.Invoices
                .AsNoTracking()
                .Select(invoice => invoice.RentalId)
                .ToListAsync(cancellationToken))
            .ToHashSet();

        var invoicesToInsert = new List<Invoice>();

        foreach (var rental in returnedRentals)
        {
            if (existingInvoiceRentalIds.Contains(rental.Id))
            {
                continue;
            }

            var rentalDays = Math.Max(1, (rental.EndDate.Date - rental.StartDate.Date).Days);
            var amount = rentalDays * rental.Car.DailyPrice;

            invoicesToInsert.Add(new Invoice
            {
                RentalId = rental.Id,
                Amount = amount,
                IssuedAt = rental.ClosedAt ?? rental.EndDate.AddDays(1),
                PaidAt = rental.ClosedAt?.AddDays(2)
            });
        }

        if (invoicesToInsert.Count == 0)
        {
            _logger.LogInformation("Invoice seed skipped. Invoices already exist for all eligible rentals.");
            return;
        }

        _db.Invoices.AddRange(invoicesToInsert);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Invoice seed created {Count} invoices.", invoicesToInsert.Count);
    }
}
