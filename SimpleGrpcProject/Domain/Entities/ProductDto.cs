using Domain.Models;

namespace Domain.Entities;

public record ProductDto
{
    public required string Name { get; init; }
    public required double Price { get; set; }
    public required double Weight { get; init; }
    public required ProductCategory Category { get; init; }
    public required DateTime DateOfCreation { get; init; }
    public required ulong WarehouseId { get; init; }
}