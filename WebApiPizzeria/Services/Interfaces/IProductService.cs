using WebApiPizzeria.DTOs;

namespace WebApiPizzeria.Services.Interfaces;

public interface IProductService
{
    Task<BaseResponseDto<IEnumerable<ProductAvalaibilityDto>>> GetAvalaibleProducts();
}

