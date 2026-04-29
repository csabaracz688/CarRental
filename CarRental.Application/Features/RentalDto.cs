using System;
using System.Collections.Generic;
using System.Text;
using CarRental.Domain.Enums;

namespace CarRental.Application.Features;

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

public class RentalListDto
{
    public int Id { get; set; }

    public int CarId { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string CarBrand { get; set; } = null!;
    public string CarModel { get; set; } = null!;

    public int? UserId { get; set; }
    public string? UserName { get; set; }

    public string? GuestName { get; set; }
    public string? GuestEmail { get; set; }
    public string? GuestPhone { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public CarRentStatus Status { get; set; }
    public int? ApprovedByUserId { get; set; }
    public string? ApprovedByUserName { get; set; }

    public DateTime? HandedOverAt { get; set; }
    public DateTime? ClosedAt { get; set; }
}