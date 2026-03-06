using CarRental.Application.Common.Interfaces;
using CarRental.Infrastructure.Managers;
using CarRental.Infrastructure.Persistence.Seeding;
using CarRental.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace CarRental.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICarManager, CarManager>();
        services.AddScoped<IRentalManager, RentalManager>();

        services.AddScoped<IAppDbInitializer, AppDbInitializer>();
        services.AddScoped<IAppSeedStep, RoleSeedStep>();
        services.AddScoped<IAppSeedStep, UserSeedStep>();
        services.AddScoped<IAppSeedStep, CarSeedStep>();
        services.AddScoped<IAppSeedStep, RentalSeedStep>();
        services.AddScoped<IAppSeedStep, InvoiceSeedStep>();

        services.AddSingleton<IPasswordHashService, Pbkdf2PasswordHashService>();

        return services;
    }
}