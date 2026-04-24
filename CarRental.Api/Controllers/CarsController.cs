using CarRental.Application.Common.Interfaces;
using CarRental.Application.Features;
using CarRental.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<IActionResult> GetAll() => Ok(await _cars.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var car = await _cars.GetByIdAsync(id);
        return car is null ? NotFound() : Ok(car);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCarDto dto)
    {
        var created = await _cars.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCarDto dto)
        => await _cars.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _cars.DeleteAsync(id) ? NoContent() : NotFound();

    // GET: api/cars/{id}/availability?start=2026-03-11&end=2026-03-14
    [HttpGet("{id:int}/availability")]
    public async Task<IActionResult> Availability(int id, [FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var car = await _db.Cars.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (car is null) return NotFound();

        var dto = new CarAvailabilityDto
        {
            IsAvailable = true,
            Reason = null,
            UnavailableFrom = car.UnavailableFrom,
            UnavailableTo = car.UnavailableTo,
            NextAvailableFrom = null
        };

        if (car.UnavailableFrom.HasValue && car.UnavailableTo.HasValue)
        {
            var overlaps =
                start < car.UnavailableTo.Value &&
                end > car.UnavailableFrom.Value;

            if (overlaps)
            {
                dto.IsAvailable = false;
                dto.Reason = car.UnavailableReason?.ToString() ?? "Unavailable";
                dto.NextAvailableFrom = car.UnavailableTo.Value;
            }
        }

        return Ok(dto);
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