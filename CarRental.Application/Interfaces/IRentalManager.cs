using CarRental.Domain.Entities;

namespace CarRental.Application.Common.Interfaces;

public interface IRentalManager
{
    Task<List<Rental>> GetAllAsync(CancellationToken ct = default);
    Task<Rental> RequestAsync(RequestRentalDto dto, CancellationToken ct = default);
    Task<bool> ApproveAsync(int rentalId, int approvedByUserId, CancellationToken ct = default);
    Task<bool> RejectAsync(int rentalId, int approvedByUserId, CancellationToken ct = default);
    Task<bool> CloseAsync(int rentalId, CancellationToken ct = default);
}

public class RequestRentalDto
{
    public int CarId { get; set; }
    public int? UserId { get; set; }
    public string? GuestName { get; set; }
    public string? GuestEmail { get; set; }
    public string? GuestPhone { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}