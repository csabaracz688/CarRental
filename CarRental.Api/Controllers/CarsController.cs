using CarRental.Application.Common.Interfaces;
using CarRental.Application.Features;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace CarRental.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarManager _cars;
    private readonly CarRentalDbContext _db;

    public CarsController(ICarManager cars, CarRentalDbContext db)
    {
        _cars = cars;
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll() => Ok(await _cars.GetAllAsync());

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var car = await _cars.GetByIdAsync(id);
        return car is null ? NotFound() : Ok(car);
    }

    [HttpPost]
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public async Task<IActionResult> Create([FromForm] CreateCarDto dto)
    {

        var created = await _cars.CreateAsync(dto);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCarDto dto)
        => await _cars.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    [Authorize(Roles = nameof(RoleTypes.Admin))]
    public async Task<IActionResult> Delete(int id)
        => await _cars.DeleteAsync(id) ? NoContent() : NotFound();

    // GET: api/cars/{id}/availability?start=2026-03-11&end=2026-03-14
    [HttpGet("{id:int}/availability")]
    [AllowAnonymous]
    public async Task<IActionResult> Availability(
      int id,
      [FromQuery] DateTime start,
      [FromQuery] DateTime end)
    {
        var car = await _db.Cars
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (car is null)
            return NotFound();

        if (start >= end)
            return BadRequest("Start date must be before end date.");

        var unavailableOverlap =
            car.UnavailableFrom.HasValue &&
            car.UnavailableTo.HasValue &&
            start < car.UnavailableTo.Value &&
            end > car.UnavailableFrom.Value;

        var rentalOverlap = await _db.Rentals.AnyAsync(r =>
            r.CarId == id &&
            r.Status != CarRentStatus.Rejected &&
            r.Status != CarRentStatus.Returned &&
            start < r.EndDate &&
            end > r.StartDate
        );

        var dto = new CarAvailabilityDto
        {
            IsAvailable = !unavailableOverlap && !rentalOverlap,
            Reason = unavailableOverlap
                ? car.UnavailableReason?.ToString() ?? "Unavailable"
                : rentalOverlap
                    ? "Already rented for this period"
                    : null,
            UnavailableFrom = car.UnavailableFrom,
            UnavailableTo = car.UnavailableTo,
            NextAvailableFrom = car.UnavailableTo
        };

        return Ok(dto);
    }

    [HttpGet("{id:int}/rentals")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCarRentals(int id)
    {
        var carExists = await _db.Cars.AnyAsync(c => c.Id == id);

        if (!carExists)
            return NotFound();

        var rentals = await _db.Rentals
            .AsNoTracking()
            .Where(r =>
                r.CarId == id &&
                r.Status != CarRentStatus.Rejected &&
                r.Status != CarRentStatus.Returned
            )
            .Select(r => new
            {
                startDate = r.StartDate,
                endDate = r.EndDate,
                status = (int)r.Status
            })
            .ToListAsync();

        return Ok(rentals);
    }
    [HttpGet("search")]
    public async Task<IActionResult> Search(
    [FromQuery] DateTime start,
    [FromQuery] DateTime end,
    CancellationToken cancellationToken)
    {
        if (end <= start)
        {
            return BadRequest("End date must be greater than start date.");
        }

        var result = await _cars.SearchAsync(
            new CarSearchRequestDto
            {
                StartDate = start,
                EndDate = end
            },
            cancellationToken);

        return Ok(result);
    }


}