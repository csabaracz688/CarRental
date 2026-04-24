using CarRental.Application.Features;
using CarRental.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRental.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{nameof(RoleTypes.Admin)},{nameof(RoleTypes.Officer)}")]
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
    [AllowAnonymous]
    public async Task<IActionResult> CreateRequest([FromBody] RequestRentalDto dto)
        => Ok(await _rentals.RequestAsync(dto));

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, [FromQuery] int? approvedByUserId)
    {
        var actingUserId = TryGetCurrentUserId(User) ?? approvedByUserId;
        if (actingUserId is null)
        {
            return BadRequest("Approver user id is required.");
        }

        return await _rentals.ApproveAsync(id, actingUserId.Value) ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, [FromQuery] int? approvedByUserId)
    {
        var actingUserId = TryGetCurrentUserId(User) ?? approvedByUserId;
        if (actingUserId is null)
        {
            return BadRequest("Approver user id is required.");
        }

        return await _rentals.RejectAsync(id, actingUserId.Value) ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/close")]
    public async Task<IActionResult> Close(int id)
        => await _rentals.CloseAsync(id) ? NoContent() : NotFound();

    private static int? TryGetCurrentUserId(ClaimsPrincipal user)
    {
        var claimValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var parsed) ? parsed : null;
    }
}