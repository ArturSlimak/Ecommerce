using AutoMapper;
using CatalogService.DTOs;
using CatalogService.Models.Product;

namespace CatalogService.MappingProfiles;

public class ProductMP : Profile
{
    public ProductMP()
    {
        CreateMap<Product, ProductDTO.Index>();
        CreateMap<Product, ProductDTO.Details>();
        CreateMap<ProductDTO.ToCreate, Product>();
        CreateMap<ProductDTO.ToMutate, Product>();
        CreateMap<Product, ProductDTO.ToMutate>();
    }
}
