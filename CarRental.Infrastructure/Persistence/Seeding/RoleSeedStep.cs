using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using CarRental.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure.Persistence.Seeding;

public sealed class RoleSeedStep : IAppSeedStep
{
    private readonly CarRentalDbContext _db;
    private readonly ILogger<RoleSeedStep> _logger;

    public RoleSeedStep(CarRentalDbContext db, ILogger<RoleSeedStep> logger)
    {
        _db = db;
        _logger = logger;
    }

    public int Order => 10;
    public string Name => "Roles";

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var existingRoleTypes = await _db.Roles
            .AsNoTracking()
            .Select(role => role.RoleType)
            .ToListAsync(cancellationToken);

        var existingSet = existingRoleTypes.ToHashSet();

        var missingRoles = Enum
            .GetValues<RoleTypes>()
            .Where(roleType => !existingSet.Contains(roleType))
            .Select(roleType => new Role { RoleType = roleType })
            .ToList();

        if (missingRoles.Count == 0)
        {
            _logger.LogInformation("Role seed skipped. All role types already exist.");
            return;
        }

        _db.Roles.AddRange(missingRoles);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Role seed created {Count} roles.", missingRoles.Count);
    }
}
