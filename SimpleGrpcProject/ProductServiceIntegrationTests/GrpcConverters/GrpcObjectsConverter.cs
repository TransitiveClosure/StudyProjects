using Domain.Entities;
using Domain.Models;
using GrpcProductServiceApi;
using Google.Protobuf.WellKnownTypes;
using Enum = System.Enum;

namespace ProductServiceIntegrationTests.GrpcConverters;

public static class GrpcObjectsConverter
{
    public static AddNewProductRequest ConvertToAddNewProductRequest(ProductDto productDto)
    {
        return new AddNewProductRequest()
        {
            Name = productDto.Name,
            Price = productDto.Price,
            Weight = productDto.Weight,
            Category = (ProductCategoryGrpc)Enum.Parse(typeof(ProductCategoryGrpc), productDto.Category.ToString()),
            DateOfCreation = productDto.DateOfCreation.ToTimestamp(),
            WarehouseId = productDto.WarehouseId
        };
    }

    public static GetProductsByFiltersRequest ConvertToGetProductsByFiltersRequest(
        ProductFilteringOptions filteringOptions, int pageSize,
        int pageNumber)
    {
        return new GetProductsByFiltersRequest()
        {
            WarehouseId = filteringOptions.WarehouseId,
            Category = (ProductCategoryGrpc)Enum.Parse(typeof(ProductCategoryGrpc),
                filteringOptions.Category.ToString()),
            StartDate = filteringOptions.StartDate.ToTimestamp(),
            EndDate = filteringOptions.EndDate.ToTimestamp(),
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    public static Product ConvertFromGetProductByIdResponse(GetProductByIdResponse response)
    {
        return ConvertProductGrpcToProduct(response.Product);
    }

    public static List<Product> ConvertFromGetProductsByFiltersResponse(GetProductsByFiltersResponse response)
    {
        return response.ProductsList.Select(ConvertProductGrpcToProduct).ToList();
    }

    private static Product ConvertProductGrpcToProduct(ProductGrpc productGrpc)
    {
        return new Product()
        {
            Id = productGrpc.Id,
            Name = productGrpc.Name,
            Price = productGrpc.Price,
            Weight = productGrpc.Weight,
            Category = (ProductCategory)Enum.Parse(typeof(ProductCategory),
                productGrpc.Category.ToString()),
            DateOfCreation = productGrpc.DateOfCreation.ToDateTime(),
            WarehouseId = productGrpc.WarehouseId
        };
    }
}