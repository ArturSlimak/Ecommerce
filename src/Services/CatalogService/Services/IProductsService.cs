﻿using CatalogService.Helpers;

namespace CatalogService.Services;

public interface IProductsService
{
    Task<ProductResponse.Create> CreateAsync(ProductRequest.Create request);
    Task<ProductResponse.GetIndex> GetAsync(ProductRequest.Index request);
}