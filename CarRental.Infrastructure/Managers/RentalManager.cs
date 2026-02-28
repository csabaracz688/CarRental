using CarRental.Application.Common.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Managers;

public class RentalManager : IRentalManager
{
    private readonly CarRentalDbContext _db;

    public RentalManager(CarRentalDbContext db)
    {
        _db = db;
    }

    public Task<List<Rental>> GetAllAsync(CancellationToken ct = default) =>
        _db.Rentals.AsNoTracking()
            .Include(r => r.Car)
            .Include(r => r.User)
            .Include(r => r.ApprovedByUser)
            .OrderByDescending(r => r.Id)
            .ToListAsync(ct);
    public async Task<Rental> RequestAsync(RequestRentalDto dto, CancellationToken ct = default) 
    {
        var carExists = await _db.Cars.AnyAsync(c => c.Id == dto.CarId, ct);
        if (!carExists) throw new ArgumentException("Invalid CarId");

        var isGuest = dto.UserId is null;
        if (isGuest) 
        {
            if (string.IsNullOrWhiteSpace(dto.GuestName) ||
                 string.IsNullOrWhiteSpace(dto.GuestEmail) ||
                 string.IsNullOrWhiteSpace(dto.GuestPhone))
                throw new ArgumentException("GuestName, GuestEmail and GuestPhone are required for guest rentals.");
        }
        else
        {
            var userExists = await _db.Users.AnyAsync(u => u.Id == dto.UserId, ct);
            if (!userExists) throw new ArgumentException("Invalid UserId.");
        }

        var rental = new Rental
        {
            CarId = dto.CarId,
            UserId = dto.UserId,
            GuestName = dto.GuestName,
            GuestEmail = dto.GuestEmail,
            GuestPhone = dto.GuestPhone,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = CarRentStatus.Requested
        };

        _db.Rentals.Add(rental);
        await _db.SaveChangesAsync(ct);
        return rental;
    }

    public async Task<bool> ApproveAsync(int rentalId, int approvedByUserId, CancellationToken ct = default)
    {
        var rental = await _db.Rentals.FirstOrDefaultAsync(r => r.Id == rentalId, ct);
        if (rental is null) return false;

        rental.Status = CarRentStatus.Approved;
        rental.ApprovedByUserId = approvedByUserId;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RejectAsync(int rentalId, int approvedByUserId, CancellationToken ct = default)
    {
        var rental = await _db.Rentals.FirstOrDefaultAsync(r => r.Id == rentalId, ct);
        if (rental is null) return false;

        rental.Status = CarRentStatus.Rejected;
        rental.ApprovedByUserId = approvedByUserId;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> CloseAsync(int rentalId, CancellationToken ct = default)
    {
        var rental = await _db.Rentals.FirstOrDefaultAsync(r => r.Id == rentalId, ct);
        if (rental is null) return false;

        rental.Status = CarRentStatus.Returned;
        rental.ClosedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
        return true;
    }
}

