using WebApiPizzeria.DTOs;
using WebApiPizzeria.Repositories.Interfaces;
using WebApiPizzeria.Services.Interfaces;

namespace WebApiPizzeria.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<BaseResponseDto<IEnumerable<ProductAvalaibilityDto>>> GetAvalaibleProducts()
    {
        try
        {
            var currentDay = DateTime.Now.DayOfWeek;
            var products = await _productRepository.GetAvalaibleProducts(currentDay);
            
            var response = new BaseResponseDto<IEnumerable<ProductAvalaibilityDto>>(true, products);
            return response;
        }
        catch (Exception)
        {
            var response = new BaseResponseDto<IEnumerable<ProductAvalaibilityDto>>(false, Enumerable.Empty<ProductAvalaibilityDto>());
            return response;
        }
    }
}

