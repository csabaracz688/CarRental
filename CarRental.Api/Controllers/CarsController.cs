using CarRental.Application.Common.Interfaces;
using CarRental.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CarsController : ControllerBase
{
    private readonly ICarManager _cars;

    public CarsController(ICarManager cars)
    {
        _cars = cars;
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
    public async Task<IActionResult> Create([FromBody] Car car)
    {
        var created = await _cars.CreateAsync(car);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] Car updated)
        => await _cars.UpdateAsync(id, updated) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
        => await _cars.DeleteAsync(id) ? NoContent() : NotFound();
}