namespace CarRental.Infrastructure.Persistence.Seeding;

public interface IAppDbInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
