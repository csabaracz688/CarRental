using CarRental.Domain.Entities;

namespace CarRental.Application.Common.Interfaces;

public interface ICarManager
{
    Task<List<Car>> GetAllAsync(CancellationToken ct = default);
    Task<Car?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Car> CreateAsync(Car car, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, Car updated, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}