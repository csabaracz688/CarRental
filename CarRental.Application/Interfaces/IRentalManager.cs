using CarRental.Application.Features;
using CarRental.Domain.Entities;

namespace CarRental.Application.Common.Interfaces;

public interface IRentalManager
{
    Task<List<RentalListDto>> GetAllAsync(CancellationToken ct = default);

    Task<List<RentalListDto>> GetPendingAsync(CancellationToken ct = default);

    Task<Rental> RequestAsync(RequestRentalDto dto, CancellationToken ct = default);

    Task<bool> ApproveAsync(int rentalId, int approvedByUserId, CancellationToken ct = default);

    Task<bool> RejectAsync(int rentalId, int approvedByUserId, CancellationToken ct = default);

    Task<bool> CloseAsync(int rentalId, CancellationToken ct = default);

    Task ReturnRentalAsync(int rentalId, CancellationToken ct = default);

    Task<List<Rental>> GetByUserIdAsync(int userId, CancellationToken ct = default);
}
