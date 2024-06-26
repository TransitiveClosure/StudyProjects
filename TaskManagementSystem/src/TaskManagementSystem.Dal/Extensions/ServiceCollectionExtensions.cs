using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Dal.Infrastructure;
using TaskManagementSystem.Dal.Repositories;
using TaskManagementSystem.Dal.Repositories.Interfaces;
using TaskManagementSystem.Dal.Settings;

namespace TaskManagementSystem.Dal.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDalRepositories(
        this IServiceCollection services)
    {
        AddPostgresRepositories(services);
        AddRedisRepositories(services);

        return services;
    }

    private static void AddRedisRepositories(IServiceCollection services)
    {
        services.AddScoped<ITakenTaskRepository, TakenTaskRepository>();
        services.AddScoped<IUserScheduleRepository, UserScheduleRepository>();
        services.AddScoped<IRateLimiterRepository, RateLimiterRepository>();
    }

    private static void AddPostgresRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskLogRepository, TaskLogRepository>();
        services.AddScoped<ITaskCommentRepository, TaskCommentRepository>();
    }

    public static IServiceCollection AddDalInfrastructure(
        this IServiceCollection services,
        IConfigurationRoot config)
    {
        services.Configure<DalOptions>(config.GetSection(nameof(DalOptions)));

        Postgres.MapCompositeTypes();

        Postgres.AddMigrations(services);

        return services;
    }
}