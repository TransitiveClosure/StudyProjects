using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using TaskManagementSystem.Dal.Extensions;
using TaskManagementSystem.Dal.Repositories.Interfaces;
using TaskManagementSystem.Utilities.Extensions;
using TaskManagementSystem.Utilities.Providers.Interfaces;

namespace TaskManagementSystem.IntegrationTests.Fixtures
{
    public class TestFixture
    {
        public IUserRepository UserRepository { get; }
        
        public ITaskRepository TaskRepository { get; }
        
        public ITaskLogRepository TaskLogRepository { get; }
        
        public ITakenTaskRepository TakenTaskRepository { get; }
        
        public IUserScheduleRepository UserScheduleRepository { get; }
        
        public ITaskCommentRepository TaskCommentRepository { get; }
        
        public IRateLimiterRepository RateLimiterRepository { get; }
        
        public Mock<IDateTimeProvider> DateTimeProviderFake { get; } = new(MockBehavior.Strict);

        public TestFixture()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var host = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddDalInfrastructure(config)
                        .AddDalRepositories()
                        .AddUtilities();
                    services.Replace(new ServiceDescriptor(typeof(IDateTimeProvider),
                        DateTimeProviderFake.Object));
                })
                .Build();
            
            ClearDatabase(host);
            host.MigrateUp();

            var scope = host.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            UserRepository = serviceProvider.GetRequiredService<IUserRepository>();
            TaskRepository = serviceProvider.GetRequiredService<ITaskRepository>();
            TaskLogRepository = serviceProvider.GetRequiredService<ITaskLogRepository>();
            TakenTaskRepository = serviceProvider.GetRequiredService<ITakenTaskRepository>();
            UserScheduleRepository = serviceProvider.GetRequiredService<IUserScheduleRepository>();
            TaskCommentRepository = serviceProvider.GetRequiredService<ITaskCommentRepository>();
            RateLimiterRepository = serviceProvider.GetRequiredService<IRateLimiterRepository>();
                
            FluentAssertionOptions.UseDefaultPrecision();
        }

        private static void ClearDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateDown(0);
        }
    }
}