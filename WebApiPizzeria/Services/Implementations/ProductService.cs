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

    public async Task<IEnumerable<ProductAvalaibilityDto>> GetAvalaibleProducts()
    {
        var currentDay = DateTime.Now.DayOfWeek;
        var products = await _productRepository.GetAvalaibleProducts(currentDay);
        return products;
    }
}

