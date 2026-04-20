using CarRental.Application.Users;
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
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim == null)
            return Unauthorized();

        var userId = int.Parse(userIdClaim);

        if(!string.Equals(role, "CUSTOMER", StringComparison.OrdinalIgnoreCase) || userId != id)
            return Forbid();

        var user = await _userManager.GetByIdAsync(id);

        if (user == null) return NotFound();

        return Ok(new
        {
            user.Id,
            user.UserName,
            user.Email,
            user.Address,
            user.Phone
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserProfileDto dto)
    {
        await _userManager.UpdateProfileAsync(id, dto);
        return NoContent();
    }

}