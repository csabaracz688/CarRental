using CarRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarRental.Domain.Entities;

public class Car
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int DistanceKm { get; set; }
    public int DailyPrice { get; set; }
    public DateTime? UnavailableFrom { get; set; }
    public DateTime? UnavailableTo { get; set; }
    public CarUnavailableReason? UnavailableReason { get; set; }
    [MaxLength(500)]
    public string? UnavailableNote { get; set; }
    public string? ImagePath { get; set; }

    public CarStatus Status { get; set; }
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();


}
