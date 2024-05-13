using System.Collections.Immutable;
using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductService
{
    public ulong AddNewProduct(ProductDto productDto);
    public Product GetProductById(ulong productId);
    public void UpdateProductPrice(ulong productId, double newPrice);
    public List<Product> GetProductsByFilters(ProductFilteringOptions filteringOptions, int pageSize, int pageNumber);
}