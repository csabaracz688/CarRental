using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Persistence;
using CarRental.Infrastructure.Persistence.Seeding.Models;
using CarRental.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure.Persistence.Seeding;

public sealed class UserSeedStep : IAppSeedStep
{
    private readonly CarRentalDbContext _db;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<UserSeedStep> _logger;

    private static readonly SeedUserModel[] SeedUsers =
    [
        new("admin", "admin@carrental.local", "Admin123!", "System", "Admin", "HQ Center 1", "+3610000001", RoleTypes.Admin),
        new("officer", "officer@carrental.local", "Officer123!", "Olivia", "Officer", "Service Desk 12", "+3610000002", RoleTypes.Officer),
        new("customer", "customer@carrental.local", "Customer123!", "Chris", "Customer", "Main Street 42", "+3610000003", RoleTypes.Customer)
    ];

    public UserSeedStep(CarRentalDbContext db, IPasswordHashService passwordHashService, ILogger<UserSeedStep> logger)
    {
        _db = db;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    public int Order => 20;
    public string Name => "Users";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var roles = await _db.Roles.ToDictionaryAsync(role => role.RoleType, cancellationToken);

        var createdCount = 0;
        var updatedCount = 0;

        foreach (var seedUser in SeedUsers)
        {
            if (!roles.TryGetValue(seedUser.RoleType, out var role))
            {
                throw new InvalidOperationException($"Cannot seed users because role '{seedUser.RoleType}' does not exist.");
            }

            var existingUser = await _db.Users
                .Include(user => user.Roles)
                .SingleOrDefaultAsync(user => user.Email == seedUser.Email, cancellationToken);

            if (existingUser is null)
            {
                var newUser = new Users
                {
                    UserName = seedUser.UserName,
                    Email = seedUser.Email,
                    Password = _passwordHashService.HashPassword(seedUser.Password),
                    FirstName = seedUser.FirstName,
                    LastName = seedUser.LastName,
                    Address = seedUser.Address,
                    Phone = seedUser.Phone,
                    Roles = new List<Role> { role }
                };

                _db.Users.Add(newUser);
                createdCount++;
                continue;
            }

            var changed = false;

            if (!existingUser.Roles.Any(userRole => userRole.RoleType == seedUser.RoleType))
            {
                existingUser.Roles.Add(role);
                changed = true;
            }

            if (changed)
            {
                updatedCount++;
            }
        }

        if (createdCount > 0 || updatedCount > 0)
        {
            await _db.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation(
            "User seed finished. Created: {CreatedCount}, Updated: {UpdatedCount}, Skipped: {SkippedCount}.",
            createdCount,
            updatedCount,
            SeedUsers.Length - createdCount - updatedCount);
    }

}
