using CarRental.Application.Features;
using CarRental.Domain.Entities;

namespace CarRental.Application.Common.Interfaces;

public interface IRentalManager
{
    Task<List<Rental>> GetAllAsync(CancellationToken ct = default);
    Task<Rental> RequestAsync(RequestRentalDto dto, CancellationToken ct = default);
    Task<bool> ApproveAsync(int rentalId, int approvedByUserId, CancellationToken ct = default);
    Task<bool> RejectAsync(int rentalId, int approvedByUserId, CancellationToken ct = default);
    Task<bool> CloseAsync(int rentalId, CancellationToken ct = default);
    //Task<Rental> RequestRentalAsync(RequestRentalDto dto, CancellationToken ct = default) => RequestAsync(dto, ct);
}

