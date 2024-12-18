﻿using CatalogService.Helpers;

namespace CatalogService.Services;

public interface IProductsService
{
    Task<ProductResponse.GetIndex> GetAsync(ProductRequest.Index request);
    Task<ProductResponse.Create> CreateAsync(ProductRequest.Create request);
    Task<ProductResponse.Mutate> MutateAsync(ProductRequest.Mutate request, string publicId);
    Task SoftDeleteProductAsync(string publicId);
    Task<ProductResponse.GetDetails> GetByIdAsync(string publicid);
}