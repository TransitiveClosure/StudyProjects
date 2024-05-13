using System.Collections.Immutable;
using Domain.Entities;
using Domain.Models;

namespace Domain.Interfaces;

public interface IProductRepository
{
    public ulong AddNewProduct(ProductDto productDto);
    public Product GetProductById(ulong productId);
    public void UpdateProductPrice(ulong productId, double newPrice);
    public List<Product> GetProductsByFilters(ProductFilteringOptions filteringOptions, int pageSize, int pageNumber);
}