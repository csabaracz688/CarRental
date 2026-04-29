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
[Authorize]
public class RentalsController : ControllerBase
{
    private readonly IRentalManager _rentals;

    public RentalsController(IRentalManager rentals)
    {
        _rentals = rentals;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await _rentals.GetAllAsync());

    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(CancellationToken ct)
        => Ok(await _rentals.GetPendingAsync(ct));

    [HttpPost("request")]
    [AllowAnonymous]
    public async Task<IActionResult> CreateRequest([FromBody] RequestRentalDto dto)
    {
        try
        {
            var currentUserId = TryGetCurrentUserId(User);

            if (currentUserId.HasValue)
            {
                dto.UserId = currentUserId.Value;
                dto.GuestName = null;
                dto.GuestEmail = null;
                dto.GuestPhone = null;
            }

            var rental = await _rentals.RequestAsync(dto);
            return Ok(rental);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/approve")]
    [Authorize(Roles = $"{nameof(RoleTypes.Admin)},{nameof(RoleTypes.Officer)}")]
    public async Task<IActionResult> Approve(int id, [FromQuery] int? approvedByUserId)
    {
        var actingUserId = TryGetCurrentUserId(User) ?? approvedByUserId;
        if (actingUserId is null)
        {
            return BadRequest("Approver user id is required.");
        }

        try
        {
            return await _rentals.ApproveAsync(id, actingUserId.Value) ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:int}/reject")]
    [Authorize(Roles = $"{nameof(RoleTypes.Admin)},{nameof(RoleTypes.Officer)}")]
    public async Task<IActionResult> Reject(int id, [FromQuery] int? approvedByUserId)
    {
        var actingUserId = TryGetCurrentUserId(User) ?? approvedByUserId;
        if (actingUserId is null)
        {
            return BadRequest("Approver user id is required.");
        }

        try
        {
            return await _rentals.RejectAsync(id, actingUserId.Value) ? NoContent() : NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("{id:int}/close")]
    [Authorize(Roles = $"{nameof(RoleTypes.Admin)},{nameof(RoleTypes.Officer)}")]
    public async Task<IActionResult> Close(int id)
        => await _rentals.CloseAsync(id) ? NoContent() : NotFound();

    [HttpPost("{id:int}/return")]
    public async Task<IActionResult> Return(int id, CancellationToken ct)
    {
        try
        {
            await _rentals.ReturnRentalAsync(id, ct);
            return NoContent();
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

    [HttpGet("user-rentals")]
    [Authorize(Roles = $"{nameof(RoleTypes.Customer)},{nameof(RoleTypes.Admin)},{nameof(RoleTypes.Officer)}")]
    public async Task<IActionResult> GetUserRentals()
    {
        var userId = TryGetCurrentUserId(User);

        if (userId is null)
            return Unauthorized(new { message = "User is not authenticated." });

        var rentals = await _rentals.GetByUserIdAsync(userId.Value);

        return Ok(rentals);
    }

    private static int? TryGetCurrentUserId(ClaimsPrincipal user)
    {
        var claimValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(claimValue, out var parsed) ? parsed : null;
    }
}
