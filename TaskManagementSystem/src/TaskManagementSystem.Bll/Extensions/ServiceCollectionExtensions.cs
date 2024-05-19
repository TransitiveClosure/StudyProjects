using Microsoft.Extensions.DependencyInjection;
using TaskManagementSystem.Bll.Services;
using TaskManagementSystem.Bll.Services.Interfaces;

namespace TaskManagementSystem.Bll.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBllServices(
        this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITaskCommentService, TaskCommentService>();
        services.AddScoped<IRateLimiterService, RateLimiterService>();

        return services;
    }
}