﻿using AutoMapper;
using CatalogService.DTOs;
using CatalogService.Models.Product;

namespace CatalogService.MappingProfiles;

public class ProductMP : Profile
{
    public ProductMP()
    {
        CreateMap<Product, ProductDTO.Index>();
        CreateMap<ProductDTO.Mutate, Product>();
    }
}
