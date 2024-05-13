using Domain.Entities;
using Domain.Models;
using GrpcProductServiceApi;
using Moq;
using ProductServiceIntegrationTests.Fakers;
using ProductServiceIntegrationTests.GrpcConverters;
using ProductServiceIntegrationTests.GrpcFixture;

namespace ProductServiceIntegrationTests;

public class ProductServiceGrpcEndpointsTests(GrpcTestFixture<Startup> fixture) : GrpcIntegrationTestBase(fixture)
{
    [Fact]
    public async Task AddNewProduct_ProductDtoCorrect_ReturnsProductId()
    {
        // Arrange 
        var client = new GrpcProductService.GrpcProductServiceClient(Channel);
        var productDto = new ProductDtoFaker().Generate(1).First();
        ulong expectedId = 100000000000;
        var request = GrpcObjectsConverter.ConvertToAddNewProductRequest(productDto);
        Fixture.ProductRepositoryFake.Setup(f => f.AddNewProduct(productDto)).Returns(expectedId);

        // Act
        var response = await client.AddNewProductAsync(request);

        // Assert
        ulong actualId = response.Id;
        Assert.Equal(expectedId, actualId);
        Fixture.ProductRepositoryFake.Verify(f =>
            f.AddNewProduct(productDto), Times.Once);
    }

    [Fact]
    public async Task GetProductById_ProductExist_ReturnsProduct()
    {
        // Arrange
        var client = new GrpcProductService.GrpcProductServiceClient(Channel);
        ulong productId = 100000000000;
        var expectedProduct = new ProductFaker().RuleFor(o => o.Id, _ => productId).Generate(1).First();
        Fixture.ProductRepositoryFake.Setup(f => f.GetProductById(productId)).Returns(expectedProduct);
        var request = new GetProductByIdRequest() { Id = productId };

        // Act
        var response = await client.GetProductByIdAsync(request);

        // Assert
        var actualProduct = GrpcObjectsConverter.ConvertFromGetProductByIdResponse(response);
        Assert.Equal(expectedProduct, actualProduct);
        Fixture.ProductRepositoryFake.Verify(f =>
            f.GetProductById(productId), Times.Once);
    }

    [Fact]
    public async Task UpdateProductPrice_ProductExist_DoesNotThrowException()
    {
        // Arrange
        var client = new GrpcProductService.GrpcProductServiceClient(Channel);
        ulong productId = 100000000000;
        double productNewPrice = 100;
        Fixture.ProductRepositoryFake.Setup(f => f.UpdateProductPrice(productId, productNewPrice));
        var request = new UpdateProductPriceRequest() { Id = productId, NewPrice = productNewPrice };

        // Act
        var _ = await client.UpdateProductPriceAsync(request);

        // Assert
        Fixture.ProductRepositoryFake.Verify(f =>
            f.UpdateProductPrice(productId, productNewPrice), Times.Once);
    }

    [Fact]
    public async Task GetProductsByFilters_SuitableProductsExists_ReturnsListOfProducts()
    {
        // Arrange
        var client = new GrpcProductService.GrpcProductServiceClient(Channel);
        int pageNumber = 1;
        int pageSize = 3;
        var filteringOptions = new ProductFilteringOptions()
        {
            Category = ProductCategory.General,
            StartDate = DateTime.MinValue.ToUniversalTime(),
            EndDate = DateTime.MaxValue.ToUniversalTime(),
            WarehouseId = 100000000000
        };
        var expectedProducts = new ProductFaker().Generate(pageSize);
        Fixture.ProductRepositoryFake.Setup(f => f.GetProductsByFilters(
            filteringOptions, pageSize, pageNumber)).Returns(expectedProducts);
        var request = GrpcObjectsConverter.ConvertToGetProductsByFiltersRequest(filteringOptions, pageSize, pageNumber);

        // Act
        var response = await client.GetProductsByFiltersAsync(request);

        // Assert
        var actualProducts = GrpcObjectsConverter.ConvertFromGetProductsByFiltersResponse(response);
        Assert.Equal(expectedProducts, actualProducts);
        Fixture.ProductRepositoryFake.Verify(f =>
                f.GetProductsByFilters(filteringOptions, pageSize, pageNumber),
            Times.Once);
    }
}