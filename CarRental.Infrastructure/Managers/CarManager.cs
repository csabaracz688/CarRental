using CarRental.Application.Common.Interfaces;
using CarRental.Domain.Entities;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Managers;

public class CarManager : ICarManager
{
    private readonly CarRentalDbContext _db;

    public CarManager(CarRentalDbContext db)
    {
        _db = db;
    }

    public Task<List<Car>> GetAllAsync(CancellationToken ct = default) =>
        _db.Cars.AsNoTracking().OrderBy(c => c.Id).ToListAsync(ct);

    public Task<Car?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _db.Cars.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Car> CreateAsync(Car car, CancellationToken ct = default)
    {
        _db.Cars.Add(car);
        await _db.SaveChangesAsync(ct);
        return car;
    }

    public async Task<bool> UpdateAsync(int id, Car updated, CancellationToken ct = default)
    {
        var car = await _db.Cars.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (car is null) return false;

        car.LicensePlate = updated.LicensePlate;
        car.Brand = updated.Brand;
        car.Model = updated.Model;
        car.DistanceKm = updated.DistanceKm;
        car.DailyPrice = updated.DailyPrice;
        car.Status = updated.Status;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var car = await _db.Cars.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (car is null) return false;

        _db.Cars.Remove(car);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}