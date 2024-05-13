using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess;

public static class RepositoryServiceCollectionExtension
{
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IProductIdGenerator, ProductIdGenerator>();
        serviceCollection.AddSingleton<IProductRepository, InMemoryProductRepository>();
        return serviceCollection;
    }
}