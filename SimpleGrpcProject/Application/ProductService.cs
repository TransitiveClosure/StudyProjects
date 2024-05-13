using System.Collections.Immutable;
using Domain.Entities;
using Domain.Interfaces;

namespace Application;

public class ProductService(IProductRepository repository) : IProductService
{
    public ulong AddNewProduct(ProductDto productDto)
    {
        return repository.AddNewProduct(productDto);
    }

    public Product GetProductById(ulong productId)
    {
        return repository.GetProductById(productId);
    }

    public void UpdateProductPrice(ulong productId, double newPrice)
    {
        repository.UpdateProductPrice(productId, newPrice);
    }

    public List<Product> GetProductsByFilters(ProductFilteringOptions filteringOptions, int pageSize, int pageNumber)
    { 
        return repository.GetProductsByFilters(filteringOptions,  pageSize, pageNumber);
    }
}