using AutoMapper;
using InventoryService.DTOs;
using InventoryService.Models.Product;

namespace InventoryService.MappingProfiles;

public class ProductMP : Profile
{
    public ProductMP()
    {
        CreateMap<Product, ProductDTO.Index>();
        // CreateMap<ProductDTO.ToCreate, Product>();
        //CreateMap<ProductDTO.ToMutate, Product>();
        //CreateMap<Product, ProductDTO.ToMutate>();



    }
}
