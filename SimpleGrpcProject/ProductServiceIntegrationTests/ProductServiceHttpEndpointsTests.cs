using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Entities;
using Domain.Models;
using GrpcProductServiceApi;
using Moq;
using Newtonsoft.Json.Linq;
using ProductServiceIntegrationTests.Fakers;
using ProductServiceIntegrationTests.HttpEndpointsRequests;

namespace ProductServiceIntegrationTests;

public class ProductServiceHttpEndpointsTests : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly CustomWebApplicationFactory<Startup> _webApplicationFactory;
    private readonly JsonSerializerOptions _serializerOptions;

    public ProductServiceHttpEndpointsTests(CustomWebApplicationFactory<Startup> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    [Fact]
    public async Task AddNewProduct_ProductDtoCorrect_ReturnsProductId()
    {
        // Arrange
        var client = _webApplicationFactory.CreateClient();
        var url = "/product";
        var jsonResponseContainerObjectName = "id";
        var productDto = new ProductDtoFaker().Generate(1).First();
        ulong expectedId = 100000000000;
        _webApplicationFactory.ProductRepositoryFake.Setup(f => f.AddNewProduct(productDto)).Returns(expectedId);
        var productDtoJson = JsonSerializer.Serialize(productDto, _serializerOptions);

        // Act
        var response =
            await client.PostAsync(url, new StringContent(productDtoJson, Encoding.UTF8, "application/json"));

        // Assert
        response.EnsureSuccessStatusCode();
        var responseJson = JObject.Parse(await response.Content.ReadAsStringAsync());
        var actualId = Convert.ToUInt64(responseJson[jsonResponseContainerObjectName]);
        Assert.Equal(expectedId, actualId);
        _webApplicationFactory.ProductRepositoryFake.Verify(f =>
            f.AddNewProduct(productDto), Times.Once);
    }

    [Fact]
    public async Task GetProductById_ProductExist_ReturnsProduct()
    {
        // Arrange
        var client = _webApplicationFactory.CreateClient();
        ulong productId = 100000000000;
        var url = $"/product/{productId}";
        var jsonResponseContainerObjectName = "product";
        var expectedProduct = new ProductFaker().RuleFor(o => o.Id, _ => productId).Generate(1).First();
        _webApplicationFactory.ProductRepositoryFake.Setup(f => f.GetProductById(productId)).Returns(expectedProduct);

        // Act
        var response = await client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseJson = JObject.Parse(await response.Content.ReadAsStringAsync());
        var actualProduct =
            JsonSerializer.Deserialize<Product>(responseJson[jsonResponseContainerObjectName].ToString(),
                _serializerOptions);
        Assert.Equal(expectedProduct, actualProduct);
        _webApplicationFactory.ProductRepositoryFake.Verify(f =>
            f.GetProductById(productId), Times.Once);
    }

    [Fact]
    public async Task UpdateProductPrice_ProductExist_DoesNotThrowException()
    {
        // Arrange
        var client = _webApplicationFactory.CreateClient();
        ulong productId = 100000000000;
        double productNewPrice = 100;
        var url = $"/product/{productId}";
        _webApplicationFactory.ProductRepositoryFake.Setup(f => f.UpdateProductPrice(productId, productNewPrice));
        var requestJson =
            JsonSerializer.Serialize(new UpdateProductRequest(productId, productNewPrice), _serializerOptions);

        // Act
        var response = await client.PostAsync(url, new StringContent(requestJson, Encoding.UTF8, "application/json"));

        // Assert
        response.EnsureSuccessStatusCode();
        _webApplicationFactory.ProductRepositoryFake.Verify(f =>
            f.UpdateProductPrice(productId, productNewPrice), Times.Once);
    }

    [Fact]
    public async Task GetProductsByFilters_SuitableProductsExists_ReturnsListOfProducts()
    {
        // Arrange
        var client = _webApplicationFactory.CreateClient();
        var url = $"/product/filtered";
        var jsonResponseContainerObjectName = "productsList";
        var filteringRequest = new GetProductsWithFilteringRequest(
            WarehouseId: 100000000000,
            Category: ProductCategory.General,
            StartDate: DateTime.MinValue.ToUniversalTime(),
            EndDate: DateTime.MaxValue.ToUniversalTime(),
            PageNumber: 1, PageSize: 3);
        var filteringOptions = new ProductFilteringOptions()
        {
            Category = filteringRequest.Category,
            StartDate = filteringRequest.StartDate,
            EndDate = filteringRequest.EndDate,
            WarehouseId = filteringRequest.WarehouseId
        };
        var expectedProducts = new ProductFaker().Generate(filteringRequest.PageSize);
        _webApplicationFactory.ProductRepositoryFake.Setup(f => f.GetProductsByFilters(
            filteringOptions, filteringRequest.PageSize, filteringRequest.PageNumber)).Returns(expectedProducts);
        var requestJson = JsonSerializer.Serialize(filteringRequest, _serializerOptions);

        // Act
        var response = await client.PostAsync(url, new StringContent(requestJson, Encoding.UTF8, "application/json"));

        // Assert
        response.EnsureSuccessStatusCode();
        var responseJson = JObject.Parse(await response.Content.ReadAsStringAsync());
        var actualProducts =
            JsonSerializer.Deserialize<List<Product>>(responseJson[jsonResponseContainerObjectName].ToString(),
                _serializerOptions);
        Assert.Equal(expectedProducts, actualProducts);
        _webApplicationFactory.ProductRepositoryFake.Verify(f =>
                f.GetProductsByFilters(filteringOptions, filteringRequest.PageSize, filteringRequest.PageNumber),
            Times.Once);
    }
}