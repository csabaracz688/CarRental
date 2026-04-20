using CarRental.Application.Common.Interfaces;
using CarRental.Application.Features;
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
        // 1) Car lekérés (nem csak AnyAsync), mert kell az Unavailable info
        var car = await _db.Cars.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == dto.CarId, ct);

        if (car is null)
            throw new ArgumentException("Invalid CarId");

        // 2) Nem elérhető időszak ellenőrzése (szerviz/törött/admin hold stb.)
        if (car.UnavailableFrom.HasValue && car.UnavailableTo.HasValue)
        {
            var overlaps =
                dto.StartDate < car.UnavailableTo.Value &&
                dto.EndDate > car.UnavailableFrom.Value;

            if (overlaps)
            {
                var reason = car.UnavailableReason?.ToString() ?? "Unavailable";
                var note = string.IsNullOrWhiteSpace(car.UnavailableNote) ? "" : $" Note: {car.UnavailableNote}";
                throw new ArgumentException(
                    $"Car is unavailable due to {reason} from {car.UnavailableFrom:yyyy-MM-dd} to {car.UnavailableTo:yyyy-MM-dd}.{note}"
                );
            }
        }

        // 3) Guest / user ellenőrzés (ahogy nálad volt)
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

        // 4) Rental létrehozás
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

    public async Task<int> RequestRentalAsync(RequestRentalDto dto, CancellationToken ct = default)
    {
        var car = await _db.Cars
            .Include(c => c.Rentals)
            .FirstOrDefaultAsync(c => c.Id == dto.CarId, ct);

        if (car == null)
            throw new InvalidOperationException("Car not found");

        if (car.Status != CarStatus.Available)
        {
            throw new InvalidOperationException(
                $"Car is not available. Current status: {car.Status}");
        }

        if (car.UnavailableFrom.HasValue && car.UnavailableTo.HasValue)
        {
            bool blocked =
                car.UnavailableFrom < dto.EndDate &&
                car.UnavailableTo > dto.StartDate;

            if (blocked)
            {
                throw new InvalidOperationException(
                    $"Car is unavailable due to maintenance between {car.UnavailableFrom} - {car.UnavailableTo}");
            }
        }
        var overlap = await _db.Rentals
            .Where(r => r.CarId == dto.CarId)
            .Where(r =>
                (r.Status == CarRentStatus.Approved ||
                 r.Status == CarRentStatus.Handed) &&
                r.StartDate < dto.EndDate &&
                r.EndDate > dto.StartDate)
            .FirstOrDefaultAsync();


        if (overlap != null)
        {
            throw new InvalidOperationException(
                $"Car already booked between {overlap.StartDate} - {overlap.EndDate}");
        }
        var rental = new Rental
        {
            CarId = dto.CarId,
            UserId = dto.UserId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = CarRentStatus.Requested
        };

        _db.Rentals.Add(rental);
        await _db.SaveChangesAsync(ct);

        return rental.Id;
    }


}