using CarRental.Application.Common.Exceptions;
using CarRental.Application.Common.Interfaces;
using CarRental.Application.Features;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthManager _authManager;

    public AuthController(IAuthManager authManager)
    {
        _authManager = authManager;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request, CancellationToken ct)
    {
        try
        {
            var result = await _authManager.RegisterAsync(request, ct);
            return Ok(result);
        }
        catch (AuthManagerException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request, CancellationToken ct)
    {
        try
        {
            var result = await _authManager.LoginAsync(request, ct);
            return Ok(result);
        }
        catch (AuthManagerException ex)
        {
            return StatusCode(ex.StatusCode, new { message = ex.Message });
        }
    }
}
