namespace CarRental.Infrastructure.Persistence.Seeding;

public interface IAppSeedStep
{
    int Order { get; }
    string Name { get; }
    Task SeedAsync(CancellationToken cancellationToken = default);
}
