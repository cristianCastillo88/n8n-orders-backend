using WebApiPizzeria.DTOs;

namespace WebApiPizzeria.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductAvalaibilityDto>> GetAvalaibleProducts();
}

