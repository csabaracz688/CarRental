using CarRental.Application.Common.Interfaces;
using CarRental.Application.Features;
using CarRental.Infrastructure.Managers;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentalsController : ControllerBase
{
    private readonly IRentalManager _rentals;

    public RentalsController(IRentalManager rentals)
    {
        _rentals = rentals;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _rentals.GetAllAsync());

    [HttpPost("request")]
    public async Task<IActionResult> Request([FromBody] RequestRentalDto dto)
    {
        try
        {
            var result = await _rentals.RequestRentalAsync(dto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                error = ex.Message
            });
        }
    }


    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, [FromQuery] int approvedByUserId)
        => await _rentals.ApproveAsync(id, approvedByUserId) ? NoContent() : NotFound();

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, [FromQuery] int approvedByUserId)
        => await _rentals.RejectAsync(id, approvedByUserId) ? NoContent() : NotFound();

    [HttpPost("{id:int}/close")]
    public async Task<IActionResult> Close(int id)
        => await _rentals.CloseAsync(id) ? NoContent() : NotFound();

    
}