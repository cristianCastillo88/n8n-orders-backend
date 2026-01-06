using Microsoft.EntityFrameworkCore;
using WebApiPizzeria.DTOs;
using WebApiPizzeria.Models;
using WebApiPizzeria.Repositories.Interfaces;

namespace WebApiPizzeria.Repositories.Implementations;

public class ProductRepository : IProductRepository
{
    private readonly PostgresContext _context;

    public ProductRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProductAvalaibilityDto>> GetAvalaibleProducts(DayOfWeek currentDay)
    {
        var dayTypeId = (int)currentDay;

        var products = await _context.Products
            .Include(p => p.ProductType)
            .Where(p => 
                p.ProductAvailabilityDays.Any(pad => pad.DayTypeId == dayTypeId) ||
                !p.ProductAvailabilityDays.Any())
            .ToListAsync();

        var productsDto = products.Select(p => new ProductAvalaibilityDto
        {
            Id = p.Id,
            Description = p.Description,
            ProductTypeName = p.ProductType.Name,
            Price = p.Price
        });

        return productsDto;
    }
}

