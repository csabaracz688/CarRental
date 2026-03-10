using System;
using System.Collections.Generic;
using System.Text;

namespace CarRental.Application.Features;


//jelenleg nincs hasznalatban, de a mappingolashoz jol johet kesobb(vagy kitoroljuk majd)
public class CreateCarDto
{
    public string LicensePlate { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int DistanceKm { get; set; }
    public int DailyPrice { get; set; }
    public int Status { get; set; }

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
}
