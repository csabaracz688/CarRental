using CarRental.Application.Common.Exceptions;
using CarRental.Application.Common.Interfaces;
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
    public async Task<IActionResult> GetAll(CancellationToken ct =default) => Ok(await _rentals.GetAllAsync(ct));

    [HttpPost("request")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateRequest([FromBody] RequestRentalDto dto, CancellationToken ct)
        => Ok(await _rentals.RequestAsync(dto, ct));

    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, [FromQuery] int? approvedByUserId, CancellationToken ct)
    {
        var actingUserId = TryGetCurrentUserId(User) ?? approvedByUserId;
        if (actingUserId is null)
        {
            return BadRequest("Approver user id is required.");
        }

        return await _rentals.ApproveAsync(id, actingUserId.Value, ct) ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(int id, [FromQuery] int? approvedByUserId, CancellationToken ct)
    {
        var actingUserId = TryGetCurrentUserId(User) ?? approvedByUserId;
        if (actingUserId is null)
        {
            return BadRequest("Approver user id is required.");
        }

        return await _rentals.RejectAsync(id, actingUserId.Value, ct) ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/close")]
    public async Task<IActionResult> Close(int id, CancellationToken ct)
        => await _rentals.CloseAsync(id, ct) ? NoContent() : NotFound();

    private static int? TryGetCurrentUserId(ClaimsPrincipal user)
    {
        var claimValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var parsed) ? parsed : null;
    }

    [HttpPost("{id:int}/return")]
    public async Task<IActionResult> Return(int id, CancellationToken ct)
    {
        try
        {
            await _rentals.ReturnRentalAsync(id, ct);
            return Ok();
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
        catch (ConflictException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

}