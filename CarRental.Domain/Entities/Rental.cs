using CarRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Domain.Entities;

public class Rental
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public Users? User { get; set; }

    public string? GuestName { get; set; }
    public string? GuestEmail { get; set; }
    public string? GuestPhone { get; set; }
    public int CarId { get; set; }
    public Car Car { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public CarRentStatus Status { get; set; }
    public int? ApprovedByUserId { get; set; }
    public Users? ApprovedByUser { get; set; }
    public DateTime? HandedOverAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public Invoice? Invoice { get; set; }
}
