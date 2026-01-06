using Microsoft.EntityFrameworkCore;
using WebApiPizzeria.DTOs;
using WebApiPizzeria.Models;
using WebApiPizzeria.Repositories.Interfaces;

namespace WebApiPizzeria.Repositories.Implementations;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly PostgresContext _context;

    public OrderItemRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task SaveRange(OrderItemPostDto dto)
    {
        var orderItems = dto.Products.Select(product => new OrderItem
        {
            OrderId = dto.OrderId,
            ProductId = product.Id,
            Quantity = product.Quantity
        }).ToList();

        await _context.OrderItems.AddRangeAsync(orderItems);
        await _context.SaveChangesAsync();
    }

    public async Task<OrderItemDto> GetByOrderId(int orderId)
    {
        var orderItems = await _context.OrderItems
            .Include(oi => oi.Product)
            .Where(oi => oi.OrderId == orderId)
            .ToListAsync();

        var items = orderItems.Select(oi => new OrderItemDetailDto
        {
            ProductId = oi.ProductId,
            Quantity = oi.Quantity,
            Price = oi.Product.Price
        }).ToList();

        return new OrderItemDto
        {
            Items = items,
            Total = 0
        };
    }
}

