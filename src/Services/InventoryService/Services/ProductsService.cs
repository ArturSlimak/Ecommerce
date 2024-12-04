using AutoMapper;
using InventoryService.DTOs;
using InventoryService.Helpers;
using InventoryService.Repositories;

namespace InventoryService.Services
{
    public class ProductsService : IProductsService
    {

        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductsService(IProductRepository productRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<ProductResponse.GetIndex> GetAsync(ProductRequest.Index request)
        {
            var products = await _productRepository.GetProductsAsync();
            var productDTOs = _mapper.Map<List<ProductDTO.Index>>(products);

            var pageSize = productDTOs.Count;
            var response = new ProductResponse.GetIndex
            {
                Products = productDTOs,
                PageSize = pageSize,
                Page = request.Page,
            };

            return response;
        }
    }
}
