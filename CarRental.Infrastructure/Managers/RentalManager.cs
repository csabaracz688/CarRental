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

    private static string? BuildImageUrl(string? imagePath)
        => imagePath != null
            ? $"https://localhost:7077/uploads/{imagePath}"
            : null;

    private static IQueryable<RentalListDto> MapToRentalListDto(IQueryable<Rental> query)
    {
        return query.Select(r => new RentalListDto
        {
            Id = r.Id,

            CarId = r.CarId,
            LicensePlate = r.Car.LicensePlate,
            CarBrand = r.Car.Brand,
            CarModel = r.Car.Model,
            DailyPrice = r.Car.DailyPrice,
            ImageUrl = r.Car.ImagePath != null
                ? $"https://localhost:7077/uploads/{r.Car.ImagePath}"
                : null,

            UserId = r.UserId,
            UserName = r.User != null ? r.User.UserName : null,
            UserEmail = r.User != null ? r.User.Email : null,

            GuestName = r.GuestName,
            GuestEmail = r.GuestEmail,
            GuestPhone = r.GuestPhone,

            CustomerName = r.User != null ? r.User.UserName : r.GuestName,
            CustomerEmail = r.User != null ? r.User.Email : r.GuestEmail,

            StartDate = r.StartDate,
            EndDate = r.EndDate,

            Status = r.Status,
            StatusText = r.Status.ToString(),

            ApprovedByUserId = r.ApprovedByUserId,
            ApprovedByUserName = r.ApprovedByUser != null ? r.ApprovedByUser.UserName : null,

            HandedOverAt = r.HandedOverAt,
            ClosedAt = r.ClosedAt
        });
    }

    public async Task<List<RentalListDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await MapToRentalListDto(_db.Rentals.AsNoTracking())
            .OrderByDescending(r => r.Id)
            .ToListAsync(ct);
    }

    public async Task<List<RentalListDto>> GetPendingAsync(CancellationToken ct = default)
    {
        return await MapToRentalListDto(
                _db.Rentals
                    .AsNoTracking()
                    .Where(r => r.Status == CarRentStatus.Requested)
            )
            .OrderByDescending(r => r.Id)
            .ToListAsync(ct);
    }

    public async Task<List<RentalListDto>> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await MapToRentalListDto(
                _db.Rentals
                    .AsNoTracking()
                    .Where(r => r.UserId == userId)
            )
            .OrderByDescending(r => r.Id)
            .ToListAsync(ct);
    }

    public async Task<Rental> RequestAsync(RequestRentalDto dto, CancellationToken ct = default)
    {
        var car = await _db.Cars.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == dto.CarId, ct);

        if (car is null)
            throw new ArgumentException("Invalid CarId.");

        if (dto.StartDate >= dto.EndDate)
            throw new ArgumentException("StartDate must be before EndDate.");

        if (car.UnavailableFrom.HasValue && car.UnavailableTo.HasValue)
        {
            var unavailableOverlap =
                dto.StartDate < car.UnavailableTo.Value &&
                dto.EndDate > car.UnavailableFrom.Value;

            if (unavailableOverlap)
            {
                var reason = car.UnavailableReason?.ToString() ?? "Unavailable";
                var note = string.IsNullOrWhiteSpace(car.UnavailableNote)
                    ? ""
                    : $" Note: {car.UnavailableNote}";

                throw new ArgumentException(
                    $"Car is unavailable due to {reason} from {car.UnavailableFrom:yyyy-MM-dd} to {car.UnavailableTo:yyyy-MM-dd}.{note}"
                );
            }
        }

        var rentalOverlap = await _db.Rentals.AnyAsync(r =>
            r.CarId == dto.CarId &&
            r.Status != CarRentStatus.Rejected &&
            r.Status != CarRentStatus.Returned &&
            dto.StartDate < r.EndDate &&
            dto.EndDate > r.StartDate,
            ct
        );

        if (rentalOverlap)
            throw new ArgumentException("Car is already booked for the selected period.");

        var isGuest = dto.UserId is null;

        if (isGuest)
        {
            if (string.IsNullOrWhiteSpace(dto.GuestName) ||
                string.IsNullOrWhiteSpace(dto.GuestEmail) ||
                string.IsNullOrWhiteSpace(dto.GuestPhone))
            {
                throw new ArgumentException("GuestName, GuestEmail and GuestPhone are required for guest rentals.");
            }
        }
        else
        {
            var userExists = await _db.Users.AnyAsync(u => u.Id == dto.UserId, ct);

            if (!userExists)
                throw new ArgumentException("Invalid UserId.");
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
}