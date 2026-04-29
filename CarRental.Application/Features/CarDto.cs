using CarRental.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace CarRental.Application.Features;


public class CreateCarDto
{
    public string LicensePlate { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int DistanceKm { get; set; }
    public int DailyPrice { get; set; }
    public int Status { get; set; }
    public DateTime? UnavailableFrom { get; set; }
    public DateTime? UnavailableTo { get; set; }
    public int? UnavailableReason { get; set; }
    public string? UnavailableNote { get; set; }
    public IFormFile? Image { get; set; }
    public string? ImagePath { get; set; }

}
public class UpdateCarDto
{
    public int? Id { get; set; } 
    public string LicensePlate { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int DistanceKm { get; set; }
    public int DailyPrice { get; set; }
    public int Status { get; set; }
    public DateTime? UnavailableFrom { get; set; }
    public DateTime? UnavailableTo { get; set; }
    public int? UnavailableReason { get; set; }
    public string? UnavailableNote { get; set; }
    public IFormFile? Image { get; set; }
}
public class CarResponseDto
{
    public int Id { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int DistanceKm { get; set; }
    public int DailyPrice { get; set; }
    public int Status { get; set; }
    public DateTime? UnavailableFrom { get; set; }
    public DateTime? UnavailableTo { get; set; }
    public int? UnavailableReason { get; set; }
    public string? UnavailableNote { get; set; }
    public string? ImageUrl { get; set; }

}
public class CarAvailabilityDto
{
    public bool IsAvailable { get; set; }
    public string? Reason { get; set; }
    public DateTime? UnavailableFrom { get; set; }
    public DateTime? UnavailableTo { get; set; }
    public DateTime? NextAvailableFrom { get; set; }
}

public class CarSearchResultDto
{
    public int CarId { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int DailyPrice { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime? NextAvailableFrom { get; set; }

    public CarUnavailableReason? Reason { get; set; }
}

public class CarSearchRequestDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}