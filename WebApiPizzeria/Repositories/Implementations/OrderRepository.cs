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

    public async Task<Product?> GetProductByIdAsync(int productId)
    {
        return await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        return await _context.Orders
            .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    public async Task<OrderItem?> GetOrderItemAsync(int orderId, int productId)
    {
        return await _context.OrderItems
            .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ProductId == productId);
    }

    public async Task AddOrderItemAsync(OrderItem orderItem)
    {
        await _context.OrderItems.AddAsync(orderItem);
    }

    public void RemoveOrderItem(OrderItem orderItem)
    {
        _context.OrderItems.Remove(orderItem);
    }

    public void DeleteOrderItem(OrderItem orderItem)
    {
        _context.OrderItems.Remove(orderItem);
    }

    public void UpdateOrder(Order order)
    {
        _context.Orders.Update(order);
    }

    public async Task<bool> SaveChangesAsync()
    {
        try
        {
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}
