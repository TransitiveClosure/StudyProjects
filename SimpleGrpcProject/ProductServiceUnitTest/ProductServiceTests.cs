using Application;
using AutoFixture;
using AutoFixture.Xunit2;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Moq;

namespace ProductServiceUnitTest;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryFake = new(MockBehavior.Strict);
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _productService = new ProductService(_productRepositoryFake.Object);
    }

    [Fact]
    public void AddNewProduct_NewProductDto_ReturnsProductId()
    {
        // Arrange
        var fixture = new Fixture();
        var productDto = fixture.Create<ProductDto>();
        ulong expectedId = 100000000000;
        _productRepositoryFake.Setup(f => f.AddNewProduct(productDto)).Returns(expectedId);

        // Act
        ulong actualId = _productService.AddNewProduct(productDto);

        // Assert
        Assert.Equal(expectedId, actualId);
        _productRepositoryFake.Verify(f => f.AddNewProduct(productDto), Times.Once);
    }

    [Fact]
    public void GetProductById_ProductExist_ReturnsProduct()
    {
        // Arrange
        var fixture = new Fixture();
        ulong productId = 100000000000;
        var expectedProduct = fixture.Build<Product>().With(product => product.Id, productId).Create();
        _productRepositoryFake.Setup(f => f.GetProductById(productId)).Returns(expectedProduct);

        // Act
        var actualProduct = _productService.GetProductById(productId);

        // Assert
        Assert.Equal(expectedProduct, actualProduct);
        _productRepositoryFake.Verify(f => f.GetProductById(productId), Times.Once);
    }

    [Fact]
    public void GetProductById_ProductNotExist_DoNotCatchProductNotFoundException()
    {
        // Arrange
        ulong productId = 100000000000;
        _productRepositoryFake.Setup(f => f.GetProductById(productId))
            .Throws(new ProductNotFoundException(productId));

        // Act & Assert
        Assert.Throws<ProductNotFoundException>(() => _productService.GetProductById(productId));
        _productRepositoryFake.Verify(f => f.GetProductById(productId), Times.Once);
    }


    [Fact]
    public void UpdateProductPrice_ProductExist_DoesNotThrowException()
    {
        // Arrange
        ulong productId = 100000000000;
        double price = 0;
        _productRepositoryFake.Setup(f => f.UpdateProductPrice(productId, price));

        // Act
        _productService.UpdateProductPrice(productId, price);

        // Assert
        _productRepositoryFake.Verify(f => f.UpdateProductPrice(productId, price), Times.Once);
    }

    [Fact]
    public void UpdateProductPrice_ProductNotExist_DoNotCatchProductNotFoundException()
    {
        // Arrange
        ulong productId = 100000000000;
        double productNewPrice = 0;
        _productRepositoryFake.Setup(f => f.UpdateProductPrice(productId, productNewPrice))
            .Throws(new ProductNotFoundException(productId));

        // Act & Assert
        Assert.Throws<ProductNotFoundException>(() => _productService.UpdateProductPrice(productId, productNewPrice));
        _productRepositoryFake.Verify(f => f.UpdateProductPrice(productId, productNewPrice), Times.Once);
    }

    [Theory, AutoData]
    public void GetProductsByFilters_SuitableProductsExists_ReturnsListOfProducts(
        ProductFilteringOptions filteringOptions, int pageSize, int pageNumber)
    {
        // Arrange
        var fixture = new Fixture();
        var product = fixture.Create<Product>();
        var expectedProducts = new List<Product> { product };
        _productRepositoryFake.Setup(f =>
            f.GetProductsByFilters(filteringOptions, pageSize, pageNumber)).Returns(expectedProducts);

        // Act
        var actualProducts = _productService.GetProductsByFilters(filteringOptions, pageSize, pageNumber);

        // Assert
        Assert.Equal(expectedProducts, actualProducts);
        _productRepositoryFake.Verify(f =>
            f.GetProductsByFilters(filteringOptions, pageSize, pageNumber), Times.Once);
    }

    [Theory, AutoData]
    public void GetProductsByFilters_SuitableProductsNotExists_ReturnsEmptyListOfProducts(
        ProductFilteringOptions filteringOptions, int pageSize, int pageNumber)
    {
        // Arrange
        _productRepositoryFake.Setup(f =>
            f.GetProductsByFilters(filteringOptions, pageSize, pageNumber)).Returns(new List<Product>());

        // Act
        var actualProducts = _productService.GetProductsByFilters(filteringOptions, pageSize, pageNumber);

        // Assert
        Assert.Empty(actualProducts);
        _productRepositoryFake.Verify(f =>
            f.GetProductsByFilters(filteringOptions, pageSize, pageNumber), Times.Once);
    }
}