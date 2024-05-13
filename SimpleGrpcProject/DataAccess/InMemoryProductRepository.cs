using System.Collections.Concurrent;
using System.Collections.Immutable;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;

namespace DataAccess;

public class InMemoryProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<ulong, Product> _products = [];
    private readonly IProductIdGenerator _productIdGenerator;

    public InMemoryProductRepository(IProductIdGenerator productIdGenerator)
    {
        _productIdGenerator = productIdGenerator;
    }
    
    public ulong AddNewProduct(ProductDto productDto)
    {
        ulong newProductId = _productIdGenerator.GetId();
        var newProduct = new Product()
        {
            Id = newProductId,
            Name = productDto.Name,
            Price = productDto.Price,
            Weight = productDto.Weight,
            Category = productDto.Category,
            DateOfCreation = productDto.DateOfCreation,
            WarehouseId = productDto.WarehouseId
        };
        if (!_products.TryAdd(newProductId, newProduct))
        {
            throw new ProductAlreadyExistException(newProductId);
        }
        return newProductId;
    }

    public Product GetProductById(ulong productId)
    {
        Product product = IfExistGetProduct(productId);
        return product;
    }
 
    public void UpdateProductPrice(ulong productId, double newPrice)
    {
        Product product = IfExistGetProduct(productId);
        Product updatedProduct = product with { Price = newPrice };
        if (!_products.TryUpdate(productId, updatedProduct, product))
        {
            throw new ProductNotFoundException(productId);
        }
    }

    public List<Product> GetProductsByFilters(ProductFilteringOptions filteringOptions, int pageSize, int pageNumber)
    {
        return _products.Values.ToList()
            .Where(item => item.Category == filteringOptions.Category)
            .Where(item => (item.DateOfCreation >= filteringOptions.StartDate) 
                           && (item.DateOfCreation < filteringOptions.EndDate))
            .Where(item => item.WarehouseId == filteringOptions.WarehouseId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    private Product IfExistGetProduct(ulong productId)
    {
        if (_products.TryGetValue(productId, out Product? product))
        {
            return product;
        }
        throw new ProductNotFoundException(productId);
    }
}