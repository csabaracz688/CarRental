using CarRental.Application.Features;

namespace CarRental.Application.Common.Interfaces;

public interface ICarManager
{
    Task<List<CarResponseDto>> GetAllAsync(CancellationToken ct = default);
    Task<CarResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CarResponseDto> CreateAsync(CreateCarDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, UpdateCarDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
    Task<List<CarSearchResultDto>> SearchAsync(CarSearchRequestDto dto, CancellationToken ct = default);
}