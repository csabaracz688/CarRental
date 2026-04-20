using CarRental.Application.Common.Exceptions;
using CarRental.Application.Common.Interfaces;
using CarRental.Application.Features;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Persistence;
using CarRental.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Infrastructure.Managers;

public sealed class AuthManager : IAuthManager
{
    private readonly CarRentalDbContext _db;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthManager(
        CarRentalDbContext db,
        IPasswordHashService passwordHashService,
        IJwtTokenService jwtTokenService)
    {
        _db = db;
        _passwordHashService = passwordHashService;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request, CancellationToken ct = default)
    {
        var email = request.Email.Trim();
        var userName = request.UserName.Trim();

        var emailTaken = await _db.Users.AnyAsync(u => u.Email == email, ct);
        if (emailTaken)
        {
            throw new AuthManagerException("Email is already in use.", 409);
        }

        var userNameTaken = await _db.Users.AnyAsync(u => u.UserName == userName, ct);
        if (userNameTaken)
        {
            throw new AuthManagerException("Username is already in use.", 409);
        }

        var customerRole = await _db.Roles.FirstOrDefaultAsync(r => r.RoleType == RoleTypes.Customer, ct);
        if (customerRole is null)
        {
            throw new AuthManagerException("Customer role is missing from the database.", 500);
        }

        var user = new Users
        {
            UserName = userName,
            Email = email,
            Password = _passwordHashService.HashPassword(request.Password),
            FirstName = request.FirstName?.Trim(),
            LastName = request.LastName?.Trim(),
            Address = request.Address?.Trim(),
            Phone = request.Phone?.Trim(),
            Roles = new List<Role> { customerRole }
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        var token = _jwtTokenService.CreateToken(user, RoleTypes.Customer);
        return new AuthResponseDto(token, RoleTypes.Customer.ToString(), user.Id, user.UserName);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request, CancellationToken ct = default)
    {
        var email = request.Email.Trim();

        var user = await _db.Users
            .Include(u => u.Roles)
            .SingleOrDefaultAsync(u => u.Email == email, ct);

        if (user is null || !_passwordHashService.VerifyPassword(request.Password, user.Password))
        {
            throw new AuthManagerException("Invalid email or password.", 401);
        }

        if (user.Roles is null || !user.Roles.Any())
        {
            throw new AuthManagerException("User has no roles assigned.", 401);
        }

        var role = user.Roles
            .Select(r => r.RoleType)
            .OrderByDescending(r => r)
            .First();

        var token = _jwtTokenService.CreateToken(user, role);
        return new AuthResponseDto(token, role.ToString(), user.Id, user.UserName);
    }
}
