using Domain.Models;

namespace ProductServiceIntegrationTests.HttpEndpointsRequests;

public record GetProductsWithFilteringRequest(
    ProductCategory Category,
    DateTime StartDate,
    DateTime EndDate,
    ulong WarehouseId,
    int PageSize,
    int PageNumber);