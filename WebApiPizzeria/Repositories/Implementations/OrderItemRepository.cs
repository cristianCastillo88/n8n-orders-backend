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
        var orderItemsToAdd = new List<OrderItem>();
        foreach (var product in dto.Products)
        {
            if (product.SubProducts != null && product.SubProducts.Any())
            {
                // Los combos siempre se crean como nuevos items (cada combo es independiente)
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

                    orderItemsToAdd.Add(childOrderItem);
                }
            }
            else
            {
                // Para productos simples (sin subproductos), verificar si ya existe en la orden
                var existingOrderItem = await _context.OrderItems
                    .FirstOrDefaultAsync(oi => oi.OrderId == dto.OrderId && 
                                               oi.ProductId == product.Id && 
                                               oi.ParentOrderItemId == null);

                if (existingOrderItem != null)
                {
                    // Si existe, sumar la cantidad
                    existingOrderItem.Quantity += product.Quantity;
                }
                else
                {
                    // Si no existe, crear nuevo item
                    var orderItem = new OrderItem
                    {
                        OrderId = dto.OrderId,
                        ProductId = product.Id,
                        Quantity = product.Quantity,
                        ParentOrderItemId = null
                    };

                    orderItemsToAdd.Add(orderItem);
                }
            }
        }

        // Agregar solo los items nuevos (combos hijos y productos nuevos)
        if (orderItemsToAdd.Any())
        {
            await _context.OrderItems.AddRangeAsync(orderItemsToAdd);
            await _context.SaveChangesAsync();
        }
        else
        {
            // Si solo se actualizaron cantidades de items existentes, guardar cambios
            await _context.SaveChangesAsync();
        }
    }

    public async Task<OrderItemDto> GetByOrderId(int orderId)
    {
        // Solo obtenemos items padre (no subproductos) para el cálculo de total
        // Los subproductos (combos) no deben sumarse al total, solo el precio del combo padre
        var orderItems = await _context.OrderItems
            .Include(oi => oi.Product)
            .Where(oi => oi.OrderId == orderId && oi.ParentOrderItemId == null)
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
        // Buscar items padre a eliminar
        var parentOrderItemsToDelete = await _context.OrderItems
            .Where(oi => oi.OrderId == orderId && 
                         productIds.Contains(oi.ProductId) && 
                         oi.ParentOrderItemId == null)
            .ToListAsync();

        if (parentOrderItemsToDelete.Any())
        {
            // Obtener los IDs de los items padre que se van a eliminar
            var parentIds = parentOrderItemsToDelete.Select(oi => oi.Id).ToList();

            // Buscar y eliminar también los subproductos (hijos) de estos combos
            var childOrderItemsToDelete = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId && 
                             oi.ParentOrderItemId.HasValue &&
                             parentIds.Contains(oi.ParentOrderItemId.Value))
                .ToListAsync();

            // Eliminar primero los hijos, luego los padres (para evitar problemas de FK)
            if (childOrderItemsToDelete.Any())
            {
                _context.OrderItems.RemoveRange(childOrderItemsToDelete);
            }

            _context.OrderItems.RemoveRange(parentOrderItemsToDelete);
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

