using Microsoft.Extensions.Logging;

namespace CarRental.Infrastructure.Persistence.Seeding;

public sealed class AppDbInitializer : IAppDbInitializer
{
    private readonly IEnumerable<IAppSeedStep> _seedSteps;
    private readonly ILogger<AppDbInitializer> _logger;

    public AppDbInitializer(IEnumerable<IAppSeedStep> seedSteps, ILogger<AppDbInitializer> logger)
    {
        _seedSteps = seedSteps;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        var orderedSteps = _seedSteps.OrderBy(step => step.Order).ToList();

        _logger.LogInformation("Database initialization started with {Count} seed steps.", orderedSteps.Count);

        foreach (var step in orderedSteps)
        {
            _logger.LogInformation("Running seed step: {SeedStepName}.", step.Name);
            await step.SeedAsync(cancellationToken);
        }

        _logger.LogInformation("Database initialization finished.");
    }
}
