using Domain.Models;

namespace Domain.Entities;

public record Product
{
    public required ulong Id { get; init; }
    public required string Name { get; init; }
    public required double Price { get; init; }
    public required double Weight { get; init; }
    public required ProductCategory Category { get; init; }
    public required DateTime DateOfCreation { get; init; }
    public required ulong WarehouseId { get; init; }
}