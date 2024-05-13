using Domain.Entities;
using Domain.Interfaces;
using Domain.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcProductServiceApi;
using Enum = System.Enum;

namespace GrpcProductServiceApi.Services;

public class ProductServiceGrpc : GrpcProductService.GrpcProductServiceBase
{
    private readonly IProductService _productService;

    public ProductServiceGrpc(IProductService productService)
    {
        _productService = productService;
    } 

    public override Task<AddNewProductResponse> AddNewProduct(AddNewProductRequest request, ServerCallContext context)
    {
        var productDto = new ProductDto()
        {
            Name = request.Name,
            Price = request.Price,
            Weight = request.Weight,
            Category = (ProductCategory)Enum.Parse(typeof(ProductCategory), request.Category.ToString()),
            DateOfCreation = request.DateOfCreation.ToDateTime(),
            WarehouseId = request.WarehouseId
        };  
        
        return Task.FromResult(new AddNewProductResponse
        {
            Id = _productService.AddNewProduct(productDto)
        });
    }
    
    public override Task<GetProductByIdResponse> GetProductById(GetProductByIdRequest request, ServerCallContext context)
    {
        var product = _productService.GetProductById(request.Id);
        var productGrpc = new ProductGrpc() {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Weight = product.Weight,
            Category = (ProductCategoryGrpc)Enum.Parse(typeof(ProductCategoryGrpc), product.Category.ToString()),
            DateOfCreation = product.DateOfCreation.ToTimestamp(),
            WarehouseId = product.WarehouseId };
        
        return Task.FromResult(new GetProductByIdResponse
        {
            Product = productGrpc
        });
    }

    public override Task<Empty> UpdateProductPrice(UpdateProductPriceRequest updateProductPriceRequest,
        ServerCallContext context)
    {
        _productService.UpdateProductPrice(updateProductPriceRequest.Id, updateProductPriceRequest.NewPrice);
        return Task.FromResult(new Empty());
    }
    
    public override Task<GetProductsByFiltersResponse> GetProductsByFilters(GetProductsByFiltersRequest request, 
        ServerCallContext context)
    {
        var productFilterOptions  = new ProductFilteringOptions() {
            Category = (ProductCategory)Enum.Parse(typeof(ProductCategory), request.Category.ToString()),
            StartDate = request.StartDate.ToDateTime(),
            EndDate = request.EndDate.ToDateTime(),
            WarehouseId = request.WarehouseId 
        };
        var products = _productService.GetProductsByFilters(productFilterOptions, request.PageSize, request.PageNumber);
        
        return Task.FromResult(new GetProductsByFiltersResponse
        {
            ProductsList =
            {
                products.Select(product => new ProductGrpc()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    Weight = product.Weight,
                    Category = (ProductCategoryGrpc)Enum.Parse(typeof(ProductCategoryGrpc), product.Category.ToString()),
                    DateOfCreation = product.DateOfCreation.ToTimestamp(),
                    WarehouseId = product.WarehouseId 
                })
            }
        });
    }
}