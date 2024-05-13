namespace ProductServiceIntegrationTests.HttpEndpointsRequests;

public record UpdateProductRequest(ulong Id, double NewPrice);