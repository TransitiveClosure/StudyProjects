using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Utilities.Providers;
using TaskManagementSystem.Utilities.Providers.Interfaces;

namespace TaskManagementSystem.Utilities.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddUtilities(
        this IServiceCollection services)
    {
        services.AddScoped<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}