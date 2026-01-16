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
        var orderItems = new List<OrderItem>();
        foreach (var product in dto.Products)
        {
            if (product.SubProducts != null && product.SubProducts.Any())
            {
                var parentOrderItem = new OrderItem
                {
                    OrderId = dto.OrderId,
                    ProductId = product.Id,
                    Quantity = product.Quantity,
                    ParentOrderItemId = null
                };

                await _context.OrderItems.AddAsync(parentOrderItem);
                await _context.SaveChangesAsync();

                foreach (var subProduct in product.SubProducts)
                {
                    var childOrderItem = new OrderItem
                    {
                        OrderId = dto.OrderId,
                        ProductId = subProduct.Id,
                        Quantity = subProduct.Quantity,
                        ParentOrderItemId = parentOrderItem.Id
                    };

                    orderItems.Add(childOrderItem);
                }
            }
            else
            {
                var orderItem = new OrderItem
                {
                    OrderId = dto.OrderId,
                    ProductId = product.Id,
                    Quantity = product.Quantity,
                    ParentOrderItemId = null
                };

                orderItems.Add(orderItem);
            }
            if (orderItems.Any())
            {
                await _context.OrderItems.AddRangeAsync(orderItems);
                await _context.SaveChangesAsync();
            }
        }
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
    public async Task DeleteOrderItems(int orderId, List<int> productIds)
    {
        var orderItemsToDelete = await _context.OrderItems
            .Where(oi => oi.OrderId == orderId && productIds.Contains(oi.ProductId))
            .ToListAsync();

        if (orderItemsToDelete.Any())
        {
            _context.OrderItems.RemoveRange(orderItemsToDelete);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UpdateOrderItem(int orderId, int productId, decimal quantity)
    {
        var orderItem = await _context.OrderItems
            .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ProductId == productId);

        if (orderItem == null)
        {
            return false;
        }

        orderItem.Quantity = quantity;
        await _context.SaveChangesAsync();
        return true;
    }
}

