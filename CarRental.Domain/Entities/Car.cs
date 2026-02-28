using CarRental.Domain.Enums;
using System;
using System.Collections.Generic;
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

    public CarStatus Status { get; set; }
    public ICollection<Rental> Rentals { get; set; } = new List<Rental>();


}
