using Bogus;
using Domain.Entities;
using Domain.Models;

namespace ProductServiceIntegrationTests.Fakers;

public sealed class ProductFaker : Faker<Product>
{
    private static readonly string[] ProductNames = new[] { "Bread", "Computer", "Tea", "Phone" };
    public ProductFaker()
    {
        RuleFor(o => o.Id, f => f.Random.ULong(100000000000, 999999999999));
        RuleFor(o => o.WarehouseId, f => f.Random.ULong(100000000000, 999999999999));
        RuleFor(o => o.Price, f => f.Random.Double(0, double.MaxValue));
        RuleFor(o => o.Weight, f => f.Random.Double(0, double.MaxValue));
        RuleFor(o => o.Category, f => f.PickRandom<ProductCategory>());
        RuleFor(o => o.Name, f => f.PickRandom(ProductNames));
        RuleFor(o => o.DateOfCreation, _ => DateTime.UtcNow);
    }
}