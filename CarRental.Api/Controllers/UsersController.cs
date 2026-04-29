using CarRental.Application.Users;
using CarRental.Domain.Constants;
using CarRental.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CarRental.WebApi.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserManager _userManager;

    public UsersController(IUserManager userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{RoleConstants.Customer},{RoleConstants.Admin}")]
    public async Task<IActionResult> Get(int id)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        if (userId != id)
            return Forbid();

        var user = await _userManager.GetByIdAsync(id);

        if (user == null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.Address,
            user.PostalCode,
            user.City,
            user.Phone
        });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{RoleConstants.Customer},{RoleConstants.Admin}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserProfileDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        if (userId != id)
            return Forbid();
        await _userManager.UpdateProfileAsync(id, dto);
        return NoContent();
    }
}