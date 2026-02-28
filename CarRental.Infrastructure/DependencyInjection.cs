using CarRental.Application.Common.Interfaces;
using CarRental.Infrastructure.Managers;
using Microsoft.Extensions.DependencyInjection;

namespace CarRental.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICarManager, CarManager>();
        services.AddScoped<IRentalManager, RentalManager>();
        return services;
    }
}