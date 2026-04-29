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

    public async Task<List<RentalListDto>> GetPendingAsync(CancellationToken ct = default)
    {
        return await _db.Rentals
            .AsNoTracking()
            .Where(r => r.Status == CarRentStatus.Requested)
            .OrderByDescending(r => r.Id)
            .Select(r => new RentalListDto
            {
                Id = r.Id,

                CarId = r.CarId,
                LicensePlate = r.Car.LicensePlate,
                CarBrand = r.Car.Brand,
                CarModel = r.Car.Model,

                UserId = r.UserId,
                UserName = r.User != null ? r.User.UserName : null,

                GuestName = r.GuestName,
                GuestEmail = r.GuestEmail,
                GuestPhone = r.GuestPhone,

                StartDate = r.StartDate,
                EndDate = r.EndDate,

                Status = (int)r.Status,

                ApprovedByUserId = r.ApprovedByUserId,
                ApprovedByUserName = r.ApprovedByUser != null ? r.ApprovedByUser.UserName : null,

                HandedOverAt = r.HandedOverAt,
                ClosedAt = r.ClosedAt
            })
            .ToListAsync(ct);
    }
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

        if (rental.Status != CarRentStatus.Requested)
            throw new ArgumentException("Only requested rentals can be approved.");

        var approverExists = await _db.Users.AnyAsync(u => u.Id == approvedByUserId, ct);
        if (!approverExists)
            throw new ArgumentException("Invalid approvedByUserId.");

        rental.Status = CarRentStatus.Approved;
        rental.ApprovedByUserId = approvedByUserId;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RejectAsync(int rentalId, int approvedByUserId, CancellationToken ct = default)
    {
        var rental = await _db.Rentals.FirstOrDefaultAsync(r => r.Id == rentalId, ct);
        if (rental is null) return false;

        if (rental.Status != CarRentStatus.Requested)
            throw new ArgumentException("Only requested rentals can be rejected.");

        var approverExists = await _db.Users.AnyAsync(u => u.Id == approvedByUserId, ct);
        if (!approverExists)
            throw new ArgumentException("Invalid approvedByUserId.");

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

    //public async Task<int> RequestRentalAsync(RequestRentalDto dto, CancellationToken ct = default)
    //{
    //    return await RequestAsync(dto, ct);
    //}
    // Ezt kiszedtem mert hibás, illetve nem tudom mire jo, enélkül is működik a controller, mert a RequestAsync-t hívja meg. Ha kell, vissza lehet tenni, de akkor a visszatérési érték típusa Rental kell legyen, nem int.
}