using WebApiPizzeria.DTOs;

namespace WebApiPizzeria.Repositories.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<ProductAvalaibilityDto>> GetAvalaibleProducts(DayOfWeek currentDay);
}

