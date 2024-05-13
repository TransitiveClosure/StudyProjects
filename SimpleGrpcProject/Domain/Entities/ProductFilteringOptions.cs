using Domain.Models;

namespace Domain.Entities;

public record ProductFilteringOptions
{
    public required ProductCategory Category { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }
    public required ulong WarehouseId { get; init; }
}