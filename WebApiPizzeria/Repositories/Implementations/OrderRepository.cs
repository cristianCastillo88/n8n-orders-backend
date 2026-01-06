using Microsoft.EntityFrameworkCore;
using WebApiPizzeria.Models;
using WebApiPizzeria.Repositories.Interfaces;

namespace WebApiPizzeria.Repositories.Implementations;

public class OrderRepository : IOrderRepository
{
    private readonly PostgresContext _context;

    public OrderRepository(PostgresContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetLastOrderByUserId(string userId)
    {
        var twoHoursAgo = DateTime.UtcNow.AddHours(-2);

        var lastOrder = await _context.Orders
            .Where(o => o.UserId == userId && 
                   o.LastUpdate.HasValue && 
                   o.LastUpdate.Value >= twoHoursAgo &&
                   (o.OrderStateTypeId == (int)OrderStateTypeEnum.Open || o.OrderStateTypeId == null))
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        return lastOrder;
    }

    public async Task<Order> Save(string userId)
    {
        var lastOrderNumber = await _context.Orders
            .OrderByDescending(o => o.OrderNumber)
            .Select(o => o.OrderNumber)
            .FirstOrDefaultAsync();

        var nextOrderNumber = lastOrderNumber + 1;

        var newOrder = new Order
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            LastUpdate = DateTime.UtcNow,
            OrderNumber = nextOrderNumber,
            Total = 0
        };

        _context.Orders.Add(newOrder);
        await _context.SaveChangesAsync();
        return newOrder;
    }

    public async Task UpdateTotal(int orderId, decimal total)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.Total = total;
            order.LastUpdate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}

