using CarRental.Application.Common.Interfaces;
using CarRental.Application.Features;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Managers;

public class CarManager : ICarManager
{
    private readonly CarRentalDbContext _db;

    public CarManager(CarRentalDbContext db) => _db = db;
    private static readonly CarRentStatus[] BlockingStatuses =
    {
        CarRentStatus.Requested,
        CarRentStatus.Approved,
        CarRentStatus.Handed
    };

    private static string? BuildImageUrl(string? imagePath)
        => imagePath != null
            ? $"https://localhost:7077/uploads/{imagePath}"
            : null;

    public async Task<List<CarResponseDto>> GetAllAsync(CancellationToken ct = default)
        => await _db.Cars.AsNoTracking()
            .OrderBy(c => c.Id)
            .Select(c => new CarResponseDto
            {
                Id = c.Id,
                LicensePlate = c.LicensePlate,
                Brand = c.Brand,
                Model = c.Model,
                DistanceKm = c.DistanceKm,
                DailyPrice = c.DailyPrice,
                Status = (int)c.Status,

                ImageUrl = BuildImageUrl(c.ImagePath),

                UnavailableFrom = c.UnavailableFrom,
                UnavailableTo = c.UnavailableTo,
                UnavailableReason = c.UnavailableReason == null ? null : (int)c.UnavailableReason,
                UnavailableNote = c.UnavailableNote
            })
            .ToListAsync(ct);

    public async Task<CarResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Cars.AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CarResponseDto
            {
                Id = c.Id,
                LicensePlate = c.LicensePlate,
                Brand = c.Brand,
                Model = c.Model,
                DistanceKm = c.DistanceKm,
                DailyPrice = c.DailyPrice,
                Status = (int)c.Status,

                ImageUrl = BuildImageUrl(c.ImagePath),

                UnavailableFrom = c.UnavailableFrom,
                UnavailableTo = c.UnavailableTo,
                UnavailableReason = c.UnavailableReason == null ? null : (int)c.UnavailableReason,
                UnavailableNote = c.UnavailableNote
            })
            .FirstOrDefaultAsync(ct);

    public async Task<CarResponseDto> CreateAsync(CreateCarDto dto, CancellationToken ct = default)
    {
        string? fileName = null;

        if (dto.Image != null)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await dto.Image.CopyToAsync(stream, ct);
        }

        var car = new Car
        {
            LicensePlate = dto.LicensePlate,
            Brand = dto.Brand,
            Model = dto.Model,
            DistanceKm = dto.DistanceKm,
            DailyPrice = dto.DailyPrice,
            Status = (CarStatus)dto.Status,
            ImagePath = fileName,

            UnavailableFrom = dto.UnavailableFrom,
            UnavailableTo = dto.UnavailableTo,
            UnavailableReason = dto.UnavailableReason == null
                ? null
                : (CarUnavailableReason)dto.UnavailableReason.Value,
            UnavailableNote = dto.UnavailableNote
        };

        _db.Cars.Add(car);
        await _db.SaveChangesAsync(ct);

        return new CarResponseDto
        {
            Id = car.Id,
            LicensePlate = car.LicensePlate,
            Brand = car.Brand,
            Model = car.Model,
            DistanceKm = car.DistanceKm,
            DailyPrice = car.DailyPrice,
            Status = (int)car.Status,

            ImageUrl = BuildImageUrl(car.ImagePath),

            UnavailableFrom = car.UnavailableFrom,
            UnavailableTo = car.UnavailableTo,
            UnavailableReason = car.UnavailableReason == null ? null : (int)car.UnavailableReason,
            UnavailableNote = car.UnavailableNote
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateCarDto dto, CancellationToken ct = default)
    {
        if (dto.Id.HasValue && dto.Id.Value != id)
            throw new ArgumentException("Route id and body id do not match.");

        var car = await _db.Cars.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (car is null) return false;

        car.LicensePlate = dto.LicensePlate;
        car.Brand = dto.Brand;
        car.Model = dto.Model;
        car.DistanceKm = dto.DistanceKm;
        car.DailyPrice = dto.DailyPrice;
        car.Status = (CarStatus)dto.Status;

        car.UnavailableFrom = dto.UnavailableFrom;
        car.UnavailableTo = dto.UnavailableTo;
        car.UnavailableReason = dto.UnavailableReason == null
            ? null
            : (CarUnavailableReason)dto.UnavailableReason.Value;
        car.UnavailableNote = dto.UnavailableNote;

        if (dto.Image != null)
        {
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
            var path = Path.Combine(folder, fileName);

            using var stream = new FileStream(path, FileMode.Create);
            await dto.Image.CopyToAsync(stream, ct);

            car.ImagePath = fileName;
        }

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var car = await _db.Cars.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (car is null) return false;

        // kép törlése
        if (!string.IsNullOrEmpty(car.ImagePath))
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", car.ImagePath);
            if (File.Exists(path))
                File.Delete(path);
        }

        _db.Cars.Remove(car);
        await _db.SaveChangesAsync(ct);

        return true;
    }

    public async Task<List<CarSearchResultDto>> SearchAsync(
    CarSearchRequestDto request,
    CancellationToken cancellationToken)
    {

        return await _db.Cars
            .AsNoTracking()
            .Select(car => new
            {
                car,
                HasRentalOverlap = car.Rentals.Any(r =>
                    BlockingStatuses.Contains(r.Status) &&
                    r.StartDate < request.EndDate &&
                    r.EndDate > request.StartDate),

                HasManualBlock =
                    car.UnavailableFrom != null &&
                    car.UnavailableTo != null &&
                    car.UnavailableFrom < request.EndDate &&
                    car.UnavailableTo > request.StartDate
            })
            .Select(x => new CarSearchResultDto
            {
                CarId = x.car.Id,
                LicensePlate = x.car.LicensePlate,
                Brand = x.car.Brand,
                Model = x.car.Model,
                DailyPrice = x.car.DailyPrice,

                IsAvailable = !(x.HasRentalOverlap
                                || x.HasManualBlock
                                || x.car.Status != CarStatus.Available),

                NextAvailableFrom = x.HasManualBlock
                    ? x.car.UnavailableTo
                    : x.HasRentalOverlap
                        ? x.car.Rentals
                            .Where(r =>
                                BlockingStatuses.Contains(r.Status) &&
                                r.StartDate < request.EndDate &&
                                r.EndDate > request.StartDate)
                            .Select(r => (DateTime?)r.EndDate)
                            .DefaultIfEmpty()
                            .Max()
                        : null,
                Reason = (x.car.Status != CarStatus.Available || x.HasManualBlock)
                    ? x.car.UnavailableReason
                    : null
            })
            .ToListAsync(cancellationToken);
    }

}
